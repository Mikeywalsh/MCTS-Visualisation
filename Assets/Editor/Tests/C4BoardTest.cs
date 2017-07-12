using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Reflection;

public class C4BoardTest {

    [Test]
	public void CreateBoardTest()
    {
        C4Board board = new C4Board();

        //Check that the current player is player 1
        Assert.AreEqual(1, board.CurrentPlayer);
        
        //Ensure that the created board is empty
        for(int y = 0; y < board.Height; y++)
        {
            for(int x = 0; x < board.Width; x++)
            {
                Assert.AreEqual(0, board.GetCell(x,y));
            }
        }

        //Ensure that the winner value is - 1
        Assert.AreEqual(-1, board.Winner);
    }

    [Test]
    public void MakeMoveTest()
    {
        //Create a new board and make a move in it
        C4Board board = new C4Board();
        board.MakeMove(new C4Move(2));

        Assert.AreEqual(1, board.GetCell(2, 0));
    }

    [Test]
    public void MakeMoveInFullColumn()
    {
        C4Board board = new C4Board();

        for(int i = 0; i < board.Height; i++)
        {
            board.MakeMove(new C4Move(1));
        }

        Assert.Throws<InvalidMoveException>(() => board.MakeMove(new C4Move(1)));
    }

    [Test]
    public void GetPossibleMovesEmptyBoard()
    {
        C4Board board = new C4Board();
        Assert.AreEqual(board.Width, board.PossibleMoves().Count);        
    }

    [Test]
    public void GetPossibleMovesOneFullColumn()
    {
        C4Board board = new C4Board();
        for (int i = 0; i < board.Height; i++)
        {
            board.MakeMove(new C4Move(3));
        }
        Assert.AreEqual(board.Width - 1, board.PossibleMoves().Count);
    }

    [Test]
    public void GetPossibleMovesFullBoard()
    {
        C4Board board = new C4Board();
        for (int y = 0; y < board.Height; y++)
        {
            for(int x = 0; x < board.Width; x++)
            {
                board.MakeMove(new C4Move(x));
            }
        }
        Assert.AreEqual(0, board.PossibleMoves().Count);
    }

    [Test]
    public void DuplicateTest()
    {
        //Create a new board and make a move in it
        C4Board boardA = new C4Board();
        boardA.MakeMove(new C4Move(3));

        //Duplicate the board and store it in a new board instance
        C4Board boardB = (C4Board)boardA.Duplicate();

        //Ensure the move made before duplication is present in both boards
        Assert.AreEqual(1, boardA.GetCell(3, 0));
        Assert.AreEqual(1, boardB.GetCell(3, 0));

        //These two board instances should share no memory, lets prove it by making moves in each of them and checking the other
        boardA.MakeMove(new C4Move(6));
        Assert.AreEqual(2, boardA.GetCell(6, 0));
        Assert.AreEqual(0, boardB.GetCell(6, 0));

        boardB.MakeMove(new C4Move(3));
        Assert.AreEqual(0, boardA.GetCell(3, 1));
        Assert.AreEqual(2, boardB.GetCell(3, 1));
    }

    [Test]
    public void WinTestHorizonatal()
    {
        C4Board board = new C4Board();

        board.MakeMove(new C4Move(1));
        board.MakeMove(new C4Move(1));
        board.MakeMove(new C4Move(2));
        board.MakeMove(new C4Move(2));
        board.MakeMove(new C4Move(3));
        board.MakeMove(new C4Move(3));
        board.MakeMove(new C4Move(4));

        //Player 1 should have won the game
        Assert.AreEqual(1, board.Winner);
    }

    [Test]
    public void WinTestVertical()
    {
        C4Board board = new C4Board();

        board.MakeMove(new C4Move(1));
        board.MakeMove(new C4Move(2));
        board.MakeMove(new C4Move(1));
        board.MakeMove(new C4Move(2));
        board.MakeMove(new C4Move(1));
        board.MakeMove(new C4Move(2));
        board.MakeMove(new C4Move(3));
        board.MakeMove(new C4Move(2));

        //Player 2 should have won the game
        Assert.AreEqual(2, board.Winner);
    }

    [Test]
    public void WinTestUpwardsDiagonal()
    {
        C4Board board = new C4Board();

        board.MakeMove(new C4Move(1));
        board.MakeMove(new C4Move(2));
        board.MakeMove(new C4Move(2));
        board.MakeMove(new C4Move(3));
        board.MakeMove(new C4Move(3));
        board.MakeMove(new C4Move(4));
        board.MakeMove(new C4Move(3));
        board.MakeMove(new C4Move(4));
        board.MakeMove(new C4Move(4));
        board.MakeMove(new C4Move(4));
        board.MakeMove(new C4Move(4));

        Debug.Log(board.ToString());

        //Player 1 should have won the game
        Assert.AreEqual(1, board.Winner);
    }

}
