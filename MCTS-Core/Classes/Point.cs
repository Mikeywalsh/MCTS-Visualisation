namespace MCTS.Core
{
    /// <summary>
    /// A 2D point that uses integers, used to easily refer to cells in a 2D array
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// The X coordinate of this point
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate of this point
        /// </summary>
        public int Y;

        /// <summary>
        /// Creates a new point with the given X and Y coordinates
        /// </summary>
        /// <param name="x">The X coordinate of the point to set</param>
        /// <param name="y">The Y coordinate of the point to set</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
