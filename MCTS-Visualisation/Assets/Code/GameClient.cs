using MCTS.Core;
using MCTS.Core.Games;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MCTS.Visualisation
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
		private NetworkStream stream;

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
		/// A callback which will be called if the client requests a game reset early
		/// </summary>
		private Action resetCallback;

		/// <summary>
		/// The <see cref="Board"/> reference used by this GameServer to play the game out on
		/// </summary>
		public Board GameBoard { get; set; }

		/// <summary>
		/// A move which can be set directly <para/>
		/// When this move is set, it is serialized and sent to the server, and then set to be null
		/// </summary>
		public Move ClientMove { get; set; }

		/// <summary>
		/// If this flag is set, instead of waiting for a move to be input by the client, the server will be sent a reset command
		/// </summary>
		public bool ResetFlag { get; set; }

		/// <summary>
		/// If this flag is set, instead of waiting for a move to be input by the client, the server will be send a request to change to watch mode
		/// </summary>
		public bool WatchFlag { get; set; }

		/// <summary>
		/// Creates a GameClient instance
		/// </summary>
		/// <param name="cCallback">The callback method to call when a connection with a server has been made</param>
		/// <param name="fCallback">The callback method to call when a connection with a server failed</param>
		/// <param name="mCallback">The callback method to call when a move is made</param>
		/// <param name="dCallback">The callback method to call when a client has disconnection</param>
		/// <param name="rCallback">The callback method to call when a game is being reset early</param>
		public GameClient(Action cCallback, Action fCallback, Action<Move> mCallback, Action dCallback, Action rCallback)
		{
			//Set the callbacks
			connectedCallback = cCallback;
			failedConnectionCallback = fCallback;
			moveCallback = mCallback;
			disconnectCallback = dCallback;
			resetCallback = rCallback;

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

			//Set the timeout
			stream.ReadTimeout = 500;

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
				//Main loop, never exit
				while (true)
				{
					//Create a reset message
					byte[] resetMessage = Encoding.ASCII.GetBytes("Reset");

					//If the game has not been reset early, wait for the board to be set locally
					if (!ResetFlag)
					{
						//Wait for the gameboard to be set
						while (GameBoard == null)
						{
							if (!Connected)
							{
								//Close the socket
								Disconnect();
								disconnectCallback();
								return;
							}
							await Task.Delay(500);
						}

						//Ouput a message signalling that the board was set
						Debug.Log("Board set, starting game...");

						//Alert the server that we are resetting the game board
						await Task.Run(() => stream.Write(resetMessage, 0, resetMessage.Length));

						//Set the watchmode flag to false
						WatchFlag = false;
					}
					else
					{
						//If the game has been reset early, immediately reset the game
						resetCallback();
						ResetFlag = false;
					}

					//Execute a main loop which waits for a client move to be received before sending a server response, until the game board is terminal
					while (GameBoard.Winner == -1)
					{
						//Wait for the user to select a move or set the reset/watch flag
						while (ClientMove == null && !ResetFlag && !WatchFlag)
						{
							if (!Connected)
							{
								//Close the socket
								Disconnect();
								disconnectCallback();
								return;
							}
							await Task.Delay(500);
						}

						//If the reset or watch flag was set, send the request to the server and break out of the game loop
						if (ResetFlag || WatchFlag)
						{
							if (ResetFlag)
							{
								await Task.Run(() => stream.Write(resetMessage, 0, resetMessage.Length));
							}
							else if(WatchFlag)
							{
								//Create a watch request message
								byte[] watchMessage = Encoding.ASCII.GetBytes("Watch");
								
								//Send the watch message
								await Task.Run(() => stream.Write(watchMessage, 0, watchMessage.Length));
							}
							break;
						}

						//Serialize the client move and send it to the server
						byte[] serializedMove = Serializer.Serialize(ClientMove);
						Debug.Log("Move serialized...\nLength:" + serializedMove.Length.ToString());

						//Send the serialized client move to the server						
						await Task.Run(() => stream.Write(serializedMove, 0, serializedMove.Length));
						Debug.Log("Move sent...");

						//Clear the client move
						ClientMove = null;

						//If the board is terminal, then break out of the main loop
						if (GameBoard.Winner != -1)
						{
							Debug.Log("Game over! " + (GameBoard.Winner == 0 ? "Draw!" : "Winner was player " + GameBoard.Winner));
							break;
						}

						Debug.Log("Waiting for move selection from server...");

						//Wait until data is recieved from the server
						while (true)
						{
							//Check connection
							if (!Connected)
							{
								//Close the socket
								Disconnect();
								disconnectCallback();
								return;
							}
							Debug.Log("Checking for reply from server...");
							await Task.Delay(500);
							if (stream.DataAvailable)
							{
								stream.Read(buffer, 0, buffer.Length);
								break;
							}
						}

						//Deserialize the data to obtain a move object
						Move serverMove = (C4Move)Serializer.Deserialize(buffer);

						//Output a string representation of the received move
						Debug.Log("Server move received:\n" + serverMove.ToString());

						//Make the server move on the game board, if it is terminal then break out of the main loop
						moveCallback(serverMove);
					}

					//Output an end of game message and set the gameboard to be null
					if (ResetFlag)
					{
						Debug.Log("Game was reset early by client...");
					}
					else if(WatchFlag)
					{
						Debug.Log("Game was ended early, watch mode entered...");
					}
					else
					{
						Debug.Log("Game over! " + (GameBoard.Winner == 0 ? "Draw!" : "Winner was player " + GameBoard.Winner));
					}
					GameBoard = null;
				}
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
				client.Dispose();
			}
		}

		/// <summary>
		/// A boolean flag indicating whether this client is currently connected to a server
		/// </summary>
		public bool Connected
		{
			get
			{
				if (client == null)
				{
					return false;
				}

				// Detect if client disconnected
				if (client.Client.Poll(0, SelectMode.SelectRead))
				{
					byte[] buff = new byte[1];
					if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
					{
						//Disconnected
						return false;
					}
				}

				//Still connected
				return true;
			}
		}
	}
}
