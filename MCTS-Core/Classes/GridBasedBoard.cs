using System;

namespace MCTS.Core
{
    /// <summary>
    /// An abstract extension of <see cref="Board"/> <para/>
    /// Contains common elements of grid based board games, such as Tic-Tac-Toe, Connect 4, Chess, etc
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// Gives a string representation of this grid based board
        /// </summary>
        /// <returns>A string representation of this grid based board</returns>
        public override string ToString()
        {
            string result = "\n";

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    result += boardContents[x, y];

                    if(x != Width - 1)
                    {
                        result += " ";
                    }
                }

                if (y != 0)
                {
                    result += '\n';
                }
            }

            return result;
        }

        /// <summary>
        /// Gives a rich text string representation of this grid based board <para/>
        /// The output string will have color tags that make the board easier to read
        /// </summary>
        /// <returns>A rich text string representation of this grid based board</returns>
        public override string ToRichString()
        {
            string boardText = ToString();

            boardText = boardText.Replace("0", "<color=#ffffff>0</color>");
            boardText = boardText.Replace("1", "<color=#ff0000>1</color>");
            boardText = boardText.Replace("2", "<color=#0000ff>2</color>");

            return boardText;
        }
    }
}
