using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeistGame
{
    class LightMap
    {
        public Light[] StrongLights { get; private set; }
        public Light[] WeakLights { get; private set; }
        public Dictionary<Vector2, int> FloorTilesValues { get; private set; }
    }


    class Light
    {
        private int radius;
        private Vector2[] areaWithLight;

        public Vector2 Position { get; private set; }
        public Dictionary<Vector2, int> IlluminatedTiles { get; protected set; }

        public Light(int x, int y, int radius)
        {
            Position = new Vector2(x, y);
            this.radius = radius;
            areaWithLight = Rasterizer.GetCellsAlongCircle(x, y, radius);
            IlluminatedTiles = new Dictionary<Vector2, int>();
        }

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

        public void CalculateIlluminatedTiles(Level level)
        {
            List<Vector2> actualTiles = new List<Vector2>();

            foreach (Vector2 point in areaWithLight)
            {
                Vector2[] illuminatedTiles = Rasterizer.GetCellsAlongLine(Position.X, Position.Y, point.X, point.Y);
                foreach(Vector2 tile in illuminatedTiles)
                {
                    if (level.IsTileTransparent(tile.X, tile.Y))
                    {
                        actualTiles.Add(tile);
                    }
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
