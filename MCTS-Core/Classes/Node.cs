using System;
using System.Collections.Generic;

namespace MCTS.Core
{
    /// <summary>
    /// A node for use with a Monte Carlo Search Tree <para/>
    /// Contains a game state, as well as stats such as Visits, score, Parent and Children
    /// </summary>
    public class Node
    {
        /// <summary>
        /// This nodes Parent node
        /// </summary>
        public Node Parent { get; private set; }

        /// <summary>
        /// The GameBoard state for this node
        /// </summary>
        public Board GameBoard { get; private set; }

        /// <summary>
        /// A list of all the Children of this node
        /// </summary>
        public List<Node> Children { get; private set; }

        /// <summary>
        /// The amount of times this node has been visited directly or via backpropagation
        /// </summary>
        public int Visits { get; private set; }

        /// <summary>
        /// The total score this node has as a result of direct simulation or backpropagation
        /// </summary>
        public float TotalScore { get; private set; }

        /// <summary>
        /// The Depth of this node in the game tree
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// A list of possible moves from this nodes board state that have not yet been expanded
        /// </summary>
        public List<Move> UnexpandedMoves { get; private set; }

        /// <summary>
        /// Creates a new node with the given board and Parent node
        /// </summary>
        /// <param name="ParentNode">The Parent of this node, null if this is the root node</param>
        /// <param name="board">The game board for this node</param>
        public void Initialise(Node ParentNode, Board board)
        {
            Parent = ParentNode;
            GameBoard = board;
            Children = new List<Node>(GameBoard.PossibleMoves().Count);
            UnexpandedMoves = new List<Move>(GameBoard.PossibleMoves());

            if (ParentNode == null)
            {
                Depth = 0;
            }
            else
            {
                Depth = ParentNode.Depth + 1;
            }
        }

        /// <summary>
        /// If there are still possible moves left for this node, create a child node with one of them played on it
        /// </summary>
        public Node Expand()
        {
            if (UnexpandedMoves.Count == 0 || IsLeafNode)
            {
                return this;
            }

            //Create a new child node
            Move move = (Move)UnexpandedMoves.PickRandom();

            Board newBoard = GameBoard.Duplicate();
            newBoard.MakeMove(move);

            //Create a child node with the same type as this node, initialise it, and add it to the list of Children
            Node child = (Node)Activator.CreateInstance(GetType());
            child.Initialise(this, newBoard);
            Children.Add(child);

            UnexpandedMoves.Remove(move);
            
            return child;
        }

        /// <summary>
        /// Updates the score and Visits values of this node and its Parents
        /// Used during backpropagation
        /// </summary>
        /// <param name="score">The score to update this node with</param>
        public void Update(float score)
        {
            //Update this nodes score
            TotalScore += score;

            //Increment the Visits attribute
            Visits++;
        }

        /// <summary>
        /// Gets the Upper Confidence Bound 1 value of this node
        /// </summary>
        /// <returns>The Upper Confidence Bound 1 value of this node</returns>
        public double UCBValue()
        {
            //If this node hasnt been explored, return the max value
            if (Visits == 0)
            {
                return double.MaxValue;
            }

            return AverageScore + Math.Sqrt(2 * Math.Log(Parent.Visits) / Visits);
        }

        /// <summary>
        /// Get the best child node of this node, used in calculating the best possible move from the point of this node <param/>
        /// The best child is the child with the highest amount of visits
        /// </summary>
        /// <returns></returns>
        public Node GetBestChild()
        {
            //If this node doesnt have any children, or it has children that haven't been simulated, then return null
            if (Visits <= 1)
            {
                return null;
            }

            //Calculate the best child of the current node, so that the most optimal choice can be made
            Node bestChild = null;
            float highestChildVisits = float.MinValue;

            foreach (Node child in Children)
            {
                if (child.Visits > highestChildVisits)
                {
                    bestChild = child;
                    highestChildVisits = bestChild.Visits;
                }
                else if (child.Visits == highestChildVisits)
                {
                    if (child.AverageScore > bestChild.AverageScore)
                    {
                        bestChild = child;
                    }
                }
            }

            return bestChild;
        }

        /// <summary>
        /// Signals if this node is a leaf node or not <para/>
        /// A node is a leaf node if its game has ended
        /// </summary>
        public bool IsLeafNode
        {
            get { return GameBoard.Winner != -1; }
        }

        /// <summary>
        /// The average score for this node <para/>
        /// Determined from the total score and number of Visits
        /// </summary>
        public float AverageScore
        {
            get { return (Visits == 0? 0 : TotalScore / Visits); }
        }
    }
}
