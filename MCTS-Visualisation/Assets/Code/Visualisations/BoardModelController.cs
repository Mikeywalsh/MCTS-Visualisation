using UnityEngine;
using MCTS.Core;

namespace MCTS.Visualisation
{
    public abstract class BoardModelController : MonoBehaviour
    {
        /// <summary>
        /// An array of gameobjects which represent pieces on the game board display
        /// </summary>
        protected GameObject[,] board;

        /// <summary>
        /// Performs initialisation logic on this board model
        /// </summary>
        public abstract void Initialise();

        /// <summary>
        /// Sets the pieces of this board model to represent the given board state
        /// </summary>
        /// <param name="boardToSet">The board state to represent</param>
        public abstract void SetBoard(GridBasedBoard boardToSet);
    }
}