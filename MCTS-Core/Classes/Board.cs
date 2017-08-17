using System;
using System.Collections.Generic;

namespace MCTS_Core
{
    /// <summary>
    /// Represents an abstract board state
    /// Can be used for implementations of many different games
    /// </summary>
    public abstract class Board
    {

        /// <summary>
        /// The playerID of the current player
        /// </summary>
        protected int currentPlayer;

        /// <summary>
        /// The winner value of the board state
        /// -1 if no winner yet
        /// 0 if game is a tie
        /// Any positive integer is the winning players ID
        /// </summary>
        protected int winner = -1;

        /// <summary>
        /// A list of all possible moves that can follow from this board state
        /// </summary>
        protected List<Move> possibleMoves;

        /// <summary>
        /// Simulates random plays on this board until the game has ended
        /// </summary>
        /// <returns>The value of the winner variable at the end of the game</returns>
        public int SimulateUntilEnd()
        {
            Board temp = Duplicate();

            while (temp.Winner == -1)
            {
                temp.MakeMove(temp.possibleMoves.PickRandom());
            }

            return temp.winner;
        }

        /// <summary>
        /// Returns the playerID of the current player
        /// </summary>
        /// <returns>The playerID of the current player</returns>
        public int CurrentPlayer
        {
            get { return currentPlayer; }
        }

        /// <summary>
        /// Returns the playerID of the previous player
        /// </summary>
        /// <returns>The playerID of the previous player</returns>
        public int PreviousPlayer
        {
            get
            {
                //Return the previous player, if the previous player ID is less than 1, wrap around
                if (currentPlayer - 1 <= 0)
                {
                    return PlayerCount();
                }
                else
                {
                    return currentPlayer - 1;
                }
            }
        }

        /// <summary>
        /// Returns the playerID of the next player
        /// </summary>
        /// <returns>The playerID of the next player</returns>
        public int NextPlayer
        {
            get
            {
                //Return the next player, if the next player ID is past the max player count, wrap around
                if (currentPlayer + 1 > PlayerCount())
                {
                    return 1;
                }
                else
                {
                    return currentPlayer + 1;
                }
            }
        }

        /// <summary>
        /// Returns the value of the winner integer
        /// -1 if no winner yet
        /// 0 if game is a tie
        /// Any positive integer is the winning players ID
        /// </summary>
        /// <returns>An integer indicating if the game has a winner, is a draw, or neither</returns>
        public int Winner
        {
            get { return winner; }
        }

        /// <summary>
        /// Performs a move on this board state for the current player and returns the updated state.
        /// </summary>
        /// <param name="move">The move to make</param>
        /// <returns>A board instance which has had the passed in move made</returns>
        public abstract Board MakeMove(Move move);

        /// <summary>
        /// Gets a list of possible moves that can follow from this board state
        /// </summary>
        /// <returns>A list of moves that can follow from this board state</returns>
        public abstract List<Move> PossibleMoves();

        /// <summary>
        /// Performs a deep copy of the current board state and returns the copy
        /// </summary>
        /// <returns>A copy of this board state</returns>
        public abstract Board Duplicate();

        /// <summary>
        /// Returns amount of players playing on this board
        /// </summary>
        /// <returns>The amount of players playing on this board</returns>
        protected abstract int PlayerCount();

        /// <summary>
        /// Determines if there is a winner or not for this board state and updates the winner integer accordingly
        /// </summary>
        protected abstract void DetermineWinner();

        /// <summary>
        /// A more efficient method of determining if there is a winner
        /// Saves time by using knowledge of the last move to remove unnessessary computation
        /// </summary>
        /// <param name="move">The last move made before calling this method</param>
        protected abstract void DetermineWinner(Move move);
    }
}
