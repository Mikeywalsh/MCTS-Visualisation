using MCTS.Core;
using MCTS.Core.Games;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace MCTS.Visualisation.Play
{
    /// <summary>
    /// A game client, which allows a connection to a server and for a game to be played between server and client
    /// </summary>
    class GameClient : IDisposable
    {
        /// <summary>
        /// The <see cref="TcpClient"/> used to connect to the game serevr<para/>
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// The stream used to communicate with the server
        /// </summary>
        private Stream stream;

        /// <summary>
        /// A data buffer used for communications with the server
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// A callback which will be called when a connection with a server has been made
        /// </summary>
        private Action connectedCallback;

        /// <summary>
        /// A callback which will be called when a connection with a server has failed
        /// </summary>
        private Action failedConnectionCallback;

        /// <summary>
        /// A callback which will be called whenever a move is received from the server
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
        /// When this move is set, it is serialized and sent to the server, and then set to be null
        /// </summary>
        public Move ClientMove { get; set; }

        /// <summary>
        /// Creates a GameClient instance
        /// </summary>
        /// <param name =board">The board reference that the client will use</param>
        /// <param name="cCallback">The callback method to call when a connection with a server has been made</param>
        /// <param name="fCallback">The callback method to call when a connection with a server failed</param>
        /// <param name="mCallback">The callback method to call when a move is made</param>
        /// <param name="dCallback">The callback method to call when a client has disconnection</param>
        public GameClient(Board board, Action cCallback, Action fCallback, Action<Move> mCallback, Action dCallback)
        {
            //Set the server board reference
            GameBoard = board;

            //Set the callbacks
            connectedCallback = cCallback;
            failedConnectionCallback = fCallback;
            moveCallback = mCallback;
            disconnectCallback = dCallback;

            //Create a TcpClient
            client = new TcpClient();
            Debug.Log("Client Initialised...");

            //Initialise the buffer used to communicate with the server
            buffer = new byte[1024];
        }

        /// <summary>
        /// Attempts to connect to a server on the provided IP and port <para/>
        /// A main loop is then entered which allows a client to play a full game against an opponent on the server machine
        /// </summary>
        /// <param name="serverIP">The IP address of the server</param>
        /// <param name="serverPort">The port the server is using</param>
        public async void AttemptConnect(IPAddress serverIP, short serverPort)
        {
            //Attempt connection to the server
            try
            {
                await Task.Run(() => client.Connect(serverIP, serverPort));
                Debug.Log("Connection Established...");
            }
            catch (SocketException)
            {
                failedConnectionCallback();
                return;
            }

            //Obtain a reference to the stream used to send data to the server
            stream = client.GetStream();

            //Call the connected callback
            connectedCallback();

            //Enter the update loop
            UpdateLoop();
        }

        /// <summary>
        /// A main loop is entered which allows a client to play a full game against an opponent on the server machine
        /// </summary>
        async private void UpdateLoop()
        {
            if (!client.Connected)
            {
                throw new Exception("Not connected to a client. Call StartListening first...");
            }

            try
            {
                //Execute a main loop which waits for a client move to be received before sending a server response, until the game board is terminal
                while (GameBoard.Winner == -1)
                {
                    //Wait for the user to select a move
                    while (ClientMove == null)
                    {
                        await Task.Delay(500);
                    }

                    //Serialize the client move and send it to the server
                    byte[] serializedMove = Serializer.Serialize(ClientMove);
                    Debug.Log("Move serialized...\nLength:" + serializedMove.Length.ToString());

                    //Send the serialized client move to the server
                    await Task.Run(() => stream.Write(serializedMove, 0, serializedMove.Length));
                    Debug.Log("Move sent...");

                    //Clear the client move
                    ClientMove = null;

                    //If the game board is now terminal, break out of the update loop
                    if (GameBoard.Winner != -1)
                    {
                        break;
                    }

                    //If the board is terminal, then break out of the main loop
                    if (GameBoard.Winner != -1)
                    {
                        Debug.Log("Game over! " + (GameBoard.Winner == 0 ? "Draw!" : "Winner was player " + GameBoard.Winner));
                        break;
                    }

                    Debug.Log("Waiting for move selection from server...");

                    //Wait until data is recieved from the server
                    await Task.Run(() => stream.Read(buffer, 0, buffer.Length));

                    //Deserialize the data to obtain a move object
                    Move serverMove = (C4Move)Serializer.Deserialize(buffer);

                    //Output a string representation of the received move
                    Debug.Log("Server move received:\n" + serverMove.ToString());

                    //Make the server move on the game board, if it is terminal then break out of the main loop
                    moveCallback(serverMove);

                }
                Debug.Log("Game over! " + (GameBoard.Winner == 0 ? "Draw!" : "Winner was player " + GameBoard.Winner));
            }
            catch (SocketException)
            {
                Debug.Log("Connection was closed by the server, exiting client...");
            }

            //Close the socket
            Disconnect();
            disconnectCallback();
        }

        /// <summary>
        /// Called when this object is disposed <para/>
        /// Releases unmanaged resources used by GameClient from memory
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            client.Dispose();

            if (stream != null)
            {
                stream.Dispose();

            }
        }

        /// <summary>
        /// Closes the TCP connection used to communicate with a server if there is one
        /// </summary>
        public void Disconnect()
        {
            if (client.Connected)
            {
                client.Close();
            }
        }

        /// <summary>
        /// A boolean flag indicating whether this client is currently connected to a server
        /// </summary>
        public bool Connected
        {
            get { return client == null ? false : client.Connected; }
        }
    }
}
