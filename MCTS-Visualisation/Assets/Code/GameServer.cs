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
	/// A game server, which allows a client to connect and a game to be played between client and server
	/// </summary>
	class GameServer : IDisposable
	{
		/// <summary>
		/// The address this server is being hosted on
		/// </summary>
		public IPAddress ServerAddress { get; private set; }

		/// <summary>
		/// The port this server is being hosted on
		/// </summary>
		public short ServerPort { get; private set; }

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
		/// The stream used to communicate with the client
		/// </summary>
		private NetworkStream stream;

		/// <summary>
		/// A data buffer used for communications with the client
		/// </summary>
		private byte[] buffer;

		/// <summary>
		/// A callback which will be called when the server has failed to listen on a specific port
		/// </summary>
		private Action failedListenCallback;

		/// <summary>
		/// A callback which will be called when a client has connected to this server
		/// </summary>
		private Action connectedCallback;

		/// <summary>
		/// A callback which will be called whenever a move is received from the client
		/// </summary>
		private Action<Move> moveCallback;

		/// <summary>
		/// A callback which will be called if the connection between the server and client is ended
		/// </summary>
		private Action disconnectCallback;

		/// <summary>
		/// A callback which will be called if the client wishes to reset the game
		/// </summary>
		private Action resetCallback;

		/// <summary>
		/// The <see cref="Board"/> reference used by this GameServer to play the game out on
		/// </summary>
		public Board GameBoard { get; set; }

		/// <summary>
		/// A move which can be set directly <para/>
		/// When this move is set, it is serialized and sent to the client, and then set to be null
		/// </summary>
		public Move ServerMove { get; set; }

		/// <summary>
		/// Creates a GameServer instance and a <see cref="TcpListener"/> on the local IPv4 address of this machine and the provided port
		/// </summary>
		/// <param name="port">The port which the <see cref="TcpListener"/> will run on</param>
		/// <param name="fCallback">The callback method to call when the server has failed to listen on a specific port</param>
		/// <param name="cCallback">The callback method to call when a client has connected</param>
		/// <param name="mCallback">The callback method to call when a move is made</param>
		/// <param name="dCallback">The callback method to call when a client has disconnection</param>
		/// <param name="rCallback">The callback method to call when the client wishes to reset the game</param>
		public GameServer(short port, Action fCallback, Action cCallback, Action<Move> mCallback, Action dCallback, Action rCallback)
		{
			//Set the callbacks
			failedListenCallback = fCallback;
			connectedCallback = cCallback;
			moveCallback = mCallback;
			disconnectCallback = dCallback;
			resetCallback = rCallback;

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

			//Set the server IP and port
			ServerAddress = localIP;
			ServerPort = port;

			//Create a listener on the IPv4 address and specified port
			listener = new TcpListener(localIP, port);

			//Initialise the buffer used to communicate with the client
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
				failedListenCallback();
				return;
			}

			//Start the listener and inform the user
			listener.Start();
			Debug.Log("Waiting for a connection...");

			//Listen until a connection has been made
			while (sock == null)
			{
				try
				{
					//Start a task which listens for a new connection
					sock = await Task.Run(() => listener.AcceptSocket());

					//Bind the socket to a stream
					stream = new NetworkStream(sock);
				}
				catch (Exception)
				{
					//Scene has been exited early, leave method
					return;
				}
			}

			//Only one connection is allowed at a time, stop the listener
			listener.Stop();

			//If the socket is null, something has went wrong
			if (sock == null)
			{
				throw new Exception("Socket was not established correctly!");
			}

			//Call the connected callback method
			connectedCallback();

			Debug.Log("Connected to client with endpoint: " + sock.RemoteEndPoint);

			//Enter the update loop
			UpdateLoop();
		}

		/// <summary>
		/// A main loop is entered which allows a client to play a full game against an opponent on the server machine
		/// </summary>
		async private void UpdateLoop()
		{
			if (!Connected)
			{
				throw new Exception("Not connected to a client. Call StartListening first...");
			}

			try
			{
				//Main loop, never exit
				while (true)
				{
					//Wait until data is recieved from the client
					await Task.Run(() => sock.Receive(buffer));

					//If the client has sent a reset message, then reset the gameboard
					if(Encoding.ASCII.GetString(buffer).Substring(0,5) == "Reset")
					{
						//Reset the game
						resetCallback();
					}
					else
					{
						throw new Exception("Invalid message received from client...");
					}

					//Ouput a message signalling that the board was set
					Debug.Log("Board set, starting game...");

					//Execute a main loop which waits for a client move to be received before sending a server response, until the game board is terminal
					while (GameBoard.Winner == -1)
					{
						Debug.Log("Waiting for move selection from client...");

						//Wait until data is recieved from the client
						while (true)
						{
							//Check connection
							if (!Connected)
							{
								//Close the socket
								DisconnectClient();
								disconnectCallback();
								return;
							}
							Debug.Log("Checking for reply from client...");
							await Task.Delay(500);
							if(stream.DataAvailable)
							{
								stream.Read(buffer, 0, buffer.Length);
								break;
							}
						}

						//Deserialize the data to obtain a move object
						Move clientMove = (C4Move)Serializer.Deserialize(buffer);

						//Output a string representation of the received move
						Debug.Log("Client move recieved:\n" + clientMove.ToString());

						//Make the client move on the game board, if it is terminal then break out of the main loop
						moveCallback(clientMove);

						//If the game board is now terminal, break out of the update loop
						if (GameBoard.Winner != -1)
						{
							break;
						}

						//Wait for the user to select a move
						while (ServerMove == null)
						{
							if (!Connected)
							{
								//Close the socket
								DisconnectClient();
								disconnectCallback();
								return;
							}
							await Task.Delay(500);
						}

						//Serialize the server move and send it to the client
						byte[] serializedMove = Serializer.Serialize(ServerMove);
						Debug.Log("Move serialized...\nLength:" + serializedMove.Length.ToString());

						//Send the serialized server move to the client
						await Task.Run(() => stream.Write(serializedMove, 0, serializedMove.Length));
						Debug.Log("Move sent...");

						//Clear the server move
						ServerMove = null;
					}

					//Output an end of game message and set the gameboard to be null
					Debug.Log("Game over! " + (GameBoard.Winner == 0 ? "Draw!" : "Winner was player " + GameBoard.Winner));
					GameBoard = null;
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

			if (stream != null)
			{
				stream.Dispose();
			}
		}

		/// <summary>
		/// Closes the socket used to communicate with a client if there is one
		/// </summary>
		public void DisconnectClient()
		{
			if (sock != null && sock.Connected)
			{
				sock.Close();
				sock.Dispose();
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
			get
			{
				if (sock == null)
				{
					return false;
				}

				// Detect if client disconnected
				if (sock.Poll(0, SelectMode.SelectRead))
				{
					byte[] buff = new byte[1];
					if (sock.Receive(buff, SocketFlags.Peek) == 0)
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
