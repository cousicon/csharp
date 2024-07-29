// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace TankWars
{
    /// <summary>
    /// This is the main console program. Represents the view of the server.
    /// </summary>
    class Program
    {
        /// <summary>
        /// This internal class handles the infinite loop for updating and sending the world.
        /// </summary>
        internal class ThreadLooper
        {
            // The controller for the game on the server side
            private ServerController c;
            // Access to the world
            private World theWorld;
            // Closing flag for the infinite loop, activated by entering a key in the console
            private bool closeFlag;
            // FrameRate from the settings file
            private int frameRate;

            /// <summary>
            /// Initializes the looper for the thread
            /// </summary>
            /// <param name="w">The world</param>
            /// <param name="contr">The controller</param>
            public ThreadLooper(World w, ServerController contr, int rate)
            {
                theWorld = w;
                c = contr;
                frameRate = rate;
                closeFlag = false;
            }

            /// <summary>
            /// This method runs the bulk of the program by updating the world.
            /// The server can receive control commands at any time, it uses the latest one
            /// of the associated player ID. The world is sent every 17 milliseconds, or
            /// approximately 60 frames per second.
            /// </summary>
            public void InfiniteLoop()
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    while (sw.ElapsedMilliseconds < frameRate)
                    { Thread.Sleep(1); }

                    lock (theWorld)
                    {
                        foreach (int id in c.CommandsToUpdate.Keys.ToList())
                        {
                            ControlCommands cc = c.CommandsToUpdate[id];
                            theWorld.UpdateOnCommand(id, cc.Moving, cc.Fire, cc.TankDirection);
                        }
                        theWorld.UpdateWorld();
                        string updates = theWorld.SerializeWorld();
                        if (!(updates.Length == 0))
                        {
                            c.SendUpdates(updates); 
                            theWorld.CleanUpWorld();
                        }
                    }
                    if (closeFlag)
                        break;
                    sw.Restart();
                }
            }
            /// <summary>
            /// Sets the flag to true;
            /// </summary>
            public void ClosingFlag()
            {
                closeFlag = true;
            }
        }

        /// <summary>
        /// Main method. Starts up the game and web server. Initializes the view and controller.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ServerController c = new ServerController();
            World theWorld = c.GetWorld();
            c.GetFrameValues(out int frameRate, out int shotRate, out int respawnRate);
            theWorld.SetServerSettings(shotRate, respawnRate);
            c.BeginServer();
            
            ThreadLooper thread = new ThreadLooper(theWorld, c, frameRate);
            
            Thread looper = new Thread(thread.InfiniteLoop);
            looper.Start();

            DataBaseController dbc = new DataBaseController(theWorld);
            dbc.StartWebServer();

            Console.WriteLine("Server is running.");
            Console.ReadLine();

            thread.ClosingFlag();
            looper.Join();
            dbc.SaveToDatabase();
            c.StopServer();
        }
    }
}
