using System.Threading.Tasks;
using MCTS.Core;

namespace MCTS.Runner
{
    static class MainController
    {
        private static TreeSearch<Node> mcts;

        public static void Start(Board gameBoard, int playoutCount)
        {
            mcts = new TreeSearch<Node>(gameBoard, playoutCount);
            //RunUntilEnd();
        }

        public static int TotalNodesVisited
        {
            get { return mcts.NodesVisited; }
        }

        public static bool Finished
        {
            get { return mcts.Finished; }
        }

        /// <summary>
        /// Runs MCTS until completion asyncronously and then disables the stop button
        /// </summary>
        /// <param name="m">The MCTS instance to run</param>
        static void RunUntilEnd()
        {
            //Task.Factory.StartNew(() => { while (!mcts.Finished) { mcts.Step(); } }, TaskCreationOptions.LongRunning);
        }
    }
}
