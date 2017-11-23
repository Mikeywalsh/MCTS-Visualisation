using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MCTS.Core;
using MCTS.Core.Games;
using MCTS.Visualisation.Tree;
using System.Timers;
using System;

namespace MCTS.Visualisation
{
    /// <summary>
    /// Used to play a game of connect 4 against an opponent using MCTS
    /// This code is hacked together and is not thouroughly tested/of high quality
    /// </summary>
    public class PlayController : MonoBehaviour
    {
        private C4Board board;
        private TreeSearch<NodeObject> mcts;

        private float timeToRunFor;
        private float timeLeft;
        private bool gameOver;

        public GameObject moveButtons;
        public Text boardDisplayText;
        public Text winnerText;
        public Text aiTurnProgressText;

        private int currentIndex = 1;
        private DateTime startTime;
        private Timer stopTimer;

        void Start()
        {
            LineDraw.Lines = new List<ColoredLine>();
            Application.runInBackground = true;

            //Initialise the game board and display
            board = new C4Board();
            boardDisplayText.text = board.ToRichString();
        }

        void Update()
        {
            if (mcts != null)
            {
                //While the MCTS is still running, display progress information about the time remaining and the amounts of nodes created to the user
                if (!mcts.Finished)
                {
                    if (stopTimer != null)
                    {
                        aiTurnProgressText.text = mcts.UniqueNodes + " nodes       " + TimeLeft.Seconds.ToString() + "." + TimeLeft.Milliseconds.ToString("000") + "s/" + timeToRunFor + "s";
                    }

                    for(int targetIndex = mcts.AllNodes.Count; currentIndex < targetIndex & mcts.AllNodes.Count > 1; currentIndex++)
                    {
                        NodeObject currentNode = mcts.AllNodes[currentIndex];

                        if ((NodeObject)currentNode.Parent == null)
                        {
                            return;
                        }

                        currentNode.SetPosition(VisualisationType.Standard3D);
                        LineDraw.Lines.Add(new ColoredLine(currentNode.Position, ((NodeObject)currentNode.Parent).Position, LineDraw.lineColors[currentNode.Depth % LineDraw.lineColors.Length]));
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the amount of time left for the current timer <para/>
        /// If the current timer is null, return 0
        /// </summary>
        private TimeSpan TimeLeft
        {
            get
            {
                if(stopTimer == null)
                {
                    return new TimeSpan(0);
                }

                return (DateTime.Now - startTime);
            }
        }

        public void StartAITurn()
        {
            //Initialise MCTS on the given game board
            mcts = new TreeSearch<NodeObject>(board);

            //Run mcts
            RunMCTS();
        }

        private void EndAITurn(object sender, ElapsedEventArgs e)
        {
            mcts.Finish();
            stopTimer.Stop();
            stopTimer.Dispose();
            stopTimer = null;
        }

        /// <summary>
        /// Runs MCTS until completion
        /// Should be ran on another thread to avoid freezing of the application
        /// </summary>
        /// <param name="m">The <see cref="TreeSearch"/> instance to run</param>
        async void RunMCTS()
        {
            LineDraw.Lines = new List<ColoredLine>();
            currentIndex = 1;
            timeToRunFor = 3;
            startTime = DateTime.Now;
            stopTimer = new Timer(timeToRunFor * 1000);
            stopTimer.Elapsed += EndAITurn;
            stopTimer.Start();

            await Task.Factory.StartNew(() => { while (!mcts.Finished) { mcts.Step(); } });

            //If the search has finished, get the best child choice
            Node bestChild = mcts.Root.GetBestChild();

            //Determine what move was made on the best child
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    //If 2 cells don't match up, then the move was made in this cell
                    if (board.GetCell(x, y) != ((C4Board)bestChild.GameBoard).GetCell(x, y))
                    {
                        MakeMoveOnBoard(x);
                        mcts = null;
                        if (!gameOver)
                        {
                            moveButtons.SetActive(true);
                        }
                    }
                }
            }
        }

        public void MakeMoveOnBoard(int xPos)
        {
            if (board.CurrentPlayer == 2)
            {
                aiTurnProgressText.text = "";
            }

            board.MakeMove(new C4Move(xPos));
            boardDisplayText.text = board.ToRichString();

            if (board.Winner != -1)
            {
                if (board.Winner == 0)
                {
                    winnerText.text = "DRAW";
                }
                else
                {
                    winnerText.text = "WINNER IS PLAYER " + board.Winner;
                }
                gameOver = true;
                return;
            }

            if (board.CurrentPlayer == 2)
            {
                moveButtons.SetActive(false);
                StartAITurn();
            }
        }

        /// <summary>
        /// Called when the back to menu button is pressed <para/>
        /// Changes the current scene to be the main menu
        /// </summary>
        public void BackToMenuButtonPressed()
        {
            SceneController.LoadMainMenu();
        }

        /// <summary>
        /// Called when the reset button is pressed <para/>
        /// Reloads the current scene
        /// </summary>
        public void ResetButtonPressed()
        {
            SceneController.ResetCurrentScene();
        }
    }
}