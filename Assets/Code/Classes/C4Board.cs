using System;
using System.Collections.Generic;

/// <summary>
/// A Connect 4 game board, which allows a game of Connect 4 to be played out on it
/// </summary>
public class C4Board : Board {

    /// <summary>
    /// The contents of the game board
    /// </summary>
    private int[,] boardContents;

    /// <summary>
    /// Creates a new Connect 4 board representing an empty game
    /// </summary>
    public C4Board()
    {
        currentPlayer = 1;
        boardContents = new int[7, 7];
    }

    /// <summary>
    /// Create a new Connect 4 board as a copy from an existing board
    /// </summary>
    /// <param name="board">The board to make a copy of</param>
    private C4Board(C4Board board)
    {
        currentPlayer = board.CurrentPlayer;
        boardContents = (int[,])board.boardContents.Clone();
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
        for (int y = 0; y < boardContents.GetLength(1); y++)
        {
            if (boardContents[m.x, y] == 0)
            {
                //Make the move on this board
                boardContents[m.x, y] = currentPlayer;

                m.y = y;

                //Determine if there is a winner
                DetermineWinner(m);

                //Swap out the current player
                currentPlayer = NextPlayer;

                return this;
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
        List<Move> moves = new List<Move>();
        
        for(int column = 0; column < boardContents.GetLength(0); column++)
        {
            if(!ColumnFull(column))
            {
                moves.Add(new C4Move(column));
            }
        }

        return moves;
    }

    /// <summary>
    /// Returns the contents of this Connect 4 board
    /// </summary>
    /// <returns>The contents of this Connect 4 board</returns>
    public int[,] BoardContents
    {
        get { return boardContents; }
    }

    /// <summary>
    /// Determine if the current game is over
    /// </summary>
    protected override void DetermineWinner()
    {
        for(int y = 0; y < boardContents.GetLength(1); y++)
        {
            for (int x = 0; x < boardContents.GetLength(0); x++)
            {
                //Get the current cells value
                int currentCell = boardContents[x, y];

                //If the current cells value is 0, don't bother checking for a winner starting from this cell
                if(currentCell == 0)
                {
                    continue;
                }

                //Check for a horizontal win
                if (x < boardContents.GetLength(0) - 3)
                {
                    if (boardContents[x + 1, y] == currentCell && boardContents[x + 2, y] == currentCell && boardContents[x + 3, y] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }

                //Check for a vertical win
                if (y < boardContents.GetLength(1) - 3)
                {
                    if (boardContents[x, y + 1] == currentCell && boardContents[x, y + 2] == currentCell && boardContents[x, y + 3] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }

                //Check for an upwards diagonal win
                if (x < boardContents.GetLength(0) - 3 && y < boardContents.GetLength(1) - 3)
                {
                    if (boardContents[x + 1, y + 1] == currentCell && boardContents[x + 2, y + 2] == currentCell && boardContents[x + 3, y + 3] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }

                //Check for a downwards diagonal win
                if (x < boardContents.GetLength(0) - 3 && y > 2)
                {
                    if (boardContents[x + 1, y - 1] == currentCell && boardContents[x + 2, y - 2] == currentCell && boardContents[x + 3, y - 3] == currentCell)
                    {
                        winner = CurrentPlayer;
                        return;
                    }
                }

                //If there are no possible moves left and there is still no winner, the game is a draw
                if (PossibleMoves().Count == 0)
                {
                    winner = 0;
                }
            }
        }
    }

    /// <summary>
    /// Determine if the current game is over
    /// Uses knowledge of the last move to save computation time
    /// </summary>
    /// <param name="move">The last move made</param>
    protected override void DetermineWinner(Move move)
    {
        C4Move m = (C4Move)move;

        //Get the current cells value
        int currentCell = boardContents[m.x, m.y];

        //Check for a horizontal win
        if (m.x < boardContents.GetLength(0) - 3)
        {
            if (boardContents[m.x + 1, m.y] == currentCell && boardContents[m.x + 2, m.y] == currentCell && boardContents[m.x + 3, m.y] == currentCell)
            {
                winner = CurrentPlayer;
                return;
            }
        }

        //Check for a vertical win
        if (m.y < boardContents.GetLength(1) - 3)
        {
            if (boardContents[m.x, m.y + 1] == currentCell && boardContents[m.x, m.y + 2] == currentCell && boardContents[m.x, m.y + 3] == currentCell)
            {
                winner = CurrentPlayer;
                return;
            }
        }

        //Check for an upwards diagonal win
        if (m.x < boardContents.GetLength(0) - 3 && m.y < boardContents.GetLength(1) - 3)
        {
            if (boardContents[m.x + 1, m.y + 1] == currentCell && boardContents[m.x + 2, m.y + 2] == currentCell && boardContents[m.x + 3, m.y + 3] == currentCell)
            {
                winner = CurrentPlayer;
                return;
            }
        }

        //Check for a downwards diagonal win
        if (m.x < boardContents.GetLength(0) - 3 && m.y > 2)
        {
            if (boardContents[m.x + 1, m.y - 1] == currentCell && boardContents[m.x + 2, m.y - 2] == currentCell && boardContents[m.x + 3, m.y - 3] == currentCell)
            {
                winner = CurrentPlayer;
                return;
            }
        }

        //If there are no possible moves left and there is still no winner, the game is a draw
        if (PossibleMoves().Count == 0)
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

    /// <summary>
    /// A convenience method used to determine if a given column is full
    /// Used when calculating possible moves
    /// </summary>
    /// <param name="column">The column to check if it is full or not</param>
    /// <returns>True if the column is full, false if not</returns>
    private bool ColumnFull(int column)
    {
        if(column < 0 || column > boardContents.GetLength(0))
        {
            throw new InvalidMoveException("The selected column is out of bounds of the " + boardContents.GetLength(0) + " column wide game area");
        }

        for(int y = 0; y < boardContents.GetLength(1); y++)
        {
            if (boardContents[column, y] == 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gives a string representation of this Connect 4 board
    /// </summary>
    /// <returns>A string representation of this Connect 4 board</returns>
    public override string ToString()
    {
        string result = "\n";

        for (int y = boardContents.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < boardContents.GetLength(0); x++)
            {
                result += boardContents[x, y] + " ";
            }

            if (y != 0)
            {
                result += '\n';
            }
        }

        return result;
    }
}
