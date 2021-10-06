using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame
{
    class LightMap
    {
        /// <summary>
        /// The collection of strong (i.e. larger radius) lights.
        /// </summary>
        public Light[] StrongLights { get; private set; }
        /// <summary>
        /// The collection of weak (i.e. smaller radius) lights.
        /// </summary>
        public Light[] WeakLights { get; private set; }

        public HashSet<Vector2> FloorTiles { get; private set; }

        public Dictionary<Vector2, int> FloorTilesValues { get; private set; }
    }


    class Light
    {
        private int radius;

        /// <summary>
        /// The coordinates of the light.
        /// </summary>
        public Vector2 Position { get; private set; }
        /// <summary>
        /// The collection of tiles illuminated by this light.
        /// </summary>
        public Dictionary<Vector2, int> IlluminatedTiles { get; protected set; }

        public Light(int x, int y, int radius)
        {
            Position = new Vector2(x, y);
            this.radius = radius;
            IlluminatedTiles = new Dictionary<Vector2, int>();
        }

        /// <summary>
        /// Just a test method to check if the tile assignment works.
        /// </summary>
        public void TestIlluminatedTiles1()
        {
            for (int i = 0; i <= radius; i++)
            {
                Vector2[] lightCircle = Rasterizer.GetCellsAlongEllipse(Position.X, Position.Y, i*2, i);

                int lightValue = 0;
                int threshold = radius / 3;
                if (i <= threshold) { lightValue = 3; }
                else if (i > threshold && i <= threshold * 2) { lightValue = 2; }
                else { lightValue = 1; }

                foreach (Vector2 tile in lightCircle)
                {
                    IlluminatedTiles[tile] = lightValue;
                }
            }
        }

        public void TestIlluminatedTiles2()
        {
            int threshold = radius / 3;

            for (int i = 3; i > 0 ; i--)
            {
                int thresholdRadius = threshold * i + 2;
                Vector2[] lightCircumference = Rasterizer.GetCellsAlongEllipse(Position.X, Position.Y, thresholdRadius * 2, thresholdRadius);

                foreach (Vector2 point in lightCircumference)
                {
                    Vector2[] points = Rasterizer.GetCellsAlongLine(Position.X, Position.Y, point.X, point.Y);

                    foreach (Vector2 IlluminatedPoint in points)
                    {
                        IlluminatedTiles[IlluminatedPoint] = i;
                    }
                }
            }
        }

        /// <summary>
        /// Checks which tiles are illuminated (depending on whether they can be reached by the light), and how much
        /// </summary>
        /// <param name="level"></param>
        public void CalculateIlluminatedTiles(Level level)
        {
            List<Vector2> actualTiles = new List<Vector2>();
        }
    }
}
