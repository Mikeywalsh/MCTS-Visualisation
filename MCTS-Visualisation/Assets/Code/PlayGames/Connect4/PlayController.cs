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
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

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

        private bool client = false;

        private bool waiting = false;

        void Start()
        {
            if(client)
            {
                StartClient();

            }
            else
            {
                StartServer();
            }
            
            LineDraw.Lines = new List<ColoredLine>();
            Application.runInBackground = true;

            //Initialise the game board and display
            board = new C4Board();
            boardDisplayText.text = board.ToRichString();
        }

        async void StartServer()
        {
            //Obtain the IP address of this machine
            IPAddress[] allLocalIPs = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress localIP = IPAddress.Parse("127.0.0.1");

            foreach(IPAddress address in allLocalIPs)
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = address;
                }
            }

            if(localIP.ToString() == "127.0.0.1")
            {
                throw new Exception("Could not find an IPv4 address for this machine. Are you connected to a network?");
            }

            //Initialise a listener
            TcpListener listener = new TcpListener(localIP, 8500);

            //Start listening
            listener.Start();

            Debug.Log("Started running server...");
            Debug.Log("Local end point is: " + listener.LocalEndpoint);
            Debug.Log("Waiting for a connection...");

            //Create a socket reference
            Socket sock = null;

            //Start a task which listens for a new connection
            await Task.Factory.StartNew(() => { sock = listener.AcceptSocket(); });

            if(sock == null)
            {
                throw new Exception("Socket was not established correctly!");
            }

            //Initialise a buffer and recieved byte count
            byte[] buffer = new byte[1000];
            int recievedByteCount = 0;
            
            //Wait until data is recieved from the client
            await Task.Factory.StartNew(() => { sock.Receive(buffer); });

            //Create a byte array that will hold the serialized board
            byte[] serializedBoard = new byte[recievedByteCount];

            //Copy the contents of the buffer to the serialized board byte array
            for(int i = 0; i < recievedByteCount; i++)
            {
                serializedBoard[i] = buffer[i];
            }

            //Deserialize the board to obtain a board object
            C4Board newBoard = Deserialize(serializedBoard);

            //Send a PLACEHOLDER confirmation message
            sock.Send(new ASCIIEncoding().GetBytes("Board state recieved: " + newBoard.ToString()));
            sock.Close();
            listener.Stop();
        }

        async void StartClient()
        {
            Debug.Log("Starting Client...");

            TcpClient client = new TcpClient();

            Debug.Log("Client Initialised...");
            await Task.Factory.StartNew(() => { client.Connect("10.240.107.219", 8500); });

            Debug.Log("Connection Established...");

            C4Board newBoard = new C4Board();
            newBoard.MakeMove(new C4Move(2));
            newBoard.MakeMove(new C4Move(1));
            newBoard.MakeMove(new C4Move(3));
            newBoard.MakeMove(new C4Move(5));
            newBoard.MakeMove(new C4Move(0));
            newBoard.MakeMove(new C4Move(2));
            newBoard.MakeMove(new C4Move(2));

            Stream stream = client.GetStream();

            byte[] serializedBoard = SerializeBoard(newBoard);

            stream.Write(serializedBoard, 0, serializedBoard.Length);

            byte[] reply = new byte[100];
            int replyLength = 0;

            await Task.Factory.StartNew(() => { replyLength = stream.Read(reply, 0, 100); });

            string replyMessage = "";

            for(int i = 0; i < replyLength; i++)
            {
                replyMessage += Convert.ToChar(reply[i]);
            }

            Debug.Log(replyMessage);

            client.Close();


        }

        public static byte[] SerializeBoard(C4Board board)
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, board);
                return memoryStream.ToArray();
            }
        }

        public static C4Board Deserialize(byte[] serializedBoard)
        {
            using (var memoryStream = new MemoryStream(serializedBoard))
            {
                return (C4Board)(new BinaryFormatter().Deserialize(memoryStream));
            }
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

            //Get the move made on the best child and apply it to the main game board
            MakeMoveOnBoard(((C4Move)bestChild.GameBoard.LastMoveMade).X);
            mcts = null;
            if (!gameOver)
            {
                moveButtons.SetActive(true);
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