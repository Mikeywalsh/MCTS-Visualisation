using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MCTS.Core;
using MCTS.Core.Games;
using MCTS.Visualisation.Tree;
using System.Timers;
using System;
using System.Net;

namespace MCTS.Visualisation.Play
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

        public GameObject BoardDisplay;
        public GameObject moveButtons;
        public GameObject ResetButton;

        public GameObject MenuPanel;
        public GameObject ServerWaitingPanel;
        public GameObject ClientWaitingPanel;

        public Text ConnectingText;
        public Text ServerWaitingText;
        public Text winnerText;
        public Text aiTurnProgressText;

        public C4BoardModelController modelController;

        public InputField InputIPAddress;
        public InputField InputPort;

        public Button ConnectButton;

        private int currentIndex = 1;
        private DateTime startTime;
        private Timer stopTimer;

        private PlayMode playMode;

        private GameServer server;
        private GameClient client;

        void Start()
        {
            //Initialise LineDraw and enable background running
            LineDraw.Lines = new List<ColoredLine>();
            Application.runInBackground = true;

            //Initialise the game board and display
            board = new C4Board();
            modelController.Initialise();
            modelController.SetBoard(board);
        }

        public void StartLocal()
        {
            //Initialise LineDraw
            LineDraw.Lines = new List<ColoredLine>();

            //Initialise the correct UI elements
            ResetButton.SetActive(true);
            moveButtons.SetActive(true);
            BoardDisplay.SetActive(true);

            //Set the play mode
            playMode = PlayMode.LOCAL;

            //Hide the menu panel
            MenuPanel.SetActive(false);
        }

		public void StartServer()
		{
			//Initialise LineDraw
			LineDraw.Lines = new List<ColoredLine>();

			//Initialise the game server
			server = new GameServer(8500, ResetButtonPressed, Connected, MakeMoveOnBoard, ResetButtonPressed, ResetGame)
			{
				GameBoard = board
			};
            server.StartListening();

            //Set the play mode
            playMode = PlayMode.SERVER;

            //Hide the menu panel and show the server waiting panel
            MenuPanel.SetActive(false);
            ServerWaitingPanel.SetActive(true);
            ServerWaitingText.text = string.Format("Waiting for client connection...\nIP: {0}\nPort: {1}", server.ServerAddress, server.ServerPort);
        }

        public void StartClient()
        {
            //Initialise the game client
            client = new GameClient(Connected, ClientConnectionFailed, MakeMoveOnBoard, ResetButtonPressed);

            //Set the play mode
            playMode = PlayMode.CLIENT;

            //Hide the menu panel and show the client waiting panel
            MenuPanel.SetActive(false);
            ClientWaitingPanel.SetActive(true);
        }

        public void AttemptClientConnection()
        {
            //Check the validity of the input IP and port
            IPAddress serverIP;
            short serverPort;

            if (IPAddress.TryParse(InputIPAddress.text, out serverIP) && short.TryParse(InputPort.text, out serverPort))
            {
                //Disable the connect button so that the user has to wait while a connection is attempted
                ConnectButton.gameObject.SetActive(false);
                ConnectingText.gameObject.SetActive(true);

                //Attempt a connection with the input IP and port
                client.AttemptConnect(IPAddress.Parse(InputIPAddress.text), short.Parse(InputPort.text));
            }
        }

        private void ClientConnectionFailed()
        {
            //Re-enable the connect button to allow the user to try to connect again
            Debug.Log("Could not connect to server, please try again...");

            //Only do this if the user hasn't left the scene
            if (ConnectButton != null && ConnectingText != null)
            {
                ConnectButton.gameObject.SetActive(true);
                ConnectingText.gameObject.SetActive(false);
            }
        }

        private void Connected()
        {
            ServerWaitingPanel.SetActive(false);
            ClientWaitingPanel.SetActive(false);
            BoardDisplay.SetActive(true);

            if(playMode == PlayMode.CLIENT)
            {
                moveButtons.SetActive(true);
            }
        }

		private void ResetGame()
		{
			board = new C4Board();
			server.GameBoard = board;
		}

		void Update()
        {
            if (mcts != null)
            {
                //While the MCTS is still running, display progress information about the time remaining and the amounts of nodes created to the user
                if (!mcts.Finished)
                {
                    //Display the amount of nodes created so far if the timer is still running
                    if (stopTimer != null)
                    {
                        aiTurnProgressText.text = mcts.UniqueNodes + " nodes       " + TimeLeft.Seconds.ToString() + "." + TimeLeft.Milliseconds.ToString("000") + "s/" + timeToRunFor + "s";
                    }

                    //Add new nodes to the LineDraw Lines array, so that they can be represented graphically
                    for (int targetIndex = mcts.AllNodes.Count; currentIndex < targetIndex & mcts.AllNodes.Count > 1; currentIndex++)
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
                if (stopTimer == null)
                {
                    return new TimeSpan(0);
                }

                return (DateTime.Now - startTime);
            }
        }

        /// <summary>
        /// Starts the AI turn, creating an mcts instance and running it
        /// </summary>
        public void StartAITurn()
        {
            //Initialise MCTS on the given game board
            mcts = new TreeSearch<NodeObject>(board);

            //Run mcts
            RunMCTS();
        }

        /// <summary>
        /// Called when the <see cref="stopTimer"/> timer for the AI has ran out <para/>
        /// End the turn and stop and dispose of the timer
        /// </summary>
        private void EndAITurn(object sender, ElapsedEventArgs e)
        {
            mcts.Finish();
            stopTimer.Stop();
            stopTimer.Dispose();
            stopTimer = null;
        }

        /// <summary>
        /// Runs MCTS until completion asynchronously
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

            //Get the move made on the best child and apply it to the main game board
            MakeMoveOnBoard((C4Move)bestChild.GameBoard.LastMoveMade);

            //If in server mode, pass the move to the server so that it can be serialized and sent to the client
            if(playMode == PlayMode.SERVER && server.Connected)
            {
                server.ServerMove = bestChild.GameBoard.LastMoveMade;
            }            

            //Set the TreeSearch reference to null to allow it to be garbage collected
            mcts = null;

            //If the game has not finished and is being played in local mode, then activate the move buttons
            if (!gameOver && playMode == PlayMode.LOCAL)
            {
                moveButtons.SetActive(true);
            }
        }

        /// <summary>
        /// Called when a user is playing the game using the provided on-screen buttons
        /// </summary>
        /// <param name="xPos">The x position to make the move in</param>
        public void MakeMoveOnBoard(int xPos)
        {
            Move toMake = new C4Move(xPos);

            MakeMoveOnBoard(toMake);

            //If in client mode, pass the move to the client so that it can be serialized and sent to the server
            if (playMode == PlayMode.CLIENT && client.Connected)
            {
                client.ClientMove = toMake;
                aiTurnProgressText.text = "Server is thinking...";
            }
        }

        /// <summary>
        /// Makes a move on the current board
        /// </summary>
        /// <param name="move">The move to make</param>
        private void MakeMoveOnBoard(Move move)
        {
            if (board.CurrentPlayer == 2)
            {
                aiTurnProgressText.text = "";
            }

            board.MakeMove(move);
            modelController.SetBoard(board);

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

            if(board.CurrentPlayer == 1 && playMode == PlayMode.CLIENT)
            {
                moveButtons.SetActive(true);
            }
            if (board.CurrentPlayer == 2)
            {
                moveButtons.SetActive(false);

                //If the current playmode is client, don't start an AI turn, wait for server response instead
                if (playMode != PlayMode.CLIENT)
                {
                    StartAITurn();
                }
            }            
        }

        /// <summary>
        /// Called when the back to menu button is pressed <para/>
        /// Changes the current scene to be the main menu
        /// </summary>
        public void BackToMenuButtonPressed()
        {
            if(server != null)
            {
                server.Dispose();
            }

            if(client != null)
            {
                client.Dispose();
            }

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

    enum PlayMode
    {
        LOCAL = 0,
        SERVER,
        CLIENT
    }
}