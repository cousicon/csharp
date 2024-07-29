// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    /// <summary>
    /// This class represents a tank object. It contains information about the tank in Json.
    /// The tank is associated with the player ID and will primarily be controlled by the client.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {

        // Unique ID for the tank
        [JsonProperty(PropertyName = "tank")]
        public int ID { get; private set; }

        // Current location of the tank as a 2D vector
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        // Orientation of the tank
        [JsonProperty(PropertyName = "bdir")]
        public Vector2D Orientation { get;  set; }

        // direction the turret is facing (towards the mouse cursor)
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D Aiming { get; set; }

        // Player name
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }

        // hitpoints of the tank
        [JsonProperty(PropertyName = "hp")]
        public int hitPoints = Constants.MaxHP;

        // Player score
        [JsonProperty(PropertyName = "score")]
        public int score = 0;

        // Tells whether the tank died and needs to respawn
        [JsonProperty(PropertyName = "died")]
        public bool died = false;

        // Determines whether the player has disconnected
        [JsonProperty(PropertyName = "dc")]
        public bool disconnected = false;

        // Determines if the player has joined
        [JsonProperty(PropertyName = "join")]
        private bool joined = false;

        // Determines if a tank can shoot a beam
        public int PowerUpCount {get; set;}

        // keeps track of how long the tank has been dead
        public int RespawnCounter { get; set; }

        // Main fire cooldown
        public int MainCooldown { get; set; }

        // Flag to tell if a Json was sent with the DC set to true
        public bool DCsent = false;
        // Keeps track of the shots a tank has made
        public int ShotsTaken { get; set; }
        // Keeps track of the shots where a tank hit another tank
        public int ShotsHit { get; set; }

        /// <summary>
        /// Default constructor, does nothing but allow Json.
        /// </summary>
        public Tank()
        {
        }

        /// <summary>
        /// Constructor for the server to make a new tank.
        /// </summary>
        /// <param name="name">Player name</param>
        /// <param name="id">Player ID</param>
        /// <param name="loc">Random location</param>
        public Tank(string name, int id, Vector2D loc)
        {
            Name = name;
            ID = id;
            Orientation = new Vector2D(0, -1);
            Aiming = new Vector2D(0, -1);
            Location = loc;
            PowerUpCount = 0;
            joined = true;
            ShotsTaken = 0;
            ShotsHit = 0;
        }

    }
}
