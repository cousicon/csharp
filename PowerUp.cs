// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using Newtonsoft.Json;
using System;

namespace TankWars
{
    /// <summary>
    /// This class is a model representation of the power ups in the world.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PowerUp
    {
        // ID of the power up
        [JsonProperty(PropertyName = "power")]
        public int PowerID { get; set; }

        // location of the power up
        [JsonProperty(PropertyName = "loc")]
        public Vector2D Location { get; private set; }

        // determines whether the power up has died
        [JsonProperty(PropertyName = "died")]
        public bool Died { get; set; }

        // auto increment for the PowerUp id
        private static int id = 0;
        /// <summary>
        /// Constructor needed for Json
        /// </summary>
        public PowerUp()
        {
        }

        /// <summary>
        /// Initializes a powerup in a determined random location for the server
        /// </summary>
        /// <param name="loc">Location</param>
        public PowerUp(Vector2D loc)
        {
            PowerID = id++;
            Location = loc;
            Died = false;
        }
    }
}
