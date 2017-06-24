using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    private Node parent;
    private Board gameBoard;

    /// <summary>
    /// A list of all the children of this node
    /// </summary>
    private List<Node> children;

    /// <summary>
    /// The amount of times this node has been visited directly or via backpropagation
    /// </summary>
    private int visits;

    /// <summary>
    /// The total score this node has as a result of direct simulation or backpropagation
    /// </summary>
    private float totalScore;

    private bool allChildrenFullyExplored;

    private int depth;

	public Node(Node parentNode, Board board)
    {
        parent = parentNode;
        gameBoard = board;
        children = new List<Node>(gameBoard.PossibleMoves().Count);

        if(IsLeafNode)
        {
            children.Capacity = 0;
            allChildrenFullyExplored = true;
            parentNode.CheckChildrenFullyExplored();

            //Since this is a leaf node, we can tell the score without any simulation just from looking at the Winner attribute on the game board
            totalScore = (gameBoard.Winner == gameBoard.PreviousPlayer ? 1 : 0);
            visits = 1;
        }

        if (parentNode == null)
        {
            depth = 0;
        }
        else
        {
            depth = parentNode.Depth + 1;
        }
    }

    public void CreateAllChildren()
    {
        if(IsLeafNode)
        {
            throw new InvalidNodeException("This node is a leaf node, cannot create children for it");
        }

        //Create a new child node for each possible move from this node
        foreach(Move move in gameBoard.PossibleMoves())
        {
            Board newBoard = gameBoard.Duplicate();
            newBoard.MakeMove(move);
            children.Add(new Node(this, newBoard));
        }
    }

    public void CheckChildrenFullyExplored()
    {
        if (allChildrenFullyExplored)
            return;

        foreach(Node child in children)
        {
            if (!child.AllChildrenFullyExplored)
                return;
        }

        allChildrenFullyExplored = true;

        if (parent != null)
        {
            parent.CheckChildrenFullyExplored();
        }
    }

    /// <summary>
    /// Simulates a number of playouts from this node and adds the mean score value to this nodes score attribute
    /// </summary>
    /// <param name="rand">The random instance used to ensure proper random number generation</param>
    /// <param name="playoutCount">The amount of simulations to run, a larger amount will give better results</param>
    public void SimulatePlayouts(System.Random rand, int playoutCount)
    {
        int wins = 0;
        int draws = 0;

        for (int i = 0; i < playoutCount; i++)
        {
            int winner = gameBoard.SimulateUntilEnd(rand);
            if (winner == gameBoard.PreviousPlayer)
                wins++;
            else if (winner == 0)
                draws++;
        }

        float simScore = (wins + (0.5f * draws)) / playoutCount;
        totalScore = simScore;
        visits = 1;
    }

    /// <summary>
    /// Updates the score and visits values of this node and its parents, recursively
    /// Used during backpropagation
    /// </summary>
    /// <param name="updateScore">The score to update this node with</param>
    public void Update(float updateScore, int player)
    {
        //Update this nodes score depending on the current player at this node
        if (gameBoard.CurrentPlayer == player)
        {
            totalScore += updateScore;
        }
        //else
        //{
        //    totalScore += (1 - updateScore);
        //}

        //Increment the visits attribute
        visits++;

        //Update this nodes parents with the new score
        if(parent != null)
        {
            parent.Update(updateScore, player);
        }
    }

    public bool AllChildrenFullyExplored
    {
        get { return allChildrenFullyExplored; }
    }

    /// <summary>
    /// Signals if this node is a leaf node or not
    /// A node is a leaf node if its game has ended
    /// </summary>
    public bool IsLeafNode
    {
        get { return gameBoard.Winner != -1; }
    }

    public Node Parent
    {
        get { return parent; }
    }

    public Board GameState
    {
        get { return gameBoard; }
    }

    public List<Node> Children
    {
        get { return children; }
    }

    public int Visits
    {
        get { return visits; }
    }

    public float TotalScore
    {
        get { return totalScore; }
    }

    public float AverageScore
    {
        get { return totalScore / visits; }
    }

    public int Depth
    {
        get { return depth; }
    }
}
