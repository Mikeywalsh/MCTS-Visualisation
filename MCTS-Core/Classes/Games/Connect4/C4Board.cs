using System;
using System.Collections.Generic;

namespace MCTS.Core.Games
{
    /// <summary>
    /// A Connect 4 game board, which allows a game of Connect 4 to be played out on it
    /// </summary>
    [Serializable]
    public class C4Board : GridBasedBoard
    {
        /// <summary>
        /// Creates a new Connect 4 board representing an empty game
        /// </summary>
        public C4Board()
        {
            CurrentPlayer = 1;
            boardContents = new int[7, 6];

            //Create the list of possible moves
            possibleMoves = new List<Move>();

            //Add a move for every column, since this is an empty game board
            for (int x = 0; x < Width; x++)
            {
                possibleMoves.Add(new C4Move(x));
            }
        }

        /// <summary>
        /// Create a new Connect 4 board as a copy from an existing board
        /// </summary>
        /// <param name="board">The board to make a copy of</param>
        private C4Board(C4Board board)
        {
            CurrentPlayer = board.CurrentPlayer;
            winner = board.Winner;
            boardContents = (int[,])board.boardContents.Clone();
            possibleMoves = new List<Move>(board.possibleMoves);
        }

        /// <summary>
        /// Duplicates the current Connect 4 board
        /// </summary>
        /// <returns>A clone of the current Connect 4</returns>
        public override Board Duplicate()
        {
            return new C4Board(this);
        }

        /// <summary>
        /// Makes a move on this Connect 4 board at the specified move position
        /// </summary>
        /// <param name="move">The move to make</param>
        /// <returns>A reference to this Connect 4 board</returns>
        public override Board MakeMove(Move move)
        {
            C4Move m = (C4Move)move;

            //Make a move in the first available row in the column
            for (int y = 0; y < Height; y++)
            {
                if (boardContents[m.X, y] == 0)
                {
                    //Make the move on this board
                    boardContents[m.X, y] = CurrentPlayer;

                    //Set the y position of the move
                    m.SetY(y);

                    //If this move made a column full, remove this column from the list of possible moves
                    if (y == Height - 1)
                    {
                        possibleMoves.Remove(m);
                    }

                    //Determine if there is a winner
                    DetermineWinner(m);

                    //Swap out the current player
                    CurrentPlayer = NextPlayer;

                    //Call the MakeMove method on the Board base class, to save the last move made on this board
                    return base.MakeMove(move);
                }
            }

            //If there are no available rows, raise an exception
            throw new InvalidMoveException("Tried to make a move in a full column");
        }

        /// <summary>
        /// Get a list of all possible moves for this Connect 4 board instance
        /// </summary>
        /// <returns>A list of all possible moves for this Connect 4 board instance</returns>
        public override List<Move> PossibleMoves()
        {
            return possibleMoves;
        }

        /// <summary>
        /// Determine if the current game is over
        /// </summary>
        protected override void DetermineWinner()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //Get the current cells value
                    int currentCell = boardContents[x, y];

                    //If the current cells value is 0, don't bother checking for a winner starting from this cell
                    if (currentCell == 0)
                    {
                        continue;
                    }

                    //Check for a horizontal win
                    if (x < Width - 3)
                    {
                        if (boardContents[x + 1, y] == currentCell && boardContents[x + 2, y] == currentCell && boardContents[x + 3, y] == currentCell)
                        {
                            winner = CurrentPlayer;
                            return;
                        }
                    }

                    //Check for a vertical win
                    if (y < Height - 3)
                    {
                        if (boardContents[x, y + 1] == currentCell && boardContents[x, y + 2] == currentCell && boardContents[x, y + 3] == currentCell)
                        {
                            winner = CurrentPlayer;
                            return;
                        }
                    }

                    //Check for an upwards diagonal win
                    if (x < Width - 3 && y < Height - 3)
                    {
                        if (boardContents[x + 1, y + 1] == currentCell && boardContents[x + 2, y + 2] == currentCell && boardContents[x + 3, y + 3] == currentCell)
                        {
                            winner = CurrentPlayer;
                            return;
                        }
                    }

                    //Check for a downwards diagonal win
                    if (x < Width - 3 && y > 2)
                    {
                        if (boardContents[x + 1, y - 1] == currentCell && boardContents[x + 2, y - 2] == currentCell && boardContents[x + 3, y - 3] == currentCell)
                        {
                            winner = CurrentPlayer;
                            return;
                        }
                    }

                    //If there are no possible moves left and there is still no winner, the game is a draw
                    if (possibleMoves.Count == 0)
                    {
                        winner = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Determine if the current game is over <para/>
        /// Uses knowledge of the last move to save computation time
        /// </summary>
        /// <param name="move">The last move made</param>
        protected override void DetermineWinner(Move move)
        {
            //This method seems a bit confusing, but it is just changing the worst case number of win checks from 196 to 16
            C4Move m = (C4Move)move;

            //Get the current cells value
            int currentCell;

            //Check for a horizontal win
            //Check 3 cells infront and behind of this cell to determine if there is a winner
            for (int x = m.X; x >= m.X - 3 && x >= 0; x--)
            {
                currentCell = boardContents[x, m.Y];
                if (x < Width - 3)
                {
                    if (boardContents[x + 1, m.Y] == currentCell && boardContents[x + 2, m.Y] == currentCell && boardContents[x + 3, m.Y] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }
            }

            //Check for a vertical win
            //Check 3 cells up and down from this cell to determine if there is a winner
            for (int y = m.Y; y >= m.Y - 3 && y >= 0; y--)
            {
                currentCell = boardContents[m.X, y];
                if (y < Height - 3)
                {
                    if (boardContents[m.X, y + 1] == currentCell && boardContents[m.X, y + 2] == currentCell && boardContents[m.X, y + 3] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }
            }

            //Check for an upwards diagonal win
            //Check 3 cells down and left and 3 cells up and right from this cell to determine if there is a winner
            for (int y = m.Y, x = m.X; x >= m.X - 3 && y >= m.Y - 3 && x >= 0 && y >= 0; x--, y--)
            {
                currentCell = boardContents[x, y];
                if (x < Width - 3 && y < Height - 3)
                {
                    if (boardContents[x + 1, y + 1] == currentCell && boardContents[x + 2, y + 2] == currentCell && boardContents[x + 3, y + 3] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }
            }

            //Check for a downwards diagonal win
            //Check 3 cells up and left and 3 cells down and right from this cell to determine if there is a winner
            for (int y = m.Y, x = m.X; x >= m.X - 3 && y <= m.Y + 3 && x >= 0 && y < Height; x--, y++)
            {
                currentCell = boardContents[x, y];
                if (x < Width - 3 && y > 2)
                {
                    if (boardContents[x + 1, y - 1] == currentCell && boardContents[x + 2, y - 2] == currentCell && boardContents[x + 3, y - 3] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }
            }

            //If there are no possible moves left and there is still no winner, the game is a draw
            if (possibleMoves.Count == 0)
            {
                winner = 0;
            }
        }

        /// <summary>
        /// Used to obtain the number of players on a Connect 4 board, which is always 2
        /// </summary>
        /// <returns>The number of players on a Connect 4 board, which is always 2</returns>
        protected override int PlayerCount()
        {
            return 2;
        }
    }
}
