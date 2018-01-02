using MCTS.Core;
using MCTS.Core.Games;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace MCTS.Visualisation.Play
{
    class GameServer : IDisposable
    {
        private TcpListener listener;
        private Socket sock;
        private C4Board gameBoard;
        private byte[] buffer;

        public C4Move LastClientMove { get; private set; }
        public C4Move MoveToMake { get; set; }

        /// <summary>
        /// Creates a GameServer instance and a <see cref="TcpListener"/> on the local IPv4 address of this machine and the provided port
        /// </summary>
        /// <param name="port">The port which the <see cref="TcpListener"/> will run on</param>
        public GameServer(short port)
        {
            //Obtain the IP address of this machine
            IPAddress[] allLocalIPs = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress localIP = IPAddress.Parse("127.0.0.1");

            //Find the IPv4 address of this machine
            foreach (IPAddress address in allLocalIPs)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = address;
                    break;
                }
            }

            //If this machine does not have an IPv4 address, throw an exception
            if (localIP.ToString() == "127.0.0.1")
            {
                throw new Exception("Could not find an IPv4 address for this machine. Are you connected to a network?");
            }

            //Create a listener on the IPv4 address and specified port
            listener = new TcpListener(localIP, port);

            //Initialise the buffer used to recieve information from the client
            buffer = new byte[1024];

            //Inform the user that the server has been started
            Debug.Log("Started running server...");
            Debug.Log("Local end point is: " + listener.LocalEndpoint);
        }

        /// <summary>
        /// Starts listening for a single TCP client connection<para/>
        /// When a client has connected, stop listening for new connections and start the game
        /// A main loop is entered which allows a client to play a full game against an opponent on the server machine
        /// </summary>
        public async void StartListening()
        {
            try
            {
                //Start listening
                listener.Start();
            }
            catch (SocketException)
            {
                Debug.Log("Listener already open, closing server...");
                listener = null;
                return;
            }

            //Start the listener and inform the user
            listener.Start();
            Debug.Log("Waiting for a connection...");

            //Start a task which listens for a new connection
            sock = await Task.Run(() => listener.AcceptSocket());

            //Only one connection is allowed at a time, stop the listener
            listener.Stop();

            //If the socket is null, something has went wrong
            if (sock == null)
            {
                throw new Exception("Socket was not established correctly!");
            }

            Debug.Log("Connected to client with endpoint: " + sock.RemoteEndPoint);

            StartGame();
        }

        /// <summary>
        /// Starts a game instance on the server, assuming a client is connected <para/>
        /// A main loop is entered which allows a client to play a full game against an opponent on the server machine
        /// </summary>
        async private void StartGame()
        {
            if(!Connected)
            {
                throw new Exception("Not connected to a client. Call StartListening first...");
            }

            //Create a new connect 4 game board
            gameBoard = new C4Board();

            Debug.Log("Started a game of connect 4!");

            //Send the initial board state to the client
            SendGameBoardToClient();

            try
            {
                //While the game is non-terminal, execute a main loop which sends the game state to the client and waits for a move to be returned
                while (gameBoard.Winner == -1)
                {
                    Debug.Log("Waiting for move selection from client...");

                    //Wait until data is recieved from the client
                    await Task.Run(() => sock.Receive(buffer));

                    //Deserialize the move to obtain a move object
                    C4Move toMake = (C4Move)Serializer.Deserialize(buffer);

                    //Clone the move to provide the server with a disposable version
                    LastClientMove = new C4Move(toMake.X);

                    //Output a string representation of the received move
                    Debug.Log("Move recieved:\n" + LastClientMove.ToString());

                    //Apply the move to the board
                    gameBoard.MakeMove(toMake);

                    //If this move ended the game, break out of the game loop
                    if (gameBoard.Winner != -1)
                        break;

                    //TEMP - MAKE A MOVE FOR THE OPPONENT
                    gameBoard.MakeMove(gameBoard.PossibleMoves().PickRandom());

                    //Serialize the current board state and send it to the client
                    await Task.Run(() => SendGameBoardToClient());
                }

                Debug.Log("Game over! Winner was player " + gameBoard.Winner);
            }
            catch (SocketException)
            {
                Debug.Log("Connection was closed by the client, exiting server...");
            }

            //Close the socket
            DisconnectClient();
            //StopListening();
        }

        private void SendGameBoardToClient()
        {
            if (sock == null)
            {
                //The client socket reference is null, something has went wrong
                throw new SocketException();
            }

            //Serialize the board
            byte[] serializedBoard = Serializer.Serialize(gameBoard);
            Debug.Log("Board serialized...\nLength:" + serializedBoard.Length.ToString());

            //Send the serialized initial board state to the client
            sock.Send(serializedBoard);
            Debug.Log("Board state sent...");
        }

        public void Dispose()
        {
            StopListening();
            DisconnectClient();
        }

        public void DisconnectClient()
        {
            if (sock.Connected)
            {
                sock.Close();
            }
        }

        public void StopListening()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

        public bool Listening
        {
            get { return listener != null; }
        }

        public bool Connected
        {
            get { return sock == null? false : sock.Connected; }
        }

        public bool GameStarted
        {
            get { return gameBoard != null; }
        }
    }
}
