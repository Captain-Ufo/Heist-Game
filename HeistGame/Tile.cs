/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System;

namespace HeistGame
{
    /// <summary>
    /// Helper class for pathfinding
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// The X coordinate of this tile
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y coordinate of this tile
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The cost to move into this tile, for the A* pathfinding
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// The as the crow flies distance between this tile and the destination, regardless of obstacles
        /// </summary>
        public int Distance { get; private set; }

        /// <summary>
        /// Cost + Distance
        /// </summary>
        public int CostDistance
        {
            get { return Cost + Distance; }
            set { }
        }

        /// <summary>
        /// The tile the pathfinding algorithm comes from when reaching this one
        /// </summary>
        public Tile Parent { get; set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;

            Cost = 0;
            Distance = 0;
        }

        /// <summary>
        /// Calculates and assignes the absolute value of the distance between this tile and the target indicated
        /// </summary>
        /// <param name="destinationX">The X coordinate of the destination</param>
        /// <param name="destinationY">The Y coordinate of the destination</param>
        public void SetDistance(int destinationX, int destinationY)
        {
            Distance = Math.Abs(destinationX - X) + Math.Abs(destinationY - Y);
        }
    }
}
