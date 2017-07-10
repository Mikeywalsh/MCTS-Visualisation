﻿using NUnit.Framework;

/// <summary>
/// This class tests all aspects of the <see cref="C4Move"/> class
/// </summary>
public class C4MoveTest {

    [Test]
	public void CreateValidMoveTest()
    {
        try
        {
            C4Move move = CreateMove(4);
            Assert.AreEqual(4, move.x);
            Assert.AreEqual(0, move.y);
        }
        catch(InvalidMoveException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void CreateInvalidMoveTestLow()
    {
        //Attempt to create a move with an x position of -1, which is not a valid positon
        //This should throw an InvalidMoveException
        Assert.Throws<InvalidMoveException>(() => CreateMove(-1));
    }

    [Test]
    public void CreateInvalidMoveTestHigh()
    {
        //Attempt to create a move with an x position of 7, which is the 8th position on the board
        //this should throw an InvalidMoveException as the board is only 7 columns wide
        Assert.Throws<InvalidMoveException>(() => CreateMove(7));
    }

    [Test]
    public void EqualityTest()
    {
        C4Move moveA = CreateMove(3);
        C4Move moveB = CreateMove(3);

        Assert.IsTrue(moveA.Equals(moveB));
    }

    [Test]
    public void InequalityTest()
    {
        C4Move moveA = CreateMove(0);
        C4Move moveB = CreateMove(1);

        Assert.IsFalse(moveA.Equals(moveB));
    }

    [Test]
    public void ValidHashCodeTest()
    {
        C4Move move = CreateMove(6);

        Assert.AreEqual(6, move.GetHashCode());
    }

    [Test]
    public void ToStringTest()
    {
        C4Move move = CreateMove(2);
        string moveString = move.ToString();

        Assert.AreEqual("(2)", moveString);
    }

    /// <summary>
    /// Conveinience method used for creating moves
    /// Allows the creation of moves to be passed as a delegate to <see cref="Assert.Throws(System.Type, TestDelegate)"/>
    /// </summary>
    /// <param name="x">The x position of the move to create</param>
    private C4Move CreateMove(int x)
    {
        return new C4Move(x);
    }
}
