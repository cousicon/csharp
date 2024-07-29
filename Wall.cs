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
    /// This class represents the wall object. Tanks and regular projectiles can not pass through but a beam can.
    /// Walls will only be sent from the server once to the client.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        // Unique ID of the wall
        [JsonProperty(PropertyName = "wall")]
        public int wall { get; set; }

        // One endpoint of the wall
        [JsonProperty(PropertyName = "p1")]
        public Vector2D point1 { get; private set; }

        // The other endpoint of the wall
        [JsonProperty(PropertyName = "p2")]
        public Vector2D point2 { get; private set; }

        /// <summary>
        /// Default constructor for Json
        /// </summary>
        public Wall()
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////                            SERVER CODE                                    //////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Constructor for the server to initialize a wall
        /// </summary>
        /// <param name="id">wall ID</param>
        /// <param name="p1">One end of the wall</param>
        /// <param name="p2">Other end of the wall</param>
        public Wall(int id, Vector2D p1, Vector2D p2)
        {
            wall = id;
            point1 = p1;
            point2 = p2;
        }
    }
}
