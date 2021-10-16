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

        public LightMap(Light[] strongLights, Light[] weakLights)
        {
            StrongLights = strongLights;
            WeakLights = weakLights;

            FloorTilesValues = new Dictionary<Vector2, int>();
        }

        /// <summary>
        /// Calculates the light values for each walkable floor on the map
        /// </summary>
        /// <param name="level">The level the lightmap belongs to</param>
        public void CalculateLightMap(Level level)
        {
            FloorTilesValues.Clear();

            foreach (Vector2 tile in level.FloorTiles)
            {
                FloorTilesValues.Add(tile, 0);
            }

            foreach (Light strongLight in StrongLights)
            {
                strongLight.CalculateLightAreas(level);

                foreach (KeyValuePair<Vector2, int> tile in strongLight.IlluminatedTiles)
                {
                    if (!FloorTilesValues.ContainsKey(tile.Key))
                    {
                        throw new Exception($"Error! Invalid lightmap tile: {tile.Key} in {level.Name}");
                    }

                    if (FloorTilesValues[tile.Key] < tile.Value)
                    {
                        FloorTilesValues[tile.Key] = tile.Value;
                    }
                }
            }

            foreach (Light weakLight in WeakLights)
            {
                weakLight.CalculateLightAreas(level);

                foreach (KeyValuePair<Vector2, int> tile in weakLight.IlluminatedTiles)
                {
                    if (!FloorTilesValues.ContainsKey(tile.Key))
                    {
                        throw new Exception($"Error! Invalid lightmap tile: {tile.Key} in {level.Name}");
                    }

                    if (FloorTilesValues[tile.Key] < tile.Value)
                    {
                        FloorTilesValues[tile.Key] = tile.Value;
                    }
                }
            }
        }
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
        /// Checks which tiles are illuminated (depending on whether they can be reached by the light), and how much (depending on distance from the source)
        /// </summary>
        /// <param name="level"></param>
        public void CalculateLightAreas(Level Level)
        {
            IlluminatedTiles.Clear();

            int threshold = radius / 3;

            for (int i = 3; i > 0 ; i--)
            {
                int thresholdRadius = threshold * i;
                Vector2[] lightCircumference = Rasterizer.GetCellsAlongEllipse(Position.X, Position.Y, thresholdRadius * 2, thresholdRadius);

                foreach (Vector2 point in lightCircumference)
                {
                    Vector2[] points = Rasterizer.PlotRasterizedLine(Position.X, Position.Y, point.X, point.Y);

                    foreach (Vector2 IlluminatedPoint in points)
                    {
                        if (!Level.IsTileTransparent(IlluminatedPoint.X, IlluminatedPoint.Y, false))
                        {
                            break;
                        }

                        int value = 0;
                        switch (i)
                        {
                            case 3:
                                value = 1;
                                break;
                            case 2:
                                value = 2;
                                break;
                            case 1:
                                value = 3;
                                break;
                        }

                        IlluminatedTiles[IlluminatedPoint] = value;
                    }
                }
            }
        }
    }
}
