using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// A flag indicating if this node and its Children, if it has any, have been fully explored <para/>
        /// Used to stop the MCTS algorithm from exploring exhausted nodes
        /// </summary>
        public bool FullyExplored { get; private set; }

        /// <summary>
        /// The Depth of this node in the game tree
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// The maximum amount of tasks that can be used during simulation
        /// </summary>
        private const int MAX_SIMULATION_TASKS = 4;

        /// <summary>
        /// The minimum amount of playouts needed before simulations will be ran in multi thread mode
        /// </summary>
        private const int MULTI_THREAD_MODE_PLAYOUT_REQUIREMENT = 50;

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

            if (IsLeafNode)
            {
                Children.Capacity = 0;

                //Since this is a leaf node, we know it has been fully explored upon creation
                FullyExplored = true;

                //Since this is a leaf node, we can tell the score without any simulation just from looking at the Winner attribute on the game board
                TotalScore = (GameBoard.Winner == GameBoard.PreviousPlayer ? 1 : 0);
                Visits = 1;
            }

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
        /// Creates a child for each possible move for this node and adds it to the list of Children
        /// </summary>
        public void CreateAllChildren()
        {
            if (IsLeafNode)
            {
                throw new InvalidNodeException("This node is a leaf node, cannot create Children for it");
            }

            //Create a new child node for each possible move from this node
            foreach (IMove move in GameBoard.PossibleMoves())
            {
                Board newBoard = GameBoard.Duplicate();
                newBoard.MakeMove(move);

                //Create a child node with the same type as this node, initialise it, and add it to the list of Children
                Node child = (Node)Activator.CreateInstance(GetType());
                child.Initialise(this, newBoard);
                Children.Add(child);
            }

            //Check if all Children have been explored for this node
            CheckFullyExplored();
        }

        /// <summary>
        /// Checks this nodes Children to see if they are all fully explored <para/>
        /// This is used so that during selection, the algorithm does not attempt to explore exhausted branches
        /// </summary>
        public void CheckFullyExplored()
        {
            if (FullyExplored)
                return;

            foreach (Node child in Children)
            {
                if (!child.FullyExplored)
                    return;
            }

            FullyExplored = true;

            if (Parent != null)
            {
                Parent.CheckFullyExplored();
            }
        }

        /// <summary>
        /// Simulates a number of playouts from this node and adds the mean score value to this nodes score attribute <para/>
        /// Uses <see cref="Task"/>'s to split the simulation workload across multiple threads
        /// </summary>
        /// <param name="playoutCount">The amount of simulations to run, a larger amount will give better results</param>
        public void StartSimulatePlayouts(int playoutCount)
        {
            int wins = 0;

            //Run simulations in single thread mode if the amount of playouts is below the multi thread requirement. Run in multi thread mode otherwise
            if (playoutCount < MULTI_THREAD_MODE_PLAYOUT_REQUIREMENT)
            {
                //Get the amount of wins for the simulation job
                wins = SimulatePlayouts(GameBoard, playoutCount);
            }
            else
            {
                //Create an array of tasks to use
                Task[] tasks = new Task[MAX_SIMULATION_TASKS];

                //Create a thread safe container for the simulation result data
                SimulateData sim = new SimulateData(playoutCount);

                //Split the amount of playouts to be simulated across multiple tasks
                for (int i = 0; i < MAX_SIMULATION_TASKS; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => SimulatePlayouts(GameBoard, sim));
                }

                //Ensure the other tasks have finished
                Task.WaitAll(tasks);

                //Get the amount of wins for the simulation job
                wins = sim.Wins;
            }

            //Calculate the total simulation score for this node
            float simScore = (float)wins / playoutCount;
            TotalScore = simScore;
            Visits = 1;
        }

        /// <summary>
        /// Used in multi thread mode <para/>
        /// For use with tasks to simulate a given amount of playouts for a board
        /// </summary>
        /// <param name="board">The board to perform the simulations on</param>
        /// <param name="sim">The thread safe simulate data holder which allows multiple threads to record their results at once</param>
        private static void SimulatePlayouts(Board board, SimulateData sim)
        {
            while (sim.Plays != sim.TargetPlays)
            {
                int winner = board.SimulateUntilEnd();
                sim.AddResult(winner == board.PreviousPlayer);
            }
        }

        /// <summary>
        /// Used in single thread mode <para/>
        /// Simulates an amount of playouts on a given board
        /// </summary>
        /// <param name="board">The board to perform the simulations on</param>
        /// <param name="playoutCount">The amount of simulations to run on this board</param>
        /// <returns>The sum of wins for the current player after simulating the board</returns>
        private static int SimulatePlayouts(Board board, int playoutCount)
        {
            int wins = 0;

            for (int i = 0; i < playoutCount; i++)
            {
                int winner = board.SimulateUntilEnd();
                if (winner == board.PreviousPlayer)
                    wins++;
            }

            return wins;
        }

        /// <summary>
        /// Updates the score and Visits values of this node and its Parents, recursively <para/>
        /// Used during backpropagation
        /// </summary>
        /// <param name="updateScore">The score to update this node with</param>
        /// <param name="player">The current player on the board at this node</param>
        public void Update(float updateScore, int player)
        {
            //Update this nodes score depending on the current player at this node
            if (GameBoard.CurrentPlayer == player)
            {
                TotalScore += updateScore;
            }
            else
            {
                TotalScore += (1 - updateScore);
            }

            //Increment the Visits attribute
            Visits++;

            //Update this nodes Parents with the new score
            if (Parent != null)
            {
                Parent.Update(updateScore, player);
            }
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
            get { return TotalScore / Visits; }
        }
    }
}
