using System;

namespace HeistGame
{
    /// <summary>
    /// Helper struct that holds a int X, int Y pair
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        public int X;
        public int Y;

        /// <summary>
        /// Creates a Coordinate
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">the Y coordinate</param>
        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) => obj is Vector2 other && Equals(other);

        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(X);
            hash.Add(Y);

            return hash.ToHashCode();
        }
    }
}
