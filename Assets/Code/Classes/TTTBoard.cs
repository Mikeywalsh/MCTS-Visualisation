using System.Collections.Generic;

/// <summary>
/// A Tic-Tac-Toe game board, which allows a game of Tic-Tac-Toe to be played out on it
/// </summary>
public class TTTBoard : GridBasedBoard {

    /// <summary>
    /// Creates a new Tic-Tac-Toe board representing an empty game
    /// </summary>
    public TTTBoard()
    {
        currentPlayer = 1;
        boardContents = new int[3,3];

        //Create the list of possible moves
        possibleMoves = new List<Move>();

        //Add a move for every cell, since this is an empty game board
        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                possibleMoves.Add(new TTTMove(x, y));
            }
        }
    }

    /// <summary>
    /// Create a new Tic-Tac-Toe board as a copy from an existing board
    /// </summary>
    /// <param name="board">The board to make a copy of</param>
    private TTTBoard(TTTBoard board)
    {
        currentPlayer = board.CurrentPlayer;
        boardContents = (int[,])board.boardContents.Clone();
        possibleMoves = new List<Move>(board.possibleMoves);
    }

    /// <summary>
    /// Duplicates the current Tic-Tac-Toe board
    /// </summary>
    /// <returns>A clone of the current Tic-Toe-Board</returns>
    public override Board Duplicate()
    {
        return new TTTBoard(this);
    }

    /// <summary>
    /// Makes a move on this Tic-Tac-Toe board at the specified move position
    /// </summary>
    /// <param name="move">The move to make</param>
    /// <returns>A reference to this Tic-Tac-Toe board</returns>
    public override Board MakeMove(Move move)
    {
        TTTMove m = (TTTMove)move;

        //Make the move if possible, if a move has already been made at this position then throw an invalid move exception
        if(boardContents[m.X, m.Y] == 0)
        {
            //Make the move on this board
            boardContents[m.X, m.Y] = currentPlayer;

            //Remove the move just performed from the list of possible moves
            possibleMoves.Remove(m);

            //Determine if there is a winner
            DetermineWinner(move);

            //Swap out the current player
            currentPlayer = NextPlayer;
        }
        else
        {
            throw new InvalidMoveException("Move has already been made at: " + m.ToString());
        }

        return this;
    }

    /// <summary>
    /// Get a list of all possible moves for this Tic-Tac-Toe board instance
    /// </summary>
    /// <returns>A list of all possible moves for this Tic-Tac-Toe board instance</returns>
    public override List<Move> PossibleMoves()
    {
        return possibleMoves;
    }

    /// <summary>
    /// Determine if the current game is over
    /// </summary>
    protected override void DetermineWinner()
    {
        //First check all rows and columns for a winner
        for(int i = 0; i < 3; i++)
        {
            if(boardContents[0,i] != 0 && boardContents[0,i] == boardContents[1,i] && boardContents[0,i] == boardContents[2,i] || boardContents[i, 0] != 0 && boardContents[i,0] == boardContents[i,1] && boardContents[i,0] == boardContents[i,2])
            {
                winner = currentPlayer;
                return;
            }
        }

        //Then check each diagonal
        if(boardContents[0, 0] != 0 && boardContents[0,0] == boardContents[1,1] && boardContents[0,0] == boardContents[2,2] || boardContents[0, 2] != 0 && boardContents[0,2] == boardContents[1,1] && boardContents[0,2] == boardContents[2,0])
        {
            winner = currentPlayer;
            return;
        }

        //If there is still no winner and the board is full, the game is a tie, otherwise no winner yet
        if(PossibleMoves().Count == 0)
        {
            winner = 0;
        }
    }

    /// <summary>
    /// Determine if the current game is over
    /// Uses knowledge of the last move to save computation time
    /// </summary>
    /// <param name="move">The last move made</param>
    protected override void DetermineWinner(Move move)
    {
        TTTMove m = (TTTMove)move;

        //Check the row and column of the last move
        if (boardContents[0, m.Y] == boardContents[1, m.Y] && boardContents[1, m.Y] == boardContents[2, m.Y] || boardContents[m.X, 0] == boardContents[m.X, 1] && boardContents[m.X, 1] == boardContents[m.X, 2])
        {
            winner = currentPlayer;
            return;
        }

        //Then check each diagonal
        if (boardContents[0, 0] != 0 && boardContents[0, 0] == boardContents[1, 1] && boardContents[0, 0] == boardContents[2, 2] || boardContents[0, 2] != 0 && boardContents[0, 2] == boardContents[1, 1] && boardContents[0, 2] == boardContents[2, 0])
        {
            winner = currentPlayer;
            return;
        }

        //If there is still no winner and the board is full, the game is a tie, otherwise no winner yet
        if (PossibleMoves().Count == 0)
        {
            winner = 0;
        }
    }

    /// <summary>
    /// Used to obtain the number of players on a Tic-Tac-Toe board, which is always 2
    /// </summary>
    /// <returns>The number of players on a Tic-Tac-Toe board, which is always 2</returns>
    protected override int PlayerCount()
    {
        return 2;
    }

    /// <summary>
    /// Gives a string representation of this Tic-Tac-Toe board
    /// </summary>
    /// <returns>A string representation of this Tic-Tac-Toe board</returns>
    public override string ToString()
    {
        string result = "\n";

        for(int y = 0; y < 3; y++)
        {
            for(int x = 0; x < 3; x++)
            {
                result += boardContents[x, y] + " ";
            }

            if (y != 2)
            {
                result += '\n';
            }
        }

        return result;
    }
}
