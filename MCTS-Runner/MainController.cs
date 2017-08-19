using System.Threading.Tasks;
using MCTS.Core;

namespace MCTS.Runner
{
    static class MainController
    {
        private static TreeSearch<Node> mcts;

        public static void Start(Board gameBoard, int playoutCount)
        {
            
        }

        /// <summary>
        /// Runs MCTS until completion asyncronously and then disables the stop button
        /// </summary>
        /// <param name="m">The MCTS instance to run</param>
        static void RunMCTS(TreeSearch<Node> mcts)
        {
            Task.Factory.StartNew(() => { while (!mcts.Finished) { mcts.Step(); } }, TaskCreationOptions.LongRunning);
        }
    }
}
