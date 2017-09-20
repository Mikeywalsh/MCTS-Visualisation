using System;

namespace MCTS.Core
{
    /// <summary>
    /// Runs Monte Carlo Tree Search on a given game board <para/>
    /// Every time <see cref="Step"/> is called, the algorithm performs one Select, Expand, Simulate and Backpropagate cycle <para/>
    /// The algorithm will run until the search space is exhausted <para/>
    /// A graceful exit can be achieved via <see cref="Finish"/>, which will result in an incomplete but still useful tree
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
        /// The amount of playouts each node undergoes during simulation
        /// </summary>
        public int PlayoutsPerSimulation { get; private set; }

        /// <summary>
        /// Creates a new Monte Carlo Tree Search with the given game board
        /// </summary>
        /// <param name="gameBoard">The game board to perform the MCTS with</param>
        /// <param name="playsPerSimulation">The amount of playouts to do for each simulation</param>
        public TreeSearch(Board gameBoard, int playsPerSimulation)
        {
            //Create the root node of the search tree using the provided node type
            Root = (T)Activator.CreateInstance(typeof(T));
            Root.Initialise(null, gameBoard);

            //Set the number of playouts to be done on each node during simulation
            PlayoutsPerSimulation = playsPerSimulation;

            //Simulate the root node to begin
            Simulation(Root, PlayoutsPerSimulation);
        }

        /// <summary>
        /// Perform a step of MCTS <para/>
        /// Selects the highest UCT value node, expands it, simulates its children and backpropagates the results up the tree
        /// </summary>
        public void Step()
        {
            Node bestNode = Selection(Root);

            if (bestNode == null)
            {
                Finished = true;
                return;
            }

            Expansion(bestNode);

            foreach (Node child in bestNode.Children)
            {
                if (!child.IsLeafNode)
                {
                    Simulation(child, PlayoutsPerSimulation);
                }
                Backprogation(child);
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
            else if (n.Children.Count == 0)
            {
                return n;
            }
            else
            {
                double highestUCB = float.MinValue;
                Node highestUCBChild = null;

                foreach (Node child in n.Children)
                {
                    if (!child.FullyExplored)
                    {
                        double currentUCB1 = child.UCBValue();
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
        /// The second step of MCTS <para/>
        /// A given node is expanded, creating children nodes containing possible plays which can be simulated in the next step
        /// </summary>
        /// <param name="n">The node to expand</param>
        private void Expansion(Node n)
        {
            n.CreateAllChildren();
        }

        /// <summary>
        /// The third step of MCTS <para/>
        /// Simulates a given number of random playouts from the given node to obtain an average score of the node <para/>
        /// The average score is then backpropogated up the tree to the root node
        /// </summary>
        /// <param name="n">The node to simulate the playout of</param>
        /// <param name="playoutAmount">The amount of playout simulations to run</param>
        private void Simulation(Node n, int playoutAmount)
        {
            n.StartSimulatePlayouts(playoutAmount);
        }

        /// <summary>
        /// The fourth and final step of MCTS <para/>
        /// Updates the given nodes parent hierarchy with its score value
        /// </summary>
        /// <param name="n">The child to update the hierachy of</param>
        private void Backprogation(Node n)
        {
            if (n.Parent != null)
            {
                n.Parent.Update(n.TotalScore, n.GameBoard.CurrentPlayer);
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

        /// <summary>
        /// The total number of nodes that have been visited so far
        /// </summary>
        public int NodesVisited
        {
            get { return Root.Visits; }
        }
    }
}
