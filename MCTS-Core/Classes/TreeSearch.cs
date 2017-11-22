using System;
using System.Collections.Generic;

namespace MCTS.Core
{
    /// <summary>
    /// Runs Monte Carlo Tree Search on a given game board <para/>
    /// Every time <see cref="Step"/> is called, the algorithm performs one Select, Expand, Simulate and Backpropagate cycle <para/>
    /// The algorithm will run until the <see cref="Finish"/> method is called
    /// </summary>
    /// <typeparam name="T">The type of node to use for the tree search</typeparam>
    public class TreeSearch<T> where T : Node
    {
        /// <summary>
        /// The root node of the search tree
        /// </summary>
        public T Root { get; private set; }

        /// <summary>
        /// Signals if the MCTS algorithm has finished running
        /// </summary>
        public bool Finished { get; private set; }

        /// <summary>
        /// The amount of unique nodes in the tree
        /// </summary>
        public int UniqueNodes { get; private set; }

        /// <summary>
        /// A list of all nodes in the tree in the order that they were added
        /// </summary>
        public List<T> AllNodes { get; private set; }

        /// <summary>
        /// Creates a new Monte Carlo Tree Search with the given game board
        /// </summary>
        /// <param name="gameBoard">The game board to perform the MCTS with</param>
        public TreeSearch(Board gameBoard)
        { 
            //Create the root node of the search tree using the provided node type
            Root = (T)Activator.CreateInstance(typeof(T));
            Root.Initialise(null, gameBoard);

            //Set the unique node amount to 1, to accomodate the root node
            UniqueNodes = 1;

            //Initialise the list of all nodes and add the root node to it
            AllNodes = new List<T>();
            AllNodes.Add(Root);
        }

        /// <summary>
        /// Perform a step of MCTS <para/>
        /// Selects the highest UCT value node, expands it, simulates its children and backpropagates the results up the tree
        /// </summary>
        public void Step()
        {
            //Selection
            Node currentNode = Selection(Root);

            if (currentNode == null)
            {
                throw new InvalidNodeException("Something has went wrong during selection. Null node returned.");
            }

            //Expansion
            Node nodeBeforeExpansion = currentNode;
            currentNode = currentNode.Expand();

            if (nodeBeforeExpansion != currentNode)
            {
                UniqueNodes++;
                AllNodes.Add((T)currentNode);
            }

            //Simulation
            Board resultState = currentNode.GameBoard.SimulateUntilEnd();

            //Backpropogation
            while (currentNode != null)
            {
                currentNode.Update(resultState.GetScore(currentNode.GameBoard.PreviousPlayer));
                currentNode = currentNode.Parent;
            }
        }

        /// <summary>
        /// The first step of MCTS <para/>
        /// The tree is searched recursively using the Upper Confidence Bound 1 calculation to select the best node to expand
        /// </summary>
        /// <param name="n">The current root node in the search</param>
        /// <returns>The best node to expand, obtained using Upper Confidence Bound 1</returns>
        private Node Selection(Node n)
        {
            if (n == null)
            {
                return null;
            }
            else if (n.Children.Count == 0 || n.UnexpandedMoves.Count != 0)
            {
                return n;
            }
            else
            {
                double highestUCB = float.MinValue;
                Node highestUCBChild = null;

                foreach (Node child in n.Children)
                {
                    double currentUCB1 = child.UCBValue();
                    if (currentUCB1 > highestUCB)
                    {
                        highestUCB = currentUCB1;
                        highestUCBChild = child;
                    }
                }

                return Selection(highestUCBChild);
            }
        }

        /// <summary>
        /// Given a root node, will choose a child which maximises reward, based on the game tree constructed so far
        /// </summary>
        /// <param name="n">The root node to choose the best child of</param>
        /// <returns>The best child of the given root node</returns>
        public Node BestNodeChoice(Node n)
        {
            float smallestScore = float.MinValue;
            Node chosenNode = null;

            foreach (Node child in n.Children)
            {
                //Choose the child with the best score, if multiple children have the best score, then choose the one with the least children
                if (child.AverageScore > smallestScore || child.AverageScore == smallestScore && chosenNode.Children.Count > child.Children.Count)
                {
                    smallestScore = child.AverageScore;
                    chosenNode = child;
                }
            }

            return chosenNode;
        }

        /// <summary>
        /// Can be called to gracefully halt execution of the algorithm
        /// </summary>
        public void Finish()
        {
            Finished = true;
        }
    }
}
