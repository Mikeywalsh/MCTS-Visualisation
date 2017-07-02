using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTTBoard : Board {

    /// <summary>
    /// The contents of the game board
    /// </summary>
    private int[,] boardContents;

    /// <summary>
    /// Creates a new Tic-Tac-Toe board representing an empty game
    /// </summary>
    public TTTBoard()
    {
        currentPlayer = 2;
        boardContents = new int[3,3];
        boardContents[2, 0] = 1;
        boardContents[1, 1] = 1;
        //boardContents[0, 2] = 2;
        //boardContents[1, 2] = 1;
        //boardContents[2, 2] = 2;
    }

    /// <summary>
    /// Create a new Tic-Tac-Toe board as a copy from an existing board
    /// </summary>
    /// <param name="board">The board to make a copy of</param>
    private TTTBoard(TTTBoard board)
    {
        currentPlayer = board.CurrentPlayer;
        boardContents = (int[,])board.BoardContents.Clone();
    }

    /// <summary>
    /// Duplicates the current Tic-Tac-Toe board
    /// </summary>
    /// <returns>A clone of the current Tic-Toe-Board</returns>
    public override Board Duplicate()
    {
        return new TTTBoard(this);
    }

    public override Board MakeMove(Move move)
    {
        TTTMove m = (TTTMove)move;

        //Make the move if possible, if a move has already been made at this position then throw an invalid move exception
        if(boardContents[m.x, m.y] == 0)
        {
            //Make the move on this board
            boardContents[m.x, m.y] = currentPlayer;

            //Determine if there is a winner
            DetermineWinner();

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
        List<Move> moves = new List<Move>();

        //Search the board for any cells containing a value of 0, which are areas in which moves can be made
        for(int y = 0; y < 3; y++)
        {
            for(int x = 0; x < 3; x++)
            {
                if(boardContents[x,y] == 0)
                {
                    moves.Add(new TTTMove(x, y));
                }
            }
        }

        return moves;
    }

    /// <summary>
    /// Returns the contents of this Tic-Tac-Toe board
    /// </summary>
    /// <returns>The contents of this Tic-Tac-Toe board</returns>
    public int[,] BoardContents
    {
        get { return boardContents; }
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
        else
        {
            winner = -1;
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
