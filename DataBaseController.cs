// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using MySql.Data.MySqlClient;
using NetworkUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TankWars
{
    /// <summary>
    /// This class represents the controller between the server and the database.
    /// Data is passed around using SQL.
    /// </summary>
    public class DataBaseController
    {
        // Access to the world
        private World theWorld;
        // Keeps track of the games duration
        private Stopwatch duration;
        // The web server, needed to properly close the server
        private TcpListener webTcp;
        // Connection string to the used database
        private const string connectionString = "server=atr.eng.utah.edu;database=cs3500_u1053412;uid=cs3500_u1053412;password=hi12345";

        /// <summary>
        /// Initializes the database controller
        /// </summary>
        /// <param name="w">The world</param>
        public DataBaseController(World w)
        {
            theWorld = w;

            duration = new Stopwatch();
            duration.Start();
        }

        /// <summary>
        /// Starts the web server
        /// </summary>
        public void StartWebServer()
        {
            webTcp = Networking.StartServer(HandleHTTPConnection, 80);
        }

        /// <summary>
        /// Saves all the game information to the database
        /// </summary>
        public void SaveToDatabase()
        {
            // This section of code gets the data from the world
            duration.Stop();
            List<string> players = new List<string>();
            Dictionary<string, int> acc = new Dictionary<string, int>();
            Dictionary<string, int> score = new Dictionary<string, int>();
            foreach (Tank tank in theWorld.GetTanks())
            {
                string name = tank.Name;
                players.Add(name);
                if (tank.ShotsTaken == 0)
                    acc[name] = 0;
                else
                    acc[name] = (tank.ShotsHit *100 / tank.ShotsTaken);
                score[name] = tank.score;
            }
            // This section of code uses the collected data and puts it into the database with SQL
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand command = conn.CreateCommand();
                    int d = (int)duration.ElapsedMilliseconds / 1000;
                    command.CommandText = "insert into Games (Duration) values(" + d + ");";
                    int gameID = -1;
                    // Stores new row in game table
                    command.ExecuteReader().Close();

                    command.CommandText = "select MAX(GameID) from Games;";
                    // Gets the new game id from the database
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            gameID = Convert.ToInt32(reader.GetValue(0));
                        }
                        reader.Close();
                    }
                    // Stores the player information and associates the players with the game
                    foreach (string player in players)
                    {
                        command.CommandText = "insert into Players values('" + player + "'," + score[player] + "," + acc[player] + ");";
                        command.ExecuteReader().Close();
                        command.CommandText = "insert into PlayersInGame values(" + gameID + ",'" + player + "');";
                        command.ExecuteReader().Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Closes the web server
        /// </summary>
        public void CloseWebServer()
        {
            Networking.StopServer(webTcp);
        }

        /// <summary>
        /// Handles the initial handshake with HTTP client
        /// </summary>
        /// <param name="state">The connection</param>
        private void HandleHTTPConnection(SocketState state)
        {
            if (state.ErrorOccured)
                return;
            Networking.GetData(state);
            state.OnNetworkAction = GetHTTPRequest;
        }
        /// <summary>
        /// Gets the request from the HTTP client and handles it appropriately.
        /// Only send and close is used for each request. IF there is an error,
        /// it will be written to the console but the program will continue.
        /// </summary>
        /// <param name="state">The connection</param>
        private void GetHTTPRequest(SocketState state)
        {
            if (state.ErrorOccured)
                return;
            string data = state.GetData();
            string[] parts = Regex.Split(data, @"(?<=[\r\n])");
            string request = parts[0];

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand command = conn.CreateCommand();

                    // If localhost is connected but no request is made this will be the default view
                    if (request == "GET / HTTP/1.1\r")
                        Networking.SendAndClose(state.TheSocket, WebViews.GetHomePage());
                    else if (request == "GET /games HTTP/1.1\r")
                    {
                        // Get the games and display the proper view
                        command.CommandText = "select * from Games;";
                        
                        Dictionary<uint, GameModel> gmList = new Dictionary<uint, GameModel>();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                uint gameID = Convert.ToUInt32(reader.GetValue(0));
                                uint duration = Convert.ToUInt32(reader.GetValue(1));
                                GameModel gm = new GameModel(gameID, duration);
                                gmList.Add(gameID, gm);
                            }
                        }
                        command.CommandText = "select * from Players natural join PlayersInGame;";
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = (string)reader.GetValue(0);
                                uint score = Convert.ToUInt32(reader.GetValue(1));
                                uint accuracy = Convert.ToUInt32(reader.GetValue(2));
                                uint gameID = Convert.ToUInt32(reader.GetValue(3));
                                gmList[gameID].AddPlayer(name, score, accuracy);
                            }
                        }
                        Networking.SendAndClose(state.TheSocket, WebViews.GetAllGames(gmList));
                    }
                    else if (request.Contains("GET /games?player="))
                    { 
                        // Get the data associated with the player named in between the angle brackets
                        string playerName = Regex.Match(request, @"%3C(.*?)%3E").Groups[1].Value; // Parse the input name from the angle brackets
                        command.CommandText = "select * from Players natural join PlayersInGame natural join Games where Name = '" + playerName + "';";
                        List<SessionModel> smList = new List<SessionModel>();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                uint gameID = Convert.ToUInt32(reader.GetValue(0));
                                uint score = Convert.ToUInt32(reader.GetValue(2));
                                uint accuracy = Convert.ToUInt32(reader.GetValue(3));
                                uint duration = Convert.ToUInt32(reader.GetValue(4));
                                SessionModel sm = new SessionModel(gameID, duration, score, accuracy);
                                smList.Add(sm);
                            }
                        }
                        Networking.SendAndClose(state.TheSocket, WebViews.GetPlayerGames(playerName, smList));
                    }
                    else
                        Networking.SendAndClose(state.TheSocket, WebViews.Get404());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
