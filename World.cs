// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace TankWars
{
    /// <summary>
    /// This class represents the model of the world that will be seen by the clients.
    /// It contains all the objects that will be visually seen.
    /// </summary>
    public class World
    {
        // List of all tanks in the world, associated by their ID.
        private Dictionary<int, Tank> tanks;

        // List of all projectiles in the world, associated by their owner.
        private Dictionary<int, Projectile> projectiles;

        // List of all walls in the world, associated by their ID.
        private Dictionary<int, Wall> walls;

        // List of all power ups in the world, associated by their ID.
        private Dictionary<int, PowerUp> powerUps;

        // List of all beams in the world, associated by their owner.
        private Dictionary<int, Beam> beams;

        // List of all the tanks that are waiting to respawn.
        private Dictionary<int, Stopwatch> tanksRespawning;

        // The world size sent by the server.
        public int WorldSize;
        // Unique ID of the player
        public int playerID;


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////          Server used Fields              ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Used to cleanly remove projectiles.
        private HashSet<int> removeProjs;
        // Used to cleanly remove tanks.
        private HashSet<int> removeTanks;
        // Used to cleanly remove power ups.
        private HashSet<int> removePowerUps;
        // Used to cleanly remove beams.
        private HashSet<int> removeBeams;
        // Respawn rate of the tanks
        private int RespawnRate;
        // Fire Rate of the tanks
        private int ShotsperFrame;
        // Counter for the PowerUpcooldown
        private int PCooldownCount = 0;

        /// <summary>
        /// Default constructor for the world. Creates Dictionaries of the tanks, projectiles, walls, powerUps, and beams.
        /// Added for the server are the hashsets used to remove world objects.
        /// </summary>
        public World()
        {
            tanks = new Dictionary<int, Tank>();
            projectiles = new Dictionary<int, Projectile>();
            walls = new Dictionary<int, Wall>();
            powerUps = new Dictionary<int, PowerUp>();
            beams = new Dictionary<int, Beam>();
            tanksRespawning = new Dictionary<int, Stopwatch>();

            removeProjs = new HashSet<int>();
            removeTanks = new HashSet<int>();
            removePowerUps = new HashSet<int>();
            removeBeams = new HashSet<int>();
        }

        /// <summary>
        /// Gets the all the tanks in the world. Returns an empty list if no tanks exist.
        /// </summary>
        /// <returns>list containing all the tanks</returns>
        public IEnumerable<Tank> GetTanks()
        {
            if (tanks.Count == 0)
                return new HashSet<Tank>();
            else
                return tanks.Values;
        }

        /// <summary>
        /// Adds a tank to the world.
        /// </summary>
        /// <param name="t">The tank</param>
        public void AddTank(Tank t)
        {
            tanks[t.ID] = t;
        }

        /// <summary>
        /// Checks if the world contains the players tank.
        /// </summary>
        /// <returns>True if the players tank has been created</returns>
        public bool HasMyTank()
        {
            if (tanks.ContainsKey(playerID))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the tank of the player running the client.
        /// </summary>
        /// <returns>The player tank</returns>
        public Tank GetMyTank()
        {
            return tanks[playerID];
        }

        /// <summary>
        /// If a tank disconnected it needs to be removed by this method.
        /// </summary>
        /// <param name="tank">Tank to remove</param>
        public void TankDCed(Tank tank)
        {
            tanks.Remove(tank.ID);
        }

        /// <summary>
        /// Gets all the projectiles in the world. Returns an empty list if no projectiles exist.
        /// </summary>
        /// <returns>list containing all the projectiles</returns>
        public IEnumerable<Projectile> GetProjectiles()
        {
            if (projectiles.Count == 0)
                return new HashSet<Projectile>();
            else
                return projectiles.Values;
        }

        /// <summary>
        /// Adds a projectile to the world.
        /// </summary>
        /// <param name="proj">Projectile to add</param>
        public void AddProjectile(Projectile proj)
        {
            projectiles[proj.ProjectileID] = proj;
        }

        /// <summary>
        /// Determines if a projectile still exists in the world.
        /// </summary>
        /// <param name="proj">The projectile being checked</param>
        /// <returns>True if the projectile has not hit something</returns>
        public bool ProjectileActive(Projectile proj)
        {
            if (proj.died == true)
            {
                projectiles.Remove(proj.ProjectileID);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Gets all the power ups in the world. Returns an empty list if no power ups exist.
        /// </summary>
        /// <returns>List containing all the power ups</returns>
        public IEnumerable<PowerUp> GetPowerUps()
        {
            if (powerUps.Count == 0)
                return new HashSet<PowerUp>();
            else
                return powerUps.Values;
        }

        /// <summary>
        /// Adds a powerup to the world
        /// </summary>
        /// <param name="pow">Power up</param>
        public void AddPowerUp(PowerUp pow)
        {
            powerUps[pow.PowerID] = pow;
        }

        /// <summary>
        /// Determines whether a power up is still active in the world
        /// </summary>
        /// <param name="power">Powerup</param>
        /// <returns>True if it still exists</returns>
        public bool PowerUpActive(PowerUp power)
        {
            if (power.Died == true)
            {
                powerUps.Remove(power.PowerID);
                return false;
            }
            else
                return true;
        }
        /// <summary>
        /// Gets all the walls in the world. Returns an empty list if walls exist.
        /// </summary>
        /// <returns>List containing all the walls</returns>
        public IEnumerable<Wall> GetWalls()
        {
            if (walls.Count == 0)
                return new HashSet<Wall>();
            else
                return walls.Values;
        }

        /// <summary>
        /// Adds a wall segment to the world
        /// </summary>
        /// <param name="w">Wall</param>
        public void AddWall(Wall w)
        {
            walls[w.wall] = w;
        }

        /// <summary>
        /// Adds a beam to the world
        /// </summary>
        /// <param name="b">Beam</param>
        public void AddBeam(Beam b)
        {
            beams[b.BeamID] = b;
        }

        /// <summary>
        /// Gets any beams in the world
        /// </summary>
        /// <returns>List of existing beams</returns>
        public IEnumerable<Beam> GetBeams()
        {
            if (beams.Count == 0)
                return new HashSet<Beam>();
            else
            {
                return beams.Values;
            }
        }

        /// <summary>
        /// Removes a beam from the world
        /// </summary>
        /// <param name="beam">Beam</param>
        public void RemoveBeam(Beam beam)
        {
            beams.Remove(beam.BeamID);
        }

        /// <summary>
        /// Determines if a tank is still dead by checking if it still exists in the respawning list.
        /// </summary>
        /// <param name="tank">Tank</param>
        /// <returns>True if tank is still dead</returns>
        public bool IsDead(Tank tank)
        {
            if (tanksRespawning.ContainsKey(tank.ID))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Adds a tank to the list of tanks to respawn.
        /// </summary>
        /// <param name="tank">Tank</param>
        public void AddRespawn(Tank tank)
        {
            Stopwatch t = new Stopwatch();
            t.Start();
            tanksRespawning.Add(tank.ID, t);
        }


        /// <summary>
        /// Checks the respawn list of tanks, if tanks have waited 5 seconds then enable them to respawn.
        /// </summary>
        public void CheckRespawns()
        {
            List<int> remove = new List<int>();
            foreach (int t in tanksRespawning.Keys)
            {
                if (tanksRespawning[t].ElapsedMilliseconds >= 5000)
                {
                    tanksRespawning[t].Stop();
                    remove.Add(t);
                }
            }
            foreach (int i in remove)
            {
                tanksRespawning.Remove(i);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////                 NEW SERVER CODE                    /////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// Gets access to the settings for the server. The world only uses the fire rate and respawn rate.
        /// </summary>
        /// <param name="shotRate">Tank fire rate</param>
        /// <param name="respawnRate">Tank respawn rate</param>
        public void SetServerSettings(int shotRate, int respawnRate)
        {
            RespawnRate = respawnRate;
            ShotsperFrame = shotRate;
        }

        /// <summary>
        /// Serializes the world into Json strings.
        /// </summary>
        /// <returns></returns>
        public string SerializeWorld()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tank t in GetTanks())
                sb.Append(JsonConvert.SerializeObject(t) + "\n");
            foreach (Projectile p in GetProjectiles())
                sb.Append(JsonConvert.SerializeObject(p) + "\n");
            foreach (PowerUp po in GetPowerUps())
                sb.Append(JsonConvert.SerializeObject(po) + "\n");
            foreach (Beam b in GetBeams())
                sb.Append(JsonConvert.SerializeObject(b) + "\n");
            return sb.ToString();
        }

        /// <summary>
        /// Creates a spawn point for a tank or power up to spawn at.
        /// Does not allow tanks or power ups to be spawned in a wall
        /// </summary>
        /// <returns>A vector point valid to spawn in</returns>
        public Vector2D SpawnPoint()
        {
            reIterateWalls:
            Random rand = new Random();
            int x = rand.Next(-WorldSize/2, WorldSize / 2);
            int y = rand.Next(-WorldSize / 2, WorldSize / 2);

            Vector2D tempV = new Vector2D(x, y);
            Tank temp = new Tank("temp", -1, tempV);
            foreach (Wall wall in GetWalls())
            {
                if (TankWallCollision(wall, temp))
                {
                    goto reIterateWalls;
                }
            }
            return tempV;
        }

        /// <summary>
        /// Updates the world.
        /// </summary>
        public void UpdateWorld()
        {
            UpdateProjectiles();
            UpdatePowerUps();
            UpdateBeams();
        }

        /// <summary>
        /// This method is called to update a given tank on command with the parsed
        /// parameters sent from a client.
        /// </summary>
        /// <param name="id">Client that sent the command</param>
        /// <param name="moving">Tells direction to move the tank</param>
        /// <param name="fire">Firing command</param>
        /// <param name="direction">Direction of the tank turret</param>
        public void UpdateOnCommand(int id, string moving, string fire, Vector2D direction)
        {
            Tank tank = tanks[id];
            tank.MainCooldown++;
            // This section of code handles the respawning
            if(tank.died)
            {
                tank.died = false;
                tank.RespawnCounter++;
            }
            if (tank.RespawnCounter < RespawnRate && tank.RespawnCounter != 0)
            {
                tank.RespawnCounter++;
                if(tank.RespawnCounter == RespawnRate)
                {
                    tank.RespawnCounter = 0;
                    tank.Location = SpawnPoint();
                    tank.hitPoints = Constants.MaxHP;
                }
            }
            // This section of code handles the commands sent
            else
            {
                tank.Aiming = direction;
                //HashSet<Tank> removeTanks = new HashSet<Tank>();
                if (fire == "main" && tank.MainCooldown > ShotsperFrame) // && framesPassed % fireRate =0
                {
                    tank.MainCooldown = 0;
                    AddProjectile(new Projectile(id, tanks[id].Location, direction));
                    tank.ShotsTaken++;
                }
                else if (fire == "alt")
                {
                    if (tank.PowerUpCount > 0)
                    {
                        AddBeam(new Beam(tank.Location, direction, tank.ID));
                        tank.PowerUpCount--;
                        tank.ShotsTaken++;
                    }
                }
                Vector2D moveVector;
                if (moving == "left")
                {
                    tank.Orientation = new Vector2D(-1, 0);
                    moveVector = new Vector2D(-Constants.Tvelocity, 0);
                }
                else if (moving == "right")
                {
                    tank.Orientation = new Vector2D(1, 0);
                    moveVector = new Vector2D(Constants.Tvelocity, 0);
                }
                else if (moving == "up")
                {
                    tank.Orientation = new Vector2D(0, -1);
                    moveVector = new Vector2D(0, -Constants.Tvelocity);
                }
                else if (moving == "down")
                {
                    tank.Orientation = new Vector2D(0, 1);
                    moveVector = new Vector2D(0, Constants.Tvelocity);
                }
                else
                    moveVector = new Vector2D(0, 0);
                tank.Location += moveVector;
                foreach (Wall wall in GetWalls())
                {
                    if (TankWallCollision(wall, tank))
                    {
                        tank.Location -= moveVector;
                        break;
                    }
                }
                WrapAroundCheck(tank); // If tank goes off the world, teleport it to opposite side
            }
        }

        /// <summary>
        /// If a beam exists then updates the world with the affects of a beam.
        /// </summary>
        public void UpdateBeams()
        {
            foreach(Beam b in GetBeams())
            {
                foreach (Tank t in GetTanks())
                {
                    if (t.hitPoints == 0)
                        continue;
                    if (Intersects(b.Origin, b.Direction, t.Location, Constants.Tank / 2))
                    {
                        t.hitPoints = 0;
                        t.died = true;
                        tanks[b.Owner].score++;
                        tanks[b.Owner].ShotsHit++;
                    }
                }
                removeBeams.Add(b.BeamID);
            }
        }

        /// <summary>
        /// Updates the power ups in the world. The maximum amount of delay for a power up
        /// to spawn is 1650 frames. This method randomly generates a number each frame 
        /// to determine if a power up gets spawned. Maximum count of power ups is 2.
        /// This method also checks if a power up is picked up by a tank.
        /// </summary>
        public void UpdatePowerUps()
        {
            if(powerUps.Count < Constants.MaxPowerUps)
            {
                Random rand = new Random();
                int x = rand.Next(RespawnRate, Constants.PowerUpDelay);
                if (PCooldownCount > x)
                {
                    PowerUp pow = new PowerUp(SpawnPoint());
                    powerUps.Add(pow.PowerID, pow);
                    if (powerUps.Count == Constants.MaxPowerUps)
                        PCooldownCount = 0;
                }
                else
                {
                    PCooldownCount++;
                }
            }
            foreach (PowerUp power in GetPowerUps())
            {
                foreach (Tank tank in GetTanks())
                {
                    if (PowerUpCollision(tank, power))
                    {
                        tank.PowerUpCount++;
                        power.Died = true;
                        removePowerUps.Add(power.PowerID);
                    }
                }
            }
        }

        /// <summary>
        /// This method updates the projectiles in the world. If the projectile goes
        /// out of the world bounds it is destroyed. Otherwise collision checks are
        /// performed for tanks and walls.
        /// </summary>
        public void UpdateProjectiles()
        {
            foreach (Projectile proj in GetProjectiles())
            {
                Vector2D velocity = new Vector2D(proj.Direction);
                velocity *= Constants.Pvelocity;
                proj.Location += velocity;
                if (ProjectileOutOfBounds(proj))
                {
                    proj.died = true;
                    removeProjs.Add(proj.ProjectileID);
                }
                else
                {
                    foreach (Wall wall in GetWalls())
                    {
                        if (ProjectileWallCollision(wall, proj))
                        {
                            proj.died = true;
                            removeProjs.Add(proj.ProjectileID);
                        }
                    }
                    foreach (Tank t in GetTanks())
                    {
                        if (ProjectileTankCollision(t, proj))
                        {
                            if (!(t.ID == proj.Owner))
                            {
                                tanks[proj.Owner].ShotsHit++;
                                proj.died = true;
                                t.hitPoints--;
                                removeProjs.Add(proj.ProjectileID);
                                if (t.hitPoints == 0)
                                {
                                    tanks[proj.Owner].score++;
                                    t.died = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This method checks for a collision between a tank and a wall by
        /// representing the wall as a vector rectangle and extending the wall by a
        /// a tank radius.
        /// </summary>
        /// <param name="wall">Wall segment</param>
        /// <param name="tank">The tank</param>
        /// <returns>True if there is a collision</returns>
        private bool TankWallCollision(Wall wall, Tank tank)
        {
            // Two range checks, between lowerX and upperX && between lowerY and upperY
            double lowerX;
            double upperX;
            double lowerY;
            double upperY;

            // Wall segment is Vertical
            if (wall.point1.GetX() == wall.point2.GetX())
            {
                lowerX = wall.point1.GetX() - (Constants.Tank/2 + Constants.Wall/2);
                upperX = wall.point1.GetX() + (Constants.Tank / 2 + Constants.Wall / 2);
                if (wall.point1.GetY() < wall.point2.GetY())
                {
                    lowerY = wall.point1.GetY() - (Constants.Tank / 2 + Constants.Wall / 2);
                    upperY = wall.point2.GetY() + (Constants.Tank / 2 + Constants.Wall / 2);
                }
                else
                {
                    lowerY = wall.point2.GetY() - (Constants.Tank / 2 + Constants.Wall / 2);
                    upperY = wall.point1.GetY() + (Constants.Tank / 2 + Constants.Wall / 2);
                }
            }
            else // Wall segment is horizontal
            {
                lowerY = wall.point1.GetY() - (Constants.Tank / 2 + Constants.Wall / 2);
                upperY = wall.point1.GetY() + (Constants.Tank / 2 + Constants.Wall / 2);
                if (wall.point1.GetX() < wall.point2.GetX())
                {
                    upperX = wall.point2.GetX() + (Constants.Tank / 2 + Constants.Wall / 2);
                    lowerX = wall.point1.GetX() - (Constants.Tank / 2 + Constants.Wall / 2);
                }
                else
                {
                    upperX = wall.point1.GetX() + (Constants.Tank / 2 + Constants.Wall / 2);
                    lowerX = wall.point2.GetX() - (Constants.Tank / 2 + Constants.Wall / 2);
                }
            }
            return tank.Location.GetX() <= upperX
                && tank.Location.GetX() >= lowerX
                && tank.Location.GetY() <= upperY
                && tank.Location.GetY() >= lowerY;
        }

        /// <summary>
        /// Handles the world boundary wrap around
        /// </summary>
        /// <param name="tank">The tank</param>
        private void WrapAroundCheck(Tank tank)
        {
            if(tank.Location.GetX() > WorldSize / 2 || tank.Location.GetX() < -WorldSize / 2)
                tank.Location = new Vector2D(-tank.Location.GetX(), tank.Location.GetY());
            else if (tank.Location.GetY() > WorldSize / 2 || tank.Location.GetY() < -WorldSize / 2)
                tank.Location = new Vector2D(tank.Location.GetX(), -tank.Location.GetY());
        }

        /// <summary>
        /// Checks for a collision between a wall and projectile. Projectile is destroyed
        /// if it collides with a wall
        /// </summary>
        /// <param name="wall">Wall segment</param>
        /// <param name="proj">The Projectile</param>
        /// <returns>True if there is a collision</returns>
        private bool ProjectileWallCollision(Wall wall, Projectile proj)
        {
            // Two range checks, between lowerX and upperX && between lowerY and upperY
            double lowerX;
            double upperX;
            double lowerY;
            double upperY;

            // Wall segment is Vertical
            if (wall.point1.GetX() == wall.point2.GetX())
            {
                lowerX = wall.point1.GetX() - Constants.Wall / 2;
                upperX = wall.point1.GetX() + Constants.Wall / 2;
                if (wall.point1.GetY() < wall.point2.GetY())
                {
                    lowerY = wall.point1.GetY() - Constants.Wall / 2;
                    upperY = wall.point2.GetY() + Constants.Wall / 2;
                }
                else
                {
                    lowerY = wall.point2.GetY() - Constants.Wall / 2;
                    upperY = wall.point1.GetY() + Constants.Wall / 2;
                }
            }
            else // Wall segment is horizontal
            {
                lowerY = wall.point1.GetY() - Constants.Wall / 2;
                upperY = wall.point1.GetY() + Constants.Wall / 2;
                if (wall.point1.GetX() < wall.point2.GetX())
                {
                    upperX = wall.point2.GetX() + Constants.Wall/2;
                    lowerX = wall.point1.GetX() - Constants.Wall / 2;
                }
                else
                {
                    upperX = wall.point1.GetX() + Constants.Wall / 2;
                    lowerX = wall.point2.GetX() - Constants.Wall / 2;
                }
            }
            return proj.Location.GetX() <= upperX
                && proj.Location.GetX() >= lowerX
                && proj.Location.GetY() <= upperY
                && proj.Location.GetY() >= lowerY;
        }

        /// <summary>
        /// Checks for a collision between a tank and a projectile
        /// </summary>
        /// <param name="t">The tank</param>
        /// <param name="proj">The projectile</param>
        /// <returns>True if there is a collision</returns>
        private bool ProjectileTankCollision(Tank t, Projectile proj)
        {
            if (t.hitPoints == 0)
                return false;
            Vector2D vec = proj.Location - t.Location;
            double radius = Constants.Tank / 2;
            if (vec.Length() < radius)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a projectile went out of the world bounds.
        /// </summary>
        /// <param name="proj">The projectile</param>
        /// <returns>True if out of the world</returns>
        private bool ProjectileOutOfBounds(Projectile proj)
        {
            if (proj.Location.GetX() > WorldSize / 2 || proj.Location.GetX() < -WorldSize / 2)
                return true;
            else if (proj.Location.GetY() > WorldSize / 2 || proj.Location.GetY() < -WorldSize / 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a tank should pick up a power up.
        /// </summary>
        /// <param name="t">Any tank</param>
        /// <param name="power">The power up</param>
        /// <returns>True if a tank is over a power up</returns>
        private bool PowerUpCollision(Tank t, PowerUp power)
        {
            Vector2D vec = t.Location - power.Location;
            double radius = Constants.Tank / 2;
            if (vec.Length() < radius)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substitute to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

        /// <summary>
        /// Tank is set to be disconnected by the controller if the client socket is disconnected
        /// </summary>
        /// <param name="id">ID of the disconnected</param>
        public void SetTankDC(int id)
        {
            lock (tanks)
            {
                tanks[id].hitPoints = 0;
                tanks[id].died = true;
                tanks[id].disconnected = true;
            }
        }

        /// <summary>
        /// Properly cleans up the world by removing objects that no longer should exist. 
        /// Wait to remove a tank if it's DC is set to true so that the proper Json is sent to the clients.
        /// </summary>
        public void CleanUpWorld()
        {
            foreach (int i in removeProjs)
                projectiles.Remove(i);
            removeProjs.Clear();
            foreach (int i in removeTanks)
                tanks.Remove(i);
            removeTanks.Clear();
            foreach (Tank t in GetTanks())
            {
                if (t.disconnected)
                    removeTanks.Add(t.ID);
            }
            foreach (int i in removePowerUps)
                powerUps.Remove(i);
            removePowerUps.Clear();
            foreach (int i in removeBeams)
                beams.Remove(i);
            removeBeams.Clear();
        }
    }
}
