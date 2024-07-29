// Author: Connor Cousineau and Spencer Durrant
// Tank Wars solution Fall 2019

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TankWars
{
    /// <summary>
    /// This class is the drawing panel that creates the view of the world.
    /// </summary>
    public class DrawingPanel : Panel
    {
        // Field to represent the world being drawn on.
        private World theWorld;
        // Flag to determine if the form is being started, the world shouldn't be drawn if true.
        private bool formStartup = true;
        // Frame count for the beam.
        private int frameCount = 0;

        // Initialize all images once to improve speed of the program.
        // Tank images
        private Image bluetankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\BlueTank.png");
        private Image darktankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\DarkTank.png");
        private Image greentankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\GreenTank.png");
        private Image lightgreentankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\LightGreenTank.png");
        private Image orangetankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\OrangeTank.png");
        private Image purpletankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\PurpleTank.png");
        private Image redtankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\RedTank.png");
        private Image yellowtankImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\YellowTank.png");
        // Tank died image
        private Image ripImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\RIP.png");
        // Turret images
        private Image blueturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\BlueTurret.png");
        private Image darkturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\DarkTurret.png");
        private Image greenturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\GreenTurret.png");
        private Image lightgreenturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\LightGreenTurret.png");
        private Image orangeturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\OrangeTurret.png");
        private Image purpleturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\PurpleTurret.png");
        private Image redturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\RedTurret.png");
        private Image yellowturret = Image.FromFile("..\\..\\..\\Resources\\Graphics\\YellowTurret.png");
        // Projectile images
        private Image blueshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_blue.png");
        private Image violetshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_violet.png");
        private Image greenshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_green.png");
        private Image whiteshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_white.png");
        private Image brownshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_brown.png");
        private Image greyshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_grey.png");
        private Image redshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_red.png");
        private Image yellowshot = Image.FromFile("..\\..\\..\\Resources\\Graphics\\shot_yellow.png");
        // Wall image
        private Image wallImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\WallSprite.png");
        // World image
        private Image worldImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\Background.png");
        // PowerUp image
        private Image powerImage = Image.FromFile("..\\..\\..\\Resources\\Graphics\\GAup2.png");

        /// <summary>
        /// Constructor for the drawing panel. DoubleBuffered set to true
        /// </summary>
        /// <param name="w">the world to be updated and drawn on</param>
        public DrawingPanel(World w)
        {
            theWorld = w;
            DoubleBuffered = true;
        }

        /// <summary>
        /// Helper method use by DrawObjectWithTransform. Determines the wanted image space to draw
        /// </summary>
        /// <param name="size">word size</param>
        /// <param name="w">coordinate of object int the world space</param>
        /// <returns></returns>
        public static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        /// <summary>
        /// Signature of the delegate to do the actual drawing.
        /// </summary>
        /// <param name="o">object to draw</param>
        /// <param name="e">access to graphics for drawing</param>
        public delegate void ObjectDrawer(object o, PaintEventArgs e);

        /// <summary>
        /// This method performs a translation and rotation to draw an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the object, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double playerX, double playerY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, playerX);
            int y = WorldSpaceToImageSpace(worldSize, playerY);
            if (!(o is Wall))
                e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);

            drawer(o, e);
            if (o is Wall)
                e.Graphics.TranslateTransform(x, y);
            // "pop" the transform
            e.Graphics.Transform = oldMatrix;

        }

        /// <summary>
        /// This is the method being called every frame to update the world with the drawings of all the objects.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (formStartup)
            {
                // For the initial form startup, don't draw the world yet
                formStartup = false;
                return;
            }
            // The following loops will redraw the associated objects in every frame.
            else
            {

                lock (theWorld)
                {
                    if (!theWorld.HasMyTank())
                        return;
                    Tank playerTank = theWorld.GetMyTank();
                    Transform(playerTank.Location, e);

                    //DrawObjectWithTransform(e, playerTank, theWorld.WorldSize, playerTank.location.GetX(), playerTank.location.GetY(), playerTank.orientation.ToAngle(), TankDrawer);

                    e.Graphics.DrawImage(worldImage, 0, 0, theWorld.WorldSize, theWorld.WorldSize);
                    //e.Graphics.DrawImage(worldImage, theWorld.WorldSize /2, theWorld.WorldSize /2, theWorld.WorldSize, theWorld.WorldSize);

                    foreach (Wall wall in theWorld.GetWalls())
                    {
                        DrawObjectWithTransform(e, wall, theWorld.WorldSize, wall.point1.GetX(), wall.point1.GetY(), 0, WallDrawer);
                    }
                    foreach (Tank tank in theWorld.GetTanks())
                    {
                        if (theWorld.IsDead(tank))
                            DrawObjectWithTransform(e, tank, theWorld.WorldSize, tank.Location.GetX(), tank.Location.GetY(), 0, DeathDrawer);
                        else
                        {
                            DrawObjectWithTransform(e, tank, theWorld.WorldSize, tank.Location.GetX(), tank.Location.GetY(), 0, StringDrawer);
                            DrawObjectWithTransform(e, tank, theWorld.WorldSize, tank.Location.GetX(), tank.Location.GetY(), 0, HPDrawer);
                            DrawObjectWithTransform(e, tank, theWorld.WorldSize, tank.Location.GetX(), tank.Location.GetY(), tank.Orientation.ToAngle(), TankDrawer);
                            DrawObjectWithTransform(e, tank, theWorld.WorldSize, tank.Location.GetX(), tank.Location.GetY(), tank.Aiming.ToAngle(), TurretDrawer);
                        }
                    }
                    theWorld.CheckRespawns();
                    foreach (Projectile proj in theWorld.GetProjectiles())
                    {
                        DrawObjectWithTransform(e, proj, theWorld.WorldSize, proj.Location.GetX(), proj.Location.GetY(), proj.Direction.ToAngle(), ProjectileDrawer);
                    }
                    foreach (PowerUp powerUp in theWorld.GetPowerUps())
                    {
                        DrawObjectWithTransform(e, powerUp, theWorld.WorldSize, powerUp.Location.GetX(), powerUp.Location.GetY(), 0, PowerUpDrawer);
                    }

                    Beam remove = null;
                    foreach (Beam beam in theWorld.GetBeams())
                    {
                        DrawObjectWithTransform(e, beam, theWorld.WorldSize, beam.Origin.GetX(), beam.Origin.GetY(), beam.Direction.ToAngle() + 180, BeamDrawer);
                        remove = beam;
                    }
                    if (frameCount >= 20)
                    {
                        theWorld.RemoveBeam(remove);
                        frameCount = 0;
                    }
                }
                base.OnPaint(e);
            }
        }



        /// <summary>
        /// Helper method to translate transform the location of the object to draw
        /// </summary>
        /// <param name="location">location of object</param>
        /// <param name="e">Access to graphics</param>
        /// <param name="inverseTranslateX">translated x</param>
        /// <param name="inverseTranslateY">translated y</param>
        private void Transform(Vector2D location, PaintEventArgs e)
        {
            double playerX = location.GetX();
            double playerY = location.GetY();

            double ratio = (double)Constants.ViewSize / (double)theWorld.WorldSize;
            int halfSizeScaled = (int)(theWorld.WorldSize / 2.0 * ratio);

            double inverseTranslateX = -WorldSpaceToImageSpace(theWorld.WorldSize, playerX) + halfSizeScaled;
            double inverseTranslateY = -WorldSpaceToImageSpace(theWorld.WorldSize, playerY) + halfSizeScaled;

            e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);
        }

        /// <summary>
        /// This method draws the walls. It will seperate the walls by 50 pixels each.
        /// The logic is to determine which direction the client needs to draw the walls.
        /// If the walls are the same vertically then the direction to draw is vertically.
        /// </summary>
        /// <param name="o">The wall to draw</param>
        /// <param name="e">Access to graphics</param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Wall wall = o as Wall;
            // Wall segment is horizontal
            if (wall.point1.GetX() == wall.point2.GetX())
            {
                double decrementY;
                double endingPoint;
                Vector2D thePoint;
                if (wall.point1.GetY() < wall.point2.GetY())
                {
                    thePoint = wall.point2;
                    endingPoint = wall.point1.GetY();
                    decrementY = wall.point2.GetY();
                }
                else
                {
                    thePoint = wall.point1;
                    endingPoint = wall.point2.GetY();
                    decrementY = wall.point1.GetY();
                }
                while (decrementY >= endingPoint)
                {
                    PointF point = new PointF((float)wall.point1.GetX(), (float)decrementY);
                    e.Graphics.DrawImage(wallImage, point.X + theWorld.WorldSize / 2 - Constants.Wall/2, point.Y + theWorld.WorldSize / 2 - Constants.Wall/2, Constants.Wall, Constants.Wall);
                    decrementY -= Constants.Wall;
                }
            }
            else // Wall segment is vertical
            {
                double decrementX;
                double endingPoint;
                Vector2D thePoint;
                if (wall.point1.GetX() < wall.point2.GetX())
                {
                    thePoint = wall.point2;
                    endingPoint = wall.point1.GetX();
                    decrementX = wall.point2.GetX();
                }
                else
                {
                    thePoint = wall.point1;
                    endingPoint = wall.point2.GetX();
                    decrementX = wall.point1.GetX();
                }
                while (decrementX >= endingPoint)
                {
                    PointF point = new PointF((float)decrementX, (float)wall.point1.GetY());
                    e.Graphics.DrawImage(wallImage, point.X + theWorld.WorldSize / 2 - Constants.Wall / 2, point.Y + theWorld.WorldSize / 2 - Constants.Wall / 2, Constants.Wall, Constants.Wall);
                    decrementX -= Constants.Wall;
                }
            }
        }

        /// <summary>
        /// Invoked by the OnPaint method. Does the tank drawing. The color is determined by the tank ID being modded with the number
        /// of different tank sprites in the graphics folder.
        /// </summary>
        /// <param name="o">the tank being drawn</param>
        /// <param name="e">access to graphics</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            Image tankImage;
            int mod = t.ID % Constants.TankPNGs;
            switch (mod)
            {
                case 0:
                    tankImage = bluetankImage;
                    break;
                case 1:
                    tankImage = darktankImage;
                    break;
                case 2:
                    tankImage = greentankImage;
                    break;
                case 3:
                    tankImage = lightgreentankImage;
                    break;
                case 4:
                    tankImage = orangetankImage;
                    break;
                case 5:
                    tankImage = purpletankImage;
                    break;
                case 6:
                    tankImage = redtankImage;
                    break;
                default:
                    tankImage = yellowtankImage;
                    break;
            }

            e.Graphics.DrawImage(tankImage, -Constants.Tank/2, -Constants.Tank/2, Constants.Tank, Constants.Tank);
        }

        /// <summary>
        /// Invoked by the OnPaint method. Does the turret drawing exactly on top of the tank. The color is determined by the tank ID being modded with the number
        /// of different tank turret sprites in the graphics folder.
        /// </summary>
        /// <param name="o">the tank to draw a turret on</param>
        /// <param name="e">access to graphics</param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            Image turret;
            int mod = t.ID % Constants.TankPNGs;
            switch (mod)
            {
                case 0:
                    turret = blueturret;
                    break;
                case 1:
                    turret = darkturret;
                    break;
                case 2:
                    turret = greenturret;
                    break;
                case 3:
                    turret = lightgreenturret;
                    break;
                case 4:
                    turret = orangeturret;
                    break;
                case 5:
                    turret = purpleturret;
                    break;
                case 6:
                    turret = redturret;
                    break;
                default:
                    turret = yellowturret;
                    break;
            }
            e.Graphics.DrawImage(turret, -Constants.Turret/2, -Constants.Turret/2, Constants.Turret, Constants.Turret);
        }

        /// <summary>
        /// Invoked by the OnPaint method. Does the projectile drawing. The color is determined by the tank ID being modded with the number
        /// of different tank PNGs which has always the same number of shot PNGs in the graphics folder.
        /// </summary>
        /// <param name="o">the projectile being drawn</param>
        /// <param name="e">access to graphics</param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectile proj = o as Projectile;
            Image shot;
            int mod = proj.Owner % Constants.TankPNGs;
            switch (mod)
            {
                case 0:
                    shot = blueshot;
                    break;
                case 1:
                    shot = violetshot;
                    break;
                case 2:
                    shot = greenshot;
                    break;
                case 3:
                    shot = whiteshot;
                    break;
                case 4:
                    shot = brownshot;
                    break;
                case 5:
                    shot = greyshot;
                    break;
                case 6:
                    shot = redshot;
                    break;
                default:
                    shot = yellowshot;
                    break;
            }
            e.Graphics.DrawImage(shot, -Constants.Proj/2, -Constants.Proj/2, Constants.Proj, Constants.Proj);
        }

        /// <summary>
        /// Invoked by the OnPaint method. Does the power up drawing which will always be the same.
        /// </summary>
        /// <param name="o">the power up being drawn</param>
        /// <param name="e">access to graphics</param>
        private void PowerUpDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(powerImage, -Constants.Powerup/2, -Constants.Powerup/2, Constants.Powerup, Constants.Powerup);
        }

        /// <summary>
        /// Draws the beam when fired for a short time.
        /// </summary>
        /// <param name="o">Beam</param>
        /// <param name="e">Access to graphics</param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            frameCount++;
            using (SolidBrush beamBrush = new SolidBrush(Color.White))
                e.Graphics.FillRectangle(beamBrush, 0, 0, 2f, theWorld.WorldSize);
        }

        /// <summary>
        /// Draws the string containing the player name and score above the tank.
        /// </summary>
        /// <param name="o">The tank</param>
        /// <param name="e">Access to graphics</param>
        private void StringDrawer(object o, PaintEventArgs e)
        {
            Tank t = o as Tank;
            using (SolidBrush textBrush = new SolidBrush(Color.White))
                e.Graphics.DrawString(t.Name + " Score: " + t.score, DefaultFont, textBrush, -40, -45);
        }

        /// <summary>
        /// Draws the death animation of the tank.
        /// </summary>
        /// <param name="o">Tank</param>
        /// <param name="e">Access to graphics</param>
        private void DeathDrawer(object o, PaintEventArgs e)
        {
            e.Graphics.DrawImage(ripImage, -Constants.Tank/2, -Constants.Tank/2, Constants.Tank, Constants.Tank);
        }

        /// <summary>
        /// Draws the hitpoint bar below the tank. Green is full hp, yellow is 2 hp, red is 1 hp.
        /// </summary>
        /// <param name="o">Tank</param>
        /// <param name="e">Access to graphics</param>
        private void HPDrawer(object o, PaintEventArgs e)
        {
            Tank tank = o as Tank;

            if (tank.hitPoints == 3)
            {
                using (SolidBrush barBrush = new SolidBrush(Color.Green))
                    e.Graphics.FillRectangle(barBrush, -30, 32, 60, 4);
            }
            else if (tank.hitPoints == 2)
            {
                using (SolidBrush barBrush = new SolidBrush(Color.Gold))
                    e.Graphics.FillRectangle(barBrush, -30, 32, 40, 4);
            }
            else if (tank.hitPoints == 1)
            {
                using (SolidBrush barBrush = new SolidBrush(Color.DarkRed))
                    e.Graphics.FillRectangle(barBrush, -30, 32, 20, 4);
            }
            else
                return;
        }
    }
}
