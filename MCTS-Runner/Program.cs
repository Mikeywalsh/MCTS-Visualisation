using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTS.Core;
using System.Threading;

namespace MCTS.Runner
{
    class Program
    {
        private static Board startingBoard;

        static void Main(string[] args)
        {
            //Get the starting board
            startingBoard = GetStartingBoard();

            //Start the MCTS
            MainController.Start(startingBoard, 100);

            //Update the display with information about the tree search every 0.1 seconds
            while (true)
            {
                Console.WriteLine("---------------------------------------");
                Console.WriteLine("-       Monte Carlo Tree Search       -");
                Console.WriteLine("-    Game: " + GetGameName(startingBoard) + "     Threads:" + "1" + "    -");
                Console.WriteLine("-    Total nodes: " + MainController.TotalNodesVisited + " nodes");
                Console.WriteLine("-    Thread 1: " + MainController.TotalNodesVisited + " nodes");
                Console.WriteLine("--------------------------------------");

                //If the entire game tree has been created, stop refreshing the display and allow the user to exit
                if (MainController.Finished)
                {
                    Console.ReadLine();
                    break;
                }

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
                Console.Write("Input your choice: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        return new TTTBoard();
                    case "2":
                        return new C4Board();
                }

                Console.Clear();
            }
        }
    }
}
