using System;
using System.Threading.Tasks;
using MCTS.Core;
using MCTS.Core.Games;
using System.Threading;

namespace MCTS.Runner
{
    /// <summary>
    /// A simple command line runner for MCTS <para/>
    /// Used for profiling purposes as Unity's profiler is not as good as Visual Studio's, and does not support thread profiling
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main TreeSearch instance, used to represent and explore the game tree
        /// </summary>
        private static TreeSearch<Node> mcts;

        /// <summary>
        /// The starting game board, chosen by the user
        /// </summary>
        private static Board startingBoard;

        /// <summary>
        /// Prompts the user to choose a gameboard, and then runs MCTS on the board
        /// </summary>
        static void Main(string[] args)
        {
            //Get the starting board
            startingBoard = GetStartingBoard();

            //Start the MCTS
            StartMCTS(startingBoard);

            //Update the display with information about the tree search every 0.1 seconds
            while (true)
            {
                Console.WriteLine("---------------------------------------");
                Console.WriteLine("-       Monte Carlo Tree Search       -");
                Console.WriteLine("-    Game: " + GetGameName(startingBoard) + "     Threads:" + "1" + "    -");
                Console.WriteLine("-    Total nodes: " + mcts.NodesVisited + " nodes");
                Console.WriteLine("--------------------------------------");

                //If the entire game tree has been created, stop refreshing the display and allow the user to exit
                if (mcts.Finished)
                {
                    Console.ReadLine();
                    break;
                }

                //Wait for 0.1 seconds before clearing the display
                Thread.Sleep(100);
                Console.Clear();
            }
        }

        /// <summary>
        /// Gets the name of the game that the provided board is from
        /// </summary>
        /// <param name="board">The board to get the game name of</param>
        /// <returns>The name of the game that the provided board is from</returns>
        private static string GetGameName(Board board)
        {
            if (board.GetType() == typeof(TTTBoard))
                return "Tic Tac Toe";
            else if (board.GetType() == typeof(C4Board))
                return "Connect 4";
            else if (board.GetType() == typeof(OthelloBoard))
                return "Othello";
            else
                return "INVALID BOARD";
        }

        /// <summary>
        /// Prompts the user to input their board choice and returns an instance of it
        /// </summary>
        /// <returns>An instance of the user specified board type</returns>
        private static Board GetStartingBoard()
        {
            while (true)
            {
                Console.WriteLine("What game would you like to run MCTS on?");
                Console.WriteLine("(1) Tic Tac Toe");
                Console.WriteLine("(2) Connect 4");
                Console.WriteLine("(3) Othello");
                Console.Write("Input your choice: ");

                string input = Console.ReadLine();

                Console.Clear();

                switch (input)
                {
                    case "1":
                        return new TTTBoard();
                    case "2":
                        return new C4Board();
                    case "3":
                        return new OthelloBoard();
                }
            }
        }

        /// <summary>
        /// Starts a new monte carlo tree search with the provided game board
        /// </summary>
        /// <param name="gameBoard">The starting gameboard to perform mcts from</param>
        public static void StartMCTS(Board gameBoard)
        {
            mcts = new TreeSearch<Node>(gameBoard);
            RunUntilEnd();
        }

        /// <summary>
        /// Runs MCTS until completion asyncronously and then disables the stop button
        /// </summary>
        /// <param name="m">The MCTS instance to run</param>
        static void RunUntilEnd()
        {
            Task.Factory.StartNew(() => { while (!mcts.Finished) { mcts.Step(); } }, TaskCreationOptions.LongRunning);
        }
    }
}
