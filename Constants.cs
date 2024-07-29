// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    /// <summary>
    /// This class holds the constants for the Tank Wars game.
    /// </summary>
    public static class Constants
    {
        // Max HP of the tanks
        public const int MaxHP = 3;
        // Number of different tank images/colors
        public const int TankPNGs = 8;
        // Default view size
        public const int ViewSize = 800;
        // Size of walls
        public const int Wall = 50;
        // Size of tank
        public const int Tank = 60;
        // Size of turret
        public const int Turret = 50;
        // Size of projectile
        public const int Proj = 30;
        // Size of power ups
        public const int Powerup = 40;

        // Server used only constants

        // Speed of tank
        public const double Tvelocity = 2.9;
        // Speed of projectiels
        public const double Pvelocity = 25;
        // Max power up count
        public const int MaxPowerUps = 2;
        // Maximum delay for Power up
        public const int PowerUpDelay = 1650;
    }
}
