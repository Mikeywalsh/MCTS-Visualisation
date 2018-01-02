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
        private byte[] buffer;

        public Board ClientBoard { get; private set; }
        public Board ServerBoard { get; set; }

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
                    ClientBoard = (C4Board)Serializer.Deserialize(buffer);

                    //Output a string representation of the received board
                    Debug.Log("Client board recieved:\n" + ClientBoard.ToString());

                    //If the client board is terminal, end the connection
                    if (ClientBoard.Winner != -1)
                    {
                        Debug.Log("Game over! " + (ClientBoard.Winner == 0 ? "Draw!" : "Winner was player " + ClientBoard.Winner));
                        break;
                    }

                    //Wait for the server machine to create a resultant board
                    while(ServerBoard == null)
                    {
                        await Task.Delay(500);
                    }

                    //Clear the client board
                    ClientBoard = null;

                    //Serialize the server board state and send it to the client
                    byte[] serializedBoard = Serializer.Serialize(ServerBoard);
                    Debug.Log("Board serialized...\nLength:" + serializedBoard.Length.ToString());

                    //Send the serialized initial board state to the client
                    await Task.Run(() => sock.Send(serializedBoard));
                    Debug.Log("Board state sent...");

                    //If the server board is terminal, end the connection
                    if (ServerBoard.Winner != -1)
                    {
                        Debug.Log("Game over! " + (ServerBoard.Winner == 0? "Draw!" : "Winner was player " + ServerBoard.Winner));
                        break;
                    }

                    //Clear the server board
                    ServerBoard = null;
                }
            }
            catch (SocketException)
            {
                Debug.Log("Connection was closed by the client, exiting server...");
            }

            //Close the socket
            DisconnectClient();
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
    }
}
