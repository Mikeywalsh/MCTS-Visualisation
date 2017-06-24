using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCTS {

    private System.Random rand = new System.Random();
    private Node root;
    private bool finished;

    public MCTS()
    {
        root = new Node(null, new TTTBoard());

        Simulation(root, 100);
    }

    public void Step()
    {
        Node bestNode = Selection(root);
        if (bestNode == null)
        {
            Debug.Log("Finished!");
            finished = true;
            return;
        }

        Expansion(bestNode);

        int count = 0;
        foreach (Node child in bestNode.Children)
        {
            count++;
            if (!child.IsLeafNode)
            {
                Simulation(child, 100);
            }
            Backprogation(child);
        }
        //finished = true;
        //Debug.Log(Plays);
    }

    /// <summary>
    /// The first step of MCTS
    /// The tree is searched recursively using the Upper Confidence Bound 1 calculation to select the best node to expand
    /// </summary>
    /// <param name="n">The current root node in the search</param>
    /// <returns>The best node to expand, obtained using Upper Confidence Bound 1</returns>
    private Node Selection(Node n)
    {
        //Debug.Log("Selection depth: " + n.Depth);
        if(n == null)
        {
            return null;
        }
        else if(n.Children.Count == 0)
        {
            return n;
        }
        else
        {
            float highestUCB = float.MinValue;
            Node highestUCBChild = null;

            foreach(Node child in n.Children)
            {
                if (!child.AllChildrenFullyExplored)
                {
                    float currentUCB1 = UCB1(child, Plays);
                    if (currentUCB1 > highestUCB)
                    {
                        highestUCB = currentUCB1;
                        highestUCBChild = child;
                    }
                }
            }

            return Selection(highestUCBChild);
        }
    }

    /// <summary>
    /// The second step of MCTS
    /// A given node is expanded, creating children nodes containing possible plays which can be simulated in the next step
    /// </summary>
    /// <param name="n">The node to expand</param>
    private void Expansion(Node n)
    {
        n.CreateAllChildren();
    }

    /// <summary>
    /// The third step of MCTS
    /// Simulates a given number of random playouts from the given node to obtain an average score of the node
    /// The average score is then backpropogated up the tree to the root node
    /// </summary>
    /// <param name="n">The node to simulate the playout of</param>
    /// <param name="playoutAmount">The amount of playout simulations to run</param>
    private void Simulation(Node n, int playoutAmount)
    {
        n.SimulatePlayouts(rand, playoutAmount);
    }
    
    /// <summary>
    /// The fourth and final step of MCTS
    /// Updates the given nodes parent hierarchy with its score value
    /// </summary>
    /// <param name="n">The child to update the hierachy of</param>
    private void Backprogation(Node n)
    {
        if (n.Parent != null)
        {
            n.Parent.Update(n.AverageScore, n.GameState.CurrentPlayer);
        }
    }

    /// <summary>
    /// Gets the Upper Confidence Bound 1 value of a given node
    /// </summary>
    /// <param name="n">The node to get the value of</param>
    /// <param name="totalPlays">The total amount of plays in the game so far</param>
    /// <returns>The Upper Confidence Bound 1 value of the given node</returns>
    private float UCB1(Node n, int totalPlays)
    {
        return n.AverageScore + Mathf.Sqrt((2 * Mathf.Log(totalPlays)) / n.Visits);
    }

    public Node BestNodeChoice(Node n)
    {
        float smallestScore = float.MinValue;
        Node chosenNode = null;

        foreach (Node child in n.Children)
        {
            //Choose the child with the best score, if multiple children have the best score, then choose the one with the least children
            if(child.AverageScore > smallestScore || child.AverageScore == smallestScore && chosenNode.Children.Count > child.Children.Count)
            {
                smallestScore = child.AverageScore;
                chosenNode = child;
            }
        }

        return chosenNode;
    }

    /// <summary>
    /// The root node of the Monte Carlo Tree Search
    /// </summary>
    public Node Root
    {
        get { return root; }
    }

    /// <summary>
    /// Signals if the Monte Carlo Tree Search has finished
    /// Can be flagged as a result of all possible nodes being generated or a graceful exit
    /// </summary>
    public bool Finished
    {
        get { return finished; }
    }

    /// <summary>
    /// The number of total nodes that have been simulated so far
    /// </summary>
    public int Plays
    {
        get { return root.Visits; }
    }
}
