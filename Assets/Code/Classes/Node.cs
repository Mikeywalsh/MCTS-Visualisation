using System;
using System.Collections.Generic;

/// <summary>
/// A node for use with a Monte Carlo Search Tree
/// Contains a game state, as well as stats such as visits, score, parent and children
/// </summary>
public class Node {

    /// <summary>
    /// This nodes parent node
    /// </summary>
    private Node parent;

    /// <summary>
    /// The gameboard state for this node
    /// </summary>
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

    /// <summary>
    /// Signals if all this nodes children have been fully explored
    /// Used to stop the MCTS algorithm from exploring exhausted nodes
    /// </summary>
    private bool allChildrenFullyExplored;

    /// <summary>
    /// The depth of this node in the game tree
    /// </summary>
    private int depth;

    /// <summary>
    /// Creates a new node with the given board and parent node
    /// </summary>
    /// <param name="parentNode">The parent of this node, null if this is the root node</param>
    /// <param name="board">The game board for this node</param>
	public void Initialise(Node parentNode, Board board)
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

    /// <summary>
    /// Creates a child for each possible move for this node and adds it to the list of children
    /// </summary>
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

            //Create a child node with the same type as this node, initialise it, and add it to the list of children
            Node child = (Node)Activator.CreateInstance(GetType());
            child.Initialise(this, newBoard);
            children.Add(child);
        }
    }

    /// <summary>
    /// Checks this nodes children to see if they are all fully explored
    /// This is used so that during selection, the algorithm does not attempt to explore exhausted branches
    /// </summary>
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

        for (int i = 0; i < playoutCount; i++)
        {
            int winner = gameBoard.SimulateUntilEnd(rand);
            if (winner == gameBoard.PreviousPlayer)
                wins++;
        }

        float simScore = (float)wins / playoutCount;
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
        else
        {
            totalScore += (1 - updateScore);
        }

        //Increment the visits attribute
        visits++;

        //Update this nodes parents with the new score
        if(parent != null)
        {
            parent.Update(updateScore, player);
        }
    }

    /// <summary>
    /// Signals if all this nodes children have been fully explored
    /// Used to stop the MCTS algorithm from exploring exhausted nodes
    /// </summary>
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

    /// <summary>
    /// This nodes parent node
    /// </summary>
    public Node Parent
    {
        get { return parent; }
    }

    /// <summary>
    /// The gameboard state for this node
    /// </summary>
    public Board GameBoard
    {
        get { return gameBoard; }
    }

    /// <summary>
    /// A list of all this nodes children nodes
    /// </summary>
    public List<Node> Children
    {
        get { return children; }
    }

    /// <summary>
    /// How many times this node has been visited
    /// </summary>
    public int Visits
    {
        get { return visits; }
    }

    /// <summary>
    /// The total score for this node as a result of simulation and backpropagation
    /// </summary>
    public float TotalScore
    {
        get { return totalScore; }
    }

    /// <summary>
    /// The average score for this node
    /// Determined from the total score and number of visits
    /// </summary>
    public float AverageScore
    {
        get { return totalScore / visits; }
    }

    /// <summary>
    /// The depth of this node in the game tree
    /// </summary>
    public int Depth
    {
        get { return depth; }
    }
}
