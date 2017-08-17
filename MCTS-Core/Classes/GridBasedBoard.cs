using System;

namespace MCTS_Core
{
    /// <summary>
    /// An abstract extension of <see cref="Board"/>
    /// Contains common elements of grid based board games, such as Tic-Tac-Toe, Connect 4, Chess, etc
    /// </summary>
    public abstract class GridBasedBoard : Board
    {

        /// <summary>
        /// The contents of this game board
        /// </summary>
        protected int[,] boardContents;

        /// <summary>
        /// Returns the contents of this board cell at the given indices
        /// </summary>
        /// <param name="x">The x index of the cell to get</param>
        /// <param name="y">The y index of the cell to get</param>
        /// <returns>The contents of this board cell at the index [x,y]</returns>
        public int GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new IndexOutOfRangeException("Cell (" + x + "," + y + ") is out of range of the " + Width + "*" + Height + " game board area");
            }
            return boardContents[x, y];
        }

        /// <summary>
        /// The width of this game board
        /// </summary>
        public int Width
        {
            get
            {
                return boardContents.GetLength(0);
            }
        }

        /// <summary>
        /// The height of this game board
        /// </summary>
        public int Height
        {
            get
            {
                return boardContents.GetLength(1);
            }
        }
    }
}
