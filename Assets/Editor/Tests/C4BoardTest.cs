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
    }

    [Test]
    public void DuplicateTest()
    {
        //Create a new board and make a move in it
        C4Board boardA = new C4Board();
        boardA.MakeMove(new C4Move(3));

        //Duplicate the board and store it in a new board instance
        C4Board boardB = (C4Board)boardA.Duplicate();

        //These two board instances should share no memory, lets prove it by making moves in each of them and checking the other
        boardA.MakeMove(new C4Move(4));
        //Assert.AreEqual("lol", Assembly.GetCallingAssembly().FullName);
        //Assert.AreEqual(4, boardA.GetCell(4))
        //int a = boardA.boardContents[0, 0];
    }
}
