// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
namespace TankWars
{
    /// <summary>
    /// This class parses the settings file for the server to use.
    /// </summary>
    public class Settings
    {
        // General dictionary containing the customizable settings values
        private Dictionary<string, string> settings;
        // Contains all the walls and ID keys
        private Dictionary<int, Wall> walls;
        // The wall IDs
        private int wallCounter = 0;
        // The settings to send to clients
        private StringBuilder settingsToSend;

        /// <summary>
        /// Initializes a settings object and immediately parses the settings file which
        /// is located in the resources folder for the TankWars solution.
        /// </summary>
        /// <param name="theWorld">The World</param>
        public Settings(World theWorld)
        {
            settings = new Dictionary<string, string>();
            walls = new Dictionary<int, Wall>();
            settingsToSend = new StringBuilder();

            try
            {

                using (XmlReader reader = XmlReader.Create("..\\..\\..\\Resources\\settings.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "GameSettings":
                                    break;
                                case "UniverseSize":
                                    reader.Read();
                                    theWorld.WorldSize = int.Parse(reader.Value);
                                    settings.Add("UniverseSize", reader.Value + "\n");
                                    break;
                                case "MSPerFrame":
                                    reader.Read();
                                    settings.Add("MSPerFrame", reader.Value);
                                    break;
                                case "FramesPerShot":
                                    reader.Read();
                                    settings.Add("FramesPerShot", reader.Value);
                                    break;
                                case "RespawnRate":
                                    reader.Read();
                                    settings.Add("RespawnRate", reader.Value);
                                    break;
                                case "Wall":
                                    reader.Read(); // Need to read through all the new line characters and other arbitrary characters
                                    reader.Read();
                                    reader.Read();
                                    reader.Read();
                                    Vector2D p1;
                                    Vector2D p2;
                                    double x1 = reader.ReadElementContentAsDouble();
                                    reader.Read();
                                    double y1 = reader.ReadElementContentAsDouble();
                                    p1 = new Vector2D(x1, y1);
                                    reader.Read();
                                    reader.Read();
                                    reader.Read();
                                    reader.Read();
                                    reader.Read();
                                    double x2 = reader.ReadElementContentAsDouble();
                                    reader.Read();
                                    double y2 = reader.ReadElementContentAsDouble();
                                    p2 = new Vector2D(x2, y2);

                                    wallCounter++;
                                    Wall wall = new Wall(wallCounter, p1, p2);
                                    theWorld.AddWall(wall);
                                    string serialWall = JsonConvert.SerializeObject(wall) + "\n";
                                    walls.Add(wall.wall, wall);
                                    settingsToSend.Append(serialWall);
                                    break;
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Getter method for the world size;
        /// </summary>
        /// <returns>The World Size</returns>
        public string GetWorldSize()
        {
            return settings["UniverseSize"];
        }

        /// <summary>
        /// Getter method for the walls as strings
        /// </summary>
        /// <returns>Walls in string format</returns>
        public string GetWallsToSend()
        {
            return settingsToSend.ToString();
        }

        /// <summary>
        /// Gets frame values from the parsed settings
        /// </summary>
        /// <param name="frameRate">Server frame rate</param>
        /// <param name="shotRate">Tank fire rate</param>
        /// <param name="respawnRate">Tank respawn rate</param>
        public void GetFrameValues(out int frameRate, out int shotRate, out int respawnRate)
        {
            frameRate = int.Parse(settings["MSPerFrame"]);
            shotRate = int.Parse(settings["FramesPerShot"]);
            respawnRate = int.Parse(settings["RespawnRate"]);
        }

    }
}
