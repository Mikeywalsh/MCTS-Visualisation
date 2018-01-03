using MCTS.Core;
using MCTS.Core.Games;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace MCTS.Visualisation.Play
{
    /// <summary>
    /// A game server, which allows a client to connect and a game to be played between client and server
    /// </summary>
    class GameServer : IDisposable
    {
        /// <summary>
        /// The <see cref="TcpListener"/> used to listen for incoming client connections <para/>
        /// This listener will stop listening after one connection, as only one client may be connected at a time
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// The socket used to communicate with the client
        /// </summary>
        private Socket sock;

        /// <summary>
        /// A data buffer used for communications with the client
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// A callback which will be called whenever a move is recieved from the client
        /// </summary>
        private Action<Move> moveCallback;

        /// <summary>
        /// A callback which will be called if the connection between the server and client is ended
        /// </summary>
        private Action disconnectCallback;

        /// <summary>
        /// The <see cref="Board"/> reference used by this GameServer to play the game out on
        /// </summary>
        public Board GameBoard { get; private set; }

        /// <summary>
        /// A move which can be set directly <para/>
        /// When this move is set, it is serialized and sent to the client, and then set to be null
        /// </summary>
        public Move ServerMove { get; set; }

        /// <summary>
        /// Creates a GameServer instance and a <see cref="TcpListener"/> on the local IPv4 address of this machine and the provided port
        /// </summary>
        /// <param name =board">The board reference that the server will use</param>
        /// <param name="port">The port which the <see cref="TcpListener"/> will run on</param>
        /// <param name="mCallback">The callback method to call when a move is made</param>
        /// <param name="dCallback">The callback method to call when a client has disconnection</param>
        public GameServer(Board board, short port, Action<Move> mCallback, Action dCallback)
        {
            //Set the server board reference
            GameBoard = board;

            //Set the callbacks
            moveCallback = mCallback;
            disconnectCallback = dCallback;

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

            //Enter the update loop
            UpdateLoop();
        }

        /// <summary>
        /// A main loop is entered which allows a client to play a full game against an opponent on the server machine
        /// </summary>
        async private void UpdateLoop()
        {
            if(!Connected)
            {
                throw new Exception("Not connected to a client. Call StartListening first...");
            }

            try
            {
                //Execute a main loop which waits for a client board to be received before sending a server response, until one of the boards is terminal
                while (true)
                {
                    Debug.Log("Waiting for move selection from client...");

                    //Wait until data is recieved from the client
                    await Task.Run(() => sock.Receive(buffer));

                    //Deserialize the move to obtain a move object
                    Move clientMove = (C4Move)Serializer.Deserialize(buffer);

                    //Output a string representation of the received move
                    Debug.Log("Client move recieved:\n" + clientMove.ToString());

                    //Make the client move on the game board, if it is terminal then break out of the main loop
                    moveCallback(clientMove);

                    if (GameBoard.Winner != -1)
                    {
                        Debug.Log("Game over! " + (GameBoard.Winner == 0 ? "Draw!" : "Winner was player " + GameBoard.Winner));
                        break;
                    }

                    //Wait for the server machine to create a resultant board
                    while(ServerMove == null)
                    {
                        await Task.Delay(500);
                    }

                    //Serialize the server move and send it to the client
                    byte[] serializedMove = Serializer.Serialize(ServerMove);
                    Debug.Log("Move serialized...\nLength:" + serializedMove.Length.ToString());

                    //Send the serialized server moveto the client
                    await Task.Run(() => sock.Send(serializedMove));
                    Debug.Log("Move sent...");

                    //Clear the server move
                    ServerMove = null;

                    //If the board is terminal, then break out of the main loop
                    if (GameBoard.Winner != -1)
                    {
                        Debug.Log("Game over! " + (GameBoard.Winner == 0? "Draw!" : "Winner was player " + GameBoard.Winner));
                        break;
                    }
                }
            }
            catch (SocketException)
            {
                Debug.Log("Connection was closed by the client, exiting server...");
            }

            //Close the socket
            DisconnectClient();
            disconnectCallback();
        }

        /// <summary>
        /// Called when this object is disposed <para/>
        /// Releases unmanaged resources used by GameServer from memory
        /// </summary>
        public void Dispose()
        {
            StopListening();
            DisconnectClient();
        }

        /// <summary>
        /// Closes the socket used to communicate with a client if there is one
        /// </summary>
        public void DisconnectClient()
        {
            if (sock.Connected)
            {
                sock.Close();
            }
        }

        /// <summary>
        /// If the listener has been started, stop it
        /// </summary>
        public void StopListening()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

        /// <summary>
        /// A boolean flag indicating whether a client is currently connected to the server
        /// </summary>
        public bool Connected
        {
            get { return sock == null? false : sock.Connected; }
        }
    }
}
