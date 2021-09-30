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

        public Dictionary<Vector2, int> FloorTilesValues { get; private set; }
    }


    class Light
    {
        private int radius;
        private Vector2[] areaWithLight;

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
            areaWithLight = Rasterizer.GetCellsAlongCircumference(x, y, radius);
            IlluminatedTiles = new Dictionary<Vector2, int>();
        }

        /// <summary>
        /// Just a test method to check if the tile assignment works.
        /// </summary>
        public void TestIlluminatedTiles()
        {
            List<Vector2> actualTiles = new List<Vector2>();

            foreach (Vector2 point in areaWithLight)
            {
                Vector2[] illuminatedTiles = Rasterizer.GetCellsAlongLine(Position.X, Position.Y, point.X, point.Y);
                foreach (Vector2 tile in illuminatedTiles)
                {
                    actualTiles.Add(tile);
                }
            }

            foreach (Vector2 tile in actualTiles)
            {
                int absoluteX = Math.Abs(tile.X - Position.X);
                int absoluteY = Math.Abs(tile.Y - Position.Y);
                int threshold = radius / 3;

                if (absoluteX <= threshold && absoluteY <= threshold)
                {
                    IlluminatedTiles[tile] = 3;
                }
                else if ((absoluteX > threshold && absoluteX <= threshold * 2) && (absoluteY > threshold && absoluteY <= threshold * 2))
                {
                    IlluminatedTiles[tile] = 2;
                }
                else
                {
                    IlluminatedTiles[tile] = 1;
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

            foreach (Vector2 point in areaWithLight)
            {
                Vector2[] illuminatedTiles = Rasterizer.GetCellsAlongLine(Position.X, Position.Y, point.X, point.Y);

                foreach(Vector2 tile in illuminatedTiles)
                {
                    if (!level.IsTileTransparent(tile.X, tile.Y))
                    {
                        break;
                    }
                    actualTiles.Add(tile);
                }
            }

            foreach (Vector2 tile in actualTiles)
            {
                int absoluteX = Math.Abs(tile.X - Position.X);
                int absoluteY = Math.Abs(tile.Y - Position.Y);
                int threshold = radius / 3;

                if (absoluteX <= threshold || absoluteY <= threshold)
                {
                    IlluminatedTiles[tile] = 3;
                }
                else if ((absoluteX > threshold && absoluteX <= threshold * 2) || (absoluteY > threshold && absoluteY <= threshold * 2))
                {
                    IlluminatedTiles[tile] = 2;
                }
                else
                {
                    IlluminatedTiles[tile] = 1;
                }
            }
        }
    }
}
