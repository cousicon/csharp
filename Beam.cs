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
    /// This class is a model representation of a beam. A beam can go through walls and destroy a tank instantly.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        // The ID of the beam.
        [JsonProperty(PropertyName = "beam")]
        public int BeamID { get; set; }

        // Origin of the beam
        [JsonProperty(PropertyName = "org")]
        public Vector2D Origin { get; private set; }

        // Direction of the beam
        [JsonProperty(PropertyName = "dir")]
        public Vector2D Direction { get; private set; }

        // ID of the tank that fired the beam. Can be used to draw different color beams depending on the player.
        [JsonProperty(PropertyName = "owner")]
        public int Owner { get; private set; }

        // Auto increment for the BeamID
        private static int id = 0;

        /// <summary>
        /// Default constructor needed for Json
        /// </summary>
        public Beam()
        {
        }

        /// <summary>
        /// Initializes a beam for the server
        /// </summary>
        /// <param name="orig">Origin of the beam</param>
        /// <param name="dir">Direction</param>
        /// <param name="own">Player owner of the beam</param>
        public Beam(Vector2D orig, Vector2D dir, int own)
        {
            BeamID = id++;
            Origin = orig;
            Direction = dir;
            Owner = own;

        }
    }
}
