namespace MCTS.Core
{
    /// <summary>
    /// A thread safe class which holds data about a simulation job <para/>
    /// Allows multiple threads to perform simulations and record the results, until a quota has been met
    /// </summary>
    public class SimulateData
    {
        /// <summary>
        /// A lock used 
        /// </summary>
        private object mutex = new object();

        /// <summary>
        /// The amount of plays so far
        /// </summary>
        public int Plays { get; private set; }

        /// <summary>
        /// The amount of wins so far
        /// </summary>
        public int Wins { get; private set; }

        /// <summary>
        /// The quota for the amount of simulations to run
        /// </summary>
        public int TargetPlays { get; private set; }

        /// <summary>
        /// Creates a new SimulateData instance with the given target simulation quota
        /// </summary>
        /// <param name="target">The target amount of simulations to run</param>
        public SimulateData(int target)
        {
            TargetPlays = target;
        }

        /// <summary>
        /// Records a simulation result, incrementing <see cref="Plays"/>, and <see cref="Wins"/> if the simulation resulted in a win <para/>
        /// If the target play quoto has been reached, then do nothing
        /// </summary>
        /// <param name="won">Did the simulation result in a victory?</param>
        public void AddResult(bool won)
        {
            lock (mutex)
            {
                if (Plays == TargetPlays)
                    return;

                Plays++;
                if (won)
                    Wins++;
            }
        }
    }
}
