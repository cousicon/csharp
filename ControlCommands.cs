// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using Newtonsoft.Json;
using System;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommands
    {
        // This string determines whether the player wants to move or not, and the desired direction.
        // possible values are: none, up, left, down, right.
        [JsonProperty(PropertyName = "moving")]
        public string Moving = "none";

        // This string tells whether the player wants to fire or not and the desired type.
        // Possible values are: none, main, or alt
        [JsonProperty(PropertyName = "fire")]
        public string Fire = "none";

        // Represents where the player wants to aim their turret. This vector must be normalized.
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D TankDirection = new Vector2D(0, 1);

        public ControlCommands()
        {
        }
    }
}
