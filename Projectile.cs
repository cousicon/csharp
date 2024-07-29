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
    /// This class is the model representation of a projectile shot by a tank.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        // Represents the projectile's unique ID
        [JsonProperty(PropertyName = "proj")]
        public int ProjectileID { get; set; }

        // Represents the projectile's location
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; set; }

        // Represents the projectile's orientation
        [JsonProperty(PropertyName = "dir")]
        public Vector2D Direction { get; private set; }

        // Flag on whether the the projectile hit a wall or left the bounds of the world. Server sends the dead projectiles only once
        [JsonProperty(PropertyName = "died")]
        public bool died = false;

        // ID of the tank that created the projectile. Can use this to draw projectiles differently by tank
        [JsonProperty(PropertyName = "owner")]
        public int Owner { get; private set; }

        // Auto increment for the ProjectileID
        private static int id = 0;

        
        /// <summary>
        /// Constructor for the Json
        /// </summary>
        public Projectile()
        {
        }

        /// <summary>
        /// Initializes a new projectile for the server.
        /// </summary>
        /// <param name="ownerID">Player ID who made the projectile</param>
        /// <param name="loc">Initial location of the projectile</param>
        /// <param name="dir">Direction of the projectile</param>
        public Projectile(int ownerID, Vector2D loc, Vector2D dir)
        {
            ProjectileID = id++;
            Owner = ownerID;
            Location = loc;
            Direction = dir;
        }
    }
}
