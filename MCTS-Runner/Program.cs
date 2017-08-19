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
        static void Main(string[] args)
        {
            MainController.Start(new C4Board(), 100);

            while (true)
            {
                Console.WriteLine("---------------------------------------");
                Console.WriteLine("-       Monte Carlo Tree Search       -");
                Console.WriteLine("-    Game: Connect 4    Threads: " + "1" + "    -");
                Console.WriteLine("-    Total nodes: " + MainController.TotalNodesVisited + " nodes");
                Console.WriteLine("-    Thread 1: " + MainController.TotalNodesVisited + " nodes");
                Console.WriteLine("--------------------------------------");


                Thread.Sleep(100);
                Console.Clear();
            }
        }
    }
}
