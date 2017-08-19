using System.Threading;

namespace MCTS.Core
{
    public class Worker
    {
        private Thread workerThread;
        private TreeSearch<Node> mcts;

        public int Steps { get; private set; }

        public Worker(TreeSearch<Node> tree)
        {
            mcts = tree;
            workerThread = new Thread(DoWork);
            workerThread.Start();
        }

        private void DoWork()
        {
            while (true)
            {
                mcts.Step();
                Steps++;
            }
        }

        public int ThreadID
        {
            get { return workerThread.ManagedThreadId; }
        }
    }
}
