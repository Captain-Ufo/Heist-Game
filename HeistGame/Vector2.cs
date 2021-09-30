namespace HeistGame
{
    /// <summary>
    /// Helper struct that holds a int X, int Y pair
    /// </summary>
    public struct Vector2
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
    }
}
