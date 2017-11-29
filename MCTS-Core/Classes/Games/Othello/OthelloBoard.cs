using System;
using System.Collections.Generic;

namespace MCTS.Core.Games
{
    /// <summary>
    /// An Othello game board, which allows a game of Othello to be played out on it
    /// </summary>
    [Serializable]
    public class OthelloBoard : GridBasedBoard
    {
        /// <summary>
        /// Creates a new Othello board representing an empty game
        /// </summary>
        public OthelloBoard()
        {
            CurrentPlayer = 1;
            boardContents = new int[8,8];

            //Set the starting pieces on the board
            boardContents[3, 3] = 2;
            boardContents[3, 4] = 1;
            boardContents[4, 3] = 1;
            boardContents[4, 4] = 2;

            //Begin by populating the possible moves list
            CalculatePossibleMoves();
        }

        /// <summary>
        /// Private constructor used for deep copies of a board instance
        /// </summary>
        /// <param name="board">The board instance to make a deep copy of</param>
        private OthelloBoard(OthelloBoard board)
        {
            CurrentPlayer = board.CurrentPlayer;
            winner = board.Winner;
            boardContents = (int[,])board.boardContents.Clone();
            possibleMoves = new List<Move>(board.possibleMoves);
        }
        
        /// <summary>
        /// Gets a list of sandwiched pieces on this board, meaning pieces that would be sandwiched between two of the passed in players pieces if one was placed at the passed in point
        /// </summary>
        /// <param name="pos">The position being checked for sandwiched pieces</param>
        /// <param name="player">The player playing the piece</param>
        /// <returns>A list of sandwiched pieces as a result of the passed in player making a move at the passed in board position</returns>
        private List<Point> GetSandwichedPieces(Point pos, int player)
        {
            //The list of all points that represent cells that will be sandwiched
            List<Point> result = new List<Point>();

            //The list of points that will be sandwiched in the current direction
            List<Point> currentPoints = new List<Point>();

            #region Check cells to the left
            //Check left cells
            for(int i = pos.X - 1; i >= 0; i--)
            {
                if (boardContents[i, pos.Y] != 0 && boardContents[i, pos.Y] != player)
                {
                    currentPoints.Add(new Point(i, pos.Y));
                }
                else if (boardContents[i, pos.Y] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion

            currentPoints.Clear();

            #region Check cells to the right
            for (int i = pos.X + 1; i < Width; i++)
            {
                if (boardContents[i, pos.Y] != 0 && boardContents[i, pos.Y] != player)
                {
                    currentPoints.Add(new Point(i, pos.Y));
                }
                else if (boardContents[i, pos.Y] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion
            
            currentPoints.Clear();

            #region Check cells above
            for (int i = pos.Y - 1; i >= 0; i--)
            {
                if (boardContents[pos.X, i] != 0 && boardContents[pos.X, i] != player)
                {
                    currentPoints.Add(new Point(pos.X, i));
                }
                else if (boardContents[pos.X, i] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion

            currentPoints.Clear();

            #region Check cells below

            for (int i = pos.Y + 1; i < Height; i++)
            {
                if (boardContents[pos.X, i] != 0 && boardContents[pos.X, i] != player)
                {
                    currentPoints.Add(new Point(pos.X, i));
                }
                else if (boardContents[pos.X, i] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion

            currentPoints.Clear();

            #region Check cells up and right
            for (int x = pos.X + 1, y = pos.Y + 1; x < Width && y < Height; x++, y++)
            {
                if (boardContents[x,y] != 0 && boardContents[x,y] != player)
                {
                    currentPoints.Add(new Point(x,y));
                }
                else if (boardContents[x,y] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion

            currentPoints.Clear();

            #region Check cells up and left
            for (int x = pos.X - 1, y = pos.Y + 1; x >= 0 && y < Height; x--, y++)
            {
                if (boardContents[x, y] != 0 && boardContents[x, y] != player)
                {
                    currentPoints.Add(new Point(x, y));
                }
                else if (boardContents[x, y] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion

            currentPoints.Clear();

            #region Check cells down and left
            for (int x = pos.X - 1, y = pos.Y - 1; x >= 0 && y >= 0; x--, y--)
            {
                if (boardContents[x, y] != 0 && boardContents[x, y] != player)
                {
                    currentPoints.Add(new Point(x, y));
                }
                else if (boardContents[x, y] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion

            currentPoints.Clear();

            #region Check cells down and right
            for (int x = pos.X + 1, y = pos.Y - 1; x < Width && y >= 0; x++, y--)
            {
                if (boardContents[x, y] != 0 && boardContents[x, y] != player)
                {
                    currentPoints.Add(new Point(x, y));
                }
                else if (boardContents[x, y] == player && currentPoints.Count > 0)
                {
                    result.AddRange(currentPoints);
                    break;
                }
                else
                {
                    break;
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// Duplicate this baord state, returning a deep copy of it
        /// </summary>
        /// <returns>A deep copy of this board state</returns>
        public override Board Duplicate()
        {
            return new OthelloBoard(this);
        }

        /// <summary>
        /// Makes a move on this Othello board at the specified move position
        /// </summary>
        /// <param name="move">The move to make</param>
        /// <returns>A reference to this Othello board</returns>
        public override Board MakeMove(Move move)
        {
            OthelloMove m = (OthelloMove)move;

            //Change the cell being moved in belong to the current player
            boardContents[m.Position.X, m.Position.Y] = CurrentPlayer;

            //Change each capturable cell to belong to the current player
            foreach(Point p in m.CapturableCells)
            {
                boardContents[p.X, p.Y] = CurrentPlayer;
            }

            //Swap out the current player for the next player
            CurrentPlayer = NextPlayer;
            
            //Repopulate the possible moves list for the current player
            CalculatePossibleMoves();

            //If there are no possible moves left, then determine the winner of the game
            if (possibleMoves.Count == 0)
            {
                DetermineWinner();
            }

            //Call the MakeMove method on the Board base class, to save the last move made on this board
            return base.MakeMove(move);
        }

        /// <summary>
        /// Returns the possibleMoves list for this board instance, which contains a list of all possible moves tat can be taken from this board state
        /// </summary>
        /// <returns>A list of possible moves that can be taken from this baord state</returns>
        public override List<Move> PossibleMoves()
        {
            return possibleMoves;
        }

        /// <summary>
        /// Calculates the possible moves that can be made from this board state for the current player and stores the result in the possibleMoves list
        /// </summary>
        private void CalculatePossibleMoves()
        {
            possibleMoves = new List<Move>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (boardContents[x, y] == 0)
                    {
                        List<Point> sandwichedPieces = GetSandwichedPieces(new Point(x, y), CurrentPlayer);
                        if (sandwichedPieces.Count != 0)
                        {
                            possibleMoves.Add(new OthelloMove(new Point(x, y), sandwichedPieces));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines the winner of the current Othello game <para/>
        /// The winner is the player with the most pieces on the board when there are no possible moves left in the game
        /// </summary>
        protected override void DetermineWinner()
        {
            //If there are still possible moves from this point, then the game is not over
            if (possibleMoves.Count != 0)
            {
                return;
            }

            int player1Pieces = 0;
            int player2Pieces = 0;

            //Count the total number of pieces for both players on the board
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    if(boardContents[x,y] == 1)
                    {
                        player1Pieces++;
                    }
                    else if(boardContents[x,y] == 2)
                    {
                        player2Pieces++;
                    }
                }
            }

            //Determine the winner based on the player with the highest amount of pieces
            if(player1Pieces > player2Pieces)
            {
                winner = 1;
            }
            else if(player2Pieces > player1Pieces)
            {
                winner = 2;
            }
            else
            {
                winner = 0;
            }
        }

        /// <summary>
        /// There is no way to speed up the calculation of a winner based on the last move in Othello <para/>
        /// This method must still be kept to not break polymorphism
        /// </summary>
        /// <param name="move">The previous move made on this board</param>
        protected override void DetermineWinner(Move move)
        {
            DetermineWinner();
        }

        /// <summary>
        /// Gets the amount of players on this board
        /// </summary>
        /// <returns>The amount of players on this baord</returns>
        protected override int PlayerCount()
        {
            return 2;
        }
    }
}
