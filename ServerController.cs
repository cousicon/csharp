// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Xml;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TankWars
{
    /// <summary>
    /// This class represents the controller between the server and the client for the game.
    /// </summary>
    public class ServerController
    {
        // Access to the game world
        private World theWorld;
        // List of clients to keep track of connection
        private Dictionary<long, SocketState> clients;
        // List keeps track of the id list between the clients and the tank ID as an integer
        private Dictionary<long, int> idList;
        // Access to settings.
        private Settings settings;
        // Keeps track of playerIDs
        private static int playerIDs = 0;
        // The server, needed to properly close
        private TcpListener gameTcp;
        // Keeps track of all the commands to update
        public Dictionary<int, ControlCommands> CommandsToUpdate { get; private set; }

        /// <summary>
        /// Initializes the controller with all the needed fields.
        /// </summary>
        public ServerController()
        {
            theWorld = new World();
            settings = new Settings(theWorld);
            clients = new Dictionary<long, SocketState>();
            CommandsToUpdate = new Dictionary<int, ControlCommands>();
            idList = new Dictionary<long, int>();
        }

        /// <summary>
        /// Begins the server
        /// </summary>
        public void BeginServer()
        {
            gameTcp = Networking.StartServer(HandleClientConnection, 11000);
        }

        /// <summary>
        /// Handles the initial connection with a game client. Simply starts the handshake.
        /// </summary>
        /// <param name="state">the connection</param>
        private void HandleClientConnection(SocketState state)
        {
            if (state.ErrorOccured)
            {
                return;
            }
            lock (clients)
            {
                state.OnNetworkAction = ReceivePlayerName;
                Networking.GetData(state);
            }
        }

        /// <summary>
        /// Gets the player name and parses it properly. The world size and player ID are sent
        /// followed by the walls
        /// </summary>
        /// <param name="state">The connection</param>
        private void ReceivePlayerName(SocketState state)
        {
            if (state.ErrorOccured)
            {
                RemoveClient(state.ID);
                return;
            }
            // parse the name sent
            string playerName = state.GetData();
            if (playerName.EndsWith("\n"))
                playerName = playerName.Substring(0, playerName.Length - 1);
            state.RemoveData(0, playerName.Length);

            if (playerName.Length > 16 || playerName is null) // if the name is improper disconnect the client
            {
                RemoveClient(state.ID);
                return;
            }
            playerIDs = (int)state.ID;
            lock (theWorld)
            {
                Tank tank = new Tank(playerName, playerIDs, theWorld.SpawnPoint());
                theWorld.AddTank(tank);
            }
            Networking.Send(state.TheSocket, playerIDs + "\n" + settings.GetWorldSize() + settings.GetWallsToSend());
            lock (clients)
            {
                clients[state.ID] = state;
                idList[state.ID] = playerIDs;
                state.OnNetworkAction = CommandReceived;
                Networking.GetData(state);
            }
        }
        /// <summary>
        /// When a command is sent to the server this method handles that data.
        /// This method parses the data and puts it in a dictionary for the main
        /// loop to access.
        /// </summary>
        /// <param name="state">The connection</param>
        private void CommandReceived(SocketState state)
        {
            if (state.ErrorOccured)
                return;

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            lock (CommandsToUpdate)
            {
                foreach (string clientCommand in parts)
                {
                    string commandText = clientCommand;
                    if (clientCommand.Length == 0)
                        continue;
                    if (clientCommand == "\n")
                        continue;
                    if (clientCommand is null)
                        continue;
                    if (clientCommand[clientCommand.Length - 1] != '\n')
                        break;
                    else
                        commandText = clientCommand.Substring(0, clientCommand.Length - 1);

                    state.RemoveData(0, clientCommand.Length);

                    ControlCommands command = JsonConvert.DeserializeObject<ControlCommands>(commandText);

                    if (command.TankDirection.IsNaN())
                        continue;

                    CommandsToUpdate[idList[state.ID]] = command;
                }
                Networking.GetData(state);
            }
        }
        /// <summary>
        /// Sends the updated world on every frame.
        /// </summary>
        /// <param name="updatedJson"></param>
        public void SendUpdates(string updatedJson)
        {
            HashSet<long> disconnectedClients = new HashSet<long>();
            lock (clients)
            {
                foreach (SocketState client in clients.Values)
                {
                    if (client.ErrorOccured)
                    {
                        disconnectedClients.Add(client.ID);
                        continue;
                    }
                    else if (!Networking.Send(client.TheSocket, updatedJson))
                        disconnectedClients.Add(client.ID);
                }
            }
            foreach (long id in disconnectedClients)
            {
                RemoveClient(id);
            }
        }
        /// <summary>
        /// Stops the game server.
        /// </summary>
        public void StopServer()
        {
            Networking.StopServer(gameTcp);
        }

        /// <summary>
        /// Safely removes the client and sets a tank to be Disconnected.
        /// </summary>
        /// <param name="id"></param>
        private void RemoveClient(long id)
        {
            lock (idList)
            {
                theWorld.SetTankDC(idList[id]);
                CommandsToUpdate.Remove(idList[id]);
                idList.Remove(id);
            }
            lock (clients)
            {
                if (clients.ContainsKey(id))
                {
                    clients[id].ClearData();
                    clients[id].TheSocket.Close();
                    clients.Remove(id);
                }
            }
        }

        /// <summary>
        /// Gets the values from the settings file that are needed in the world and main program
        /// </summary>
        /// <param name="frameRate">Frame Rate</param>
        /// <param name="shotRate">Tank shoot rate</param>
        /// <param name="respawnRate">Respawn rate of tanks</param>
        public void GetFrameValues(out int frameRate, out int shotRate, out int respawnRate)
        {
            settings.GetFrameValues(out frameRate, out shotRate, out respawnRate);
        }

        /// <summary>
        /// Gives the form access to the world being created.
        /// </summary>
        /// <returns>The model of the world</returns>
        public World GetWorld()
        {
            return theWorld;
        }

        /// <summary>
        /// Enumerates the control commands sent from clients
        /// </summary>
        /// <returns>Commands</returns>
        public IEnumerable<ControlCommands> GetNewCommands()
        {
            return CommandsToUpdate.Values;
        }

        /// <summary>
        /// Enumerates the keys of the commands dictionary which is associated with the IDs
        /// </summary>
        /// <returns>ID enumerable</returns>
        public IEnumerable<int> GetCommandKeys()
        {
            return CommandsToUpdate.Keys;
        }
    }
}
