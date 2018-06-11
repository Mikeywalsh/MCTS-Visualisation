using MCTS.Core;
using MCTS.Core.Games;
using MCTS.Visualisation.Hashing;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MCTS.Visualisation.Festival
{
	/// <summary>
	/// Main controller singleton for the hashing visualisation <para/>
	/// In this visualisation, each board state is given its own position in world space and <see cref="LineRenderer"/> objects are used to create links between the nodes
	/// </summary>
	public class FestivalServerController : MonoBehaviour
	{
		/// <summary>
		/// Singleton reference to the active FestivalServerController
		/// </summary>
		public static FestivalServerController Controller;

		/// <summary>
		/// Flag indicating how the board is displayed to the user <para/>
		/// True if displaying a board model <para/>
		/// False if displaying a rich text board display
		/// </summary>
		public bool displayBoardModel;

		/// <summary>
		/// The board model display being used for this visualisation <para/>
		/// </summary>
		public BoardModelController boardModelController;

		/// <summary>
		/// The linerenderer which connects the board model to the last path node
		/// </summary>
		public LineRenderer pathConnection;

		/// <summary>
		/// The transform used to hold all path nodes
		/// </summary>
		public Transform PathNodeHolder;

		/// <summary>
		/// Text mesh which shows the winner, if there is one
		/// </summary>
		public TextMeshPro WinnerText;

		/// <summary>
		/// The server object used to communicate with a remote client
		/// </summary>
		private GameServer server;

		/// <summary>
		/// The main board being played on
		/// </summary>
		private C4Board board;

		/// <summary>
		/// Buffer for storing result of <see cref="BoardToPosition(Board)"/>, instead of calling every frame
		/// </summary>
		private Vector3 boardPosition;

		/// <summary>
		/// A dictionary that maps unique positions in world space to their corresponding node gameobjects
		/// </summary>
		private Dictionary<Vector3, GameObject> nodePositionMap = new Dictionary<Vector3, GameObject>();

		/// <summary>
		/// A dictionary that maps unique nodes to their corresponding node gameobjects
		/// </summary>
		private Dictionary<Node, GameObject> nodeObjectMap = new Dictionary<Node, GameObject>();

		/// <summary>
		/// A list of all hash nodes in the current scene
		/// </summary>
		private List<HashNode> AllNodes = new List<HashNode>();

		/// <summary>
		/// A list of gameobjects which indicate the current path of the AI's decision making process thus far
		/// </summary>
		private List<GameObject> PathNodes = new List<GameObject>();

		/// <summary>
		/// The MCTS used to determine the best node to select
		/// </summary>
		private TreeSearch<Node> aiMCTS;

		/// <summary>
		/// The MCTS used to produce the hashing visualisation
		/// </summary>
		private TreeSearch<Node> displayMCTS;

		/// <summary>
		/// An array of evenly distributed points on the surface of a sphere, used for position calculation of <see cref="HashNode"/>'s <para/>
		/// This array is only calculated once, the first time it is needed
		/// </summary>
		private static Vector3[] spherePositions;

		/// <summary>
		/// The timestamp that the last step was performed <para/>
		/// Used to determine when to do the next step
		/// </summary>
		private float lastSpawn;

		/// <summary>
		/// Flag which controls whether the playing animation is active <para/>
		/// When the playing animation is active, nodes are created periodically
		/// </summary>
		private bool playing = false;

		private bool runMCTS = false;

		private float MCTSStartTime = 0f;

		private const int NODE_LIMIT = 50000;

		private const int HASHNODE_LIMIT = 30;

		private const int HASHNODE_LIMIT_WATCH_MODE = 250;

		private Move moveToMake = null;

		private bool startMakingMove = false;

		private float makingMoveStartTime = 0f;

		private const float MAKE_MOVE_TIME = 1f;

		private bool watchMode = false;

		/// <summary>
		/// The delay in seconds between nodes being created when <see cref="playing"/> is active
		/// </summary>
		private const float SPAWN_DELAY = 0.25f;

		/// <summary>
		/// The amount of spacing between each node
		/// </summary>
		private const float NODE_SPACING = 35f;

		/// <summary>
		/// Enable the running of the application in the background when initially ran
		/// </summary>
		public void Start()
		{
			Application.runInBackground = true;

			//Set the singleton reference
			Controller = this;

			//Reset dictionaries and lists
			nodeObjectMap = new Dictionary<Node, GameObject>();
			nodePositionMap = new Dictionary<Vector3, GameObject>();
			AllNodes = new List<HashNode>();
		}

		/// <summary>
		/// Called every frame <para/>
		/// Used to control the creation of new nodes when <see cref="playing"/> is true <para/>
		/// Also used to listen for user input
		/// </summary>
		public void Update()
		{
			//Move the board display to where it is supposed to be
			boardModelController.transform.parent.position = Vector3.Lerp(boardModelController.transform.parent.position, boardPosition, 0.05f);
			boardModelController.transform.parent.LookAt(Camera.main.transform);

			//Ensure that the board model is attached to the path connection
			pathConnection.SetPosition(0, boardModelController.transform.parent.position);

			//Make the camera look at the board model and always be a set distance away from it
			//Only do this if not in watchmode
			if (!watchMode)
			{
				Camera.main.transform.LookAt(Vector3.Lerp(Camera.main.transform.position + Camera.main.transform.forward, boardModelController.transform.position, 0.001f));
				Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, boardModelController.transform.position + new Vector3(150, 0, 0), 0.01f);// ((AllNodes[0].transform.position - Camera.main.transform.position).normalized * 110), 0.001f);
			}

			//If starting to make a move, pull all hashnodes into the root as a nice animation, then make the move
			if (startMakingMove)
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position, boardPosition, Time.time - makingMoveStartTime);
					transform.GetChild(i).localScale = Vector3.Lerp(transform.GetChild(i).localScale, Vector3.zero, Time.time - makingMoveStartTime);
				}

				if (makingMoveStartTime < Time.time - MAKE_MOVE_TIME - 0.5f)
				{
					for (int i = 0; i < transform.childCount; i++)
					{
						Destroy(transform.GetChild(i).gameObject);
					}

					MakeMoveOnBoard(moveToMake);
					server.ServerMove = moveToMake;
					startMakingMove = false;
				}
			}

			//Return if there is no tree search happening and watchmode is not active
			if (aiMCTS == null && !watchMode)
			{
				return;
			}

			//If watchmode is not active, check the AI as usual
			if (!watchMode)
			{
				//If MCTS has been running too long, or has generated enough nodes, then stop it
				if (MCTSStartTime < Time.time - 5 || aiMCTS.AllNodes.Count > NODE_LIMIT)
				{
					runMCTS = false;
				}

				//If the hashnode limit has been reached, make the best move and send it to the client
				if (AllNodes.Count >= HASHNODE_LIMIT && !runMCTS)
				{
					playing = false;
					startMakingMove = true;
					makingMoveStartTime = Time.time;
					moveToMake = aiMCTS.Root.GetBestChild().GameBoard.LastMoveMade; ;// = aiMCTS.Root.GetBestChild().GameBoard.LastMoveMade;		
					aiMCTS = null;
					displayMCTS = null;
				}
			}

			if (Input.GetKeyDown(KeyCode.P))
			{
				PerformStep();
				Debug.Log("MCTS Nodes: " + aiMCTS.AllNodes.Count);
				Debug.Log(AllNodes[0].TotalVisits);
				playing = false;
			}

			////Perform a step every interval, defined as the SPAWN_DELAY constant
			if (playing && Time.time - lastSpawn > SPAWN_DELAY && ((!watchMode && AllNodes.Count < HASHNODE_LIMIT) || (watchMode && AllNodes.Count < HASHNODE_LIMIT_WATCH_MODE)))
			{
				PerformStep();
				lastSpawn = Time.time;
			}
		}

		/// <summary>
		/// Called when the start server button is pressed
		/// </summary>
		public void StartServerButtonPressed()
		{
			//Initialise the game server
			server = new GameServer((short)FestivalServerUIController.GetPortInput(), ResetScene, ClientConnected, MakeMoveOnBoard, ResetScene, ResetGame, EnterWatchMode);
			server.StartListening();

			//Alert the user that the server is listening for a client connection
			FestivalServerUIController.BeginListening();
		}

		private void EnterWatchMode()
		{
			//Enable the watch mode flag
			watchMode = true;

			//Reset the camera
			Camera.main.GetComponent<FestivalServerCameraControl>().ResetCamera();

			//Create a new display MCTS instance
			displayMCTS = new TreeSearch<Node>(board.Duplicate());

			//Reset dictionaries and lists
			nodePositionMap = new Dictionary<Vector3, GameObject>();
			nodeObjectMap = new Dictionary<Node, GameObject>();
			AllNodes = new List<HashNode>();

			//Calculate the position of the root node and add an object for it to the scene
			Vector3 rootNodePosition = BoardToPosition(displayMCTS.Root.GameBoard);
			GameObject rootNode = Instantiate(Resources.Load("HashNode"), rootNodePosition, Quaternion.identity) as GameObject;
			rootNode.transform.parent = transform;
			rootNode.GetComponent<HashNode>().AddNode(null, displayMCTS.Root, false);
			rootNode.GetComponent<HashNode>().Initialise(rootNodePosition);

			//Add the root node to the position and object maps and allnodes list
			nodePositionMap.Add(rootNodePosition, rootNode);
			nodeObjectMap.Add(displayMCTS.Root, rootNode);
			AllNodes.Add(rootNode.GetComponent<HashNode>());

			//Adjust the size of the new root
			rootNode.GetComponent<HashNode>().AdjustSize();

			//Start running MCTS
			playing = true;
		}

		/// <summary>
		/// Called when the client wishes to reset the game
		/// </summary>
		private void ResetGame()
		{
			//Reset reference objects
			board = new C4Board();
			server.GameBoard = board;
			PathNodes = new List<GameObject>();

			//Disable watch mode
			watchMode = false;

			//Reset the board model controller
			boardModelController.transform.parent.position = BoardToPosition(board);
			boardModelController.Initialise();
			boardModelController.transform.position = Vector3.zero;
			boardPosition = Vector3.zero;

			//Reset the path connection
			pathConnection.SetPosition(0, Vector3.zero);
			pathConnection.SetPosition(1, Vector3.zero);

			//Randomise its color
			Color32 newLineColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
			pathConnection.startColor = newLineColor;
			pathConnection.endColor = newLineColor;

			//Destroy all remaining path nodes
			for (int i = 0; i < PathNodeHolder.childCount; i++)
			{
				Destroy(PathNodeHolder.GetChild(i).gameObject);
			}

			//Hide the winner text mesh
			WinnerText.gameObject.SetActive(false);

			//Reset the camera position and rotation
			Camera.main.GetComponent<FestivalServerCameraControl>().ResetCamera();
		}

		/// <summary>
		/// Updates the path nodes, which show a path of moves taken so far
		/// </summary>
		private void UpdatePathNodes()
		{
			//Create a new path node
			GameObject pathNode = Instantiate(Resources.Load<GameObject>("PathNode"), boardPosition, Quaternion.identity);

			//Set the new path nodes parent to be the path node holder
			pathNode.transform.parent = PathNodeHolder;

			//If the pathnodes list is not empty, draw a line from the last pathnode to the new one
			if (PathNodes.Count > 0)
			{
				//Create a new line renderer
				LineRenderer newLine = PathNodes[PathNodes.Count - 1].AddComponent<LineRenderer>();

				//Set layer to ignore raycast, which has a value of 2
				newLine.gameObject.layer = 2;

				//Initialise the line renderer starting values
				newLine.startWidth = 0.2f;
				newLine.endWidth = 0.2f;
				Color32 lineColor = pathConnection.startColor;
				newLine.startColor = lineColor;
				newLine.endColor = lineColor;
				newLine.material = Resources.Load<Material>("LineMat");

				//Set the line positions
				newLine.SetPosition(0, PathNodes[PathNodes.Count - 1].transform.position);
				newLine.SetPosition(1, pathNode.transform.position);

				//Connect the pathconnection to the newest node
				pathConnection.SetPosition(1, pathNode.transform.position);

				//Randomise the pathconnection color
				Color32 newLineColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
				pathConnection.startColor = newLineColor;
				pathConnection.endColor = newLineColor;
			}

			//Finally, add the new pathnode to the list of path nodes
			PathNodes.Add(pathNode);
		}

		/// <summary>
		/// Called when a move is made on the board, either locally or by a remote client
		/// </summary>
		/// <param name="m">The move to make</param>
		private void MakeMoveOnBoard(Move m)
		{
			//Make the move on the board
			board.MakeMove(m);

			//Update the path nodes
			UpdatePathNodes();

			//Update the board display
			boardModelController.SetBoard(board);

			//Update the board position buffer
			boardPosition = BoardToPosition(board);

			//If the current player is now the client, or the game has finished, do nothing
			if (board.CurrentPlayer == 1 || board.Winner != -1)
			{
				//If there was a winner, set the winner text to be active
				if (board.Winner != -1)
				{
					WinnerText.gameObject.SetActive(true);
				}

				//If there was a winner, set the winner text accordingly
				if (board.Winner == 0)
				{
					WinnerText.text = "The game has ended in a draw...";
				}
				else if (board.Winner == 1)
				{
					WinnerText.text = "You have won, congratulations!";
				}
				else if (board.Winner == 2)
				{
					WinnerText.text = "The AI has won, unlucky...";
				}
				return;
			}

			//Create new MCTS instances
			aiMCTS = new TreeSearch<Node>(board.Duplicate());
			displayMCTS = new TreeSearch<Node>(board.Duplicate());

			//Reset dictionaries and lists
			nodePositionMap = new Dictionary<Vector3, GameObject>();
			nodeObjectMap = new Dictionary<Node, GameObject>();
			AllNodes = new List<HashNode>();

			//Calculate the position of the root node and add an object for it to the scene
			Vector3 rootNodePosition = BoardToPosition(displayMCTS.Root.GameBoard);
			GameObject rootNode = Instantiate(Resources.Load("HashNode"), rootNodePosition, Quaternion.identity) as GameObject;
			rootNode.transform.parent = transform;
			rootNode.GetComponent<HashNode>().AddNode(null, displayMCTS.Root, false);
			rootNode.GetComponent<HashNode>().Initialise(rootNodePosition);

			//Add the root node to the position and object maps and allnodes list
			nodePositionMap.Add(rootNodePosition, rootNode);
			nodeObjectMap.Add(displayMCTS.Root, rootNode);
			AllNodes.Add(rootNode.GetComponent<HashNode>());
			playing = true;

			//Adjust the size of the new root
			rootNode.GetComponent<HashNode>().AdjustSize();

			//Start running MCTS
			MCTSStartTime = Time.time;
			runMCTS = true;
			Task.Factory.StartNew(() => { while (runMCTS && m != null) { aiMCTS.Step(); } });
		}

		/// <summary>
		/// Called when a connection has been established to a client
		/// </summary>
		private void ClientConnected()
		{
			FestivalServerUIController.StopListening();
			StartVisualisation();
		}

		/// <summary>
		/// Resets the scene
		/// </summary>
		private void ResetScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		/// <summary>
		/// Starts a new visualisation
		/// </summary>
		private void StartVisualisation()
		{
			//Create a C4 board to be used
			board = new C4Board();
			displayBoardModel = true;

			//Initialise the board model display
			boardModelController.transform.parent.gameObject.SetActive(true);
			boardModelController.Initialise();

			//Swap out the current menu panels
			FestivalServerUIController.SetMenuPanelActive(false);
			FestivalServerUIController.SetNavigationPanelActive(true);

			//Set playing to false for now
			playing = false;
		}

		/// <summary>
		/// Gets the next node from the current MCTS <see cref="TreeSearch{T}"/> instance and adds it to the visualisation<para/>
		/// Also creates any relevant links between related nodes
		/// </summary>
		public void PerformStep()
		{
			//Perform an iteration of MCTS on the display instance
			displayMCTS.Step();

			//Get a reference to the newest node
			Node newestNode = displayMCTS.AllNodes[displayMCTS.AllNodes.Count - 1];

			//Display the current state that the AI is considering on the board model display
			boardModelController.SetBoard((GridBasedBoard)newestNode.GameBoard);

			//Hash the board contents of the newest node to obtain a positon
			Vector3 newNodePosition = BoardToPosition(newestNode.GameBoard);

			//Decrease the visibility of every node in the scene
			foreach (HashNode n in AllNodes)
			{
				n.SetVisibility(n.Visibility - 0.01f);
			}

			//If the current board state already exists, then don't create a new node, but create a line to the existing node
			if (nodePositionMap.ContainsKey(newNodePosition))
			{
				//Map the newest node to the existing node object if it does not already exist in the dicitonary
				if (!nodeObjectMap.ContainsKey(newestNode))
				{
					nodeObjectMap.Add(newestNode, nodePositionMap[newNodePosition]);
				}
			}
			else
			{
				//Instantiate the new node object at the hashed position
				GameObject newNodeObject = Instantiate(Resources.Load("HashNode"), nodeObjectMap[newestNode.Parent].transform.position, Quaternion.identity) as GameObject;

				//Set the parent of the new node object to be this controller object
				newNodeObject.transform.parent = transform;

				//Map the newest node to the new node object
				nodeObjectMap.Add(newestNode, newNodeObject);

				//Map the hashed position of the newest node to the new node object
				nodePositionMap.Add(newNodePosition, newNodeObject);

				AllNodes.Add(newNodeObject.GetComponent<HashNode>());
			}

			//Initialise the newest hash node and add an mcts Node to it
			nodeObjectMap[newestNode].GetComponent<HashNode>().Initialise(newNodePosition);
			nodeObjectMap[newestNode].GetComponent<HashNode>().AddNode(nodeObjectMap[newestNode.Parent], newestNode, true);
			FestivalServerUIController.SetTotalNodeText(nodeObjectMap.Count);
		}

		/// <summary>
		/// Hashes a <see cref="Board"/> object to a unique position in world space
		/// </summary>
		/// <param name="board">The board to hash</param>
		/// <returns>A <see cref="Vector3"/>, unique to the board state of hte board being hashed </returns>
		public Vector3 BoardToPosition(Board board)
		{
			GridBasedBoard gridBoard = (GridBasedBoard)board;
			Vector3 finalPos = Vector3.zero;

			if (spherePositions == null)
			{
				spherePositions = new Vector3[gridBoard.Width * gridBoard.Height];

				#region Fibbonacci Sphere algorithm
				for (int i = 0; i < spherePositions.Length; i++)
				{
					int samples = spherePositions.Length;
					float offset = 2f / samples;
					float increment = Mathf.PI * (3 - Mathf.Sqrt(5));

					float y = ((i * offset) - 1) + (offset / 2);
					float r = Mathf.Sqrt(1 - Mathf.Pow(y, 2));
					float phi = ((i + 1) % samples) * increment;
					float x = Mathf.Cos(phi) * r;
					float z = Mathf.Sin(phi) * r;
					spherePositions[i] = new Vector3(x, y, z);
				}
				#endregion
			}

			for (int y = 0; y < gridBoard.Height; y++)
			{
				for (int x = 0; x < gridBoard.Width; x++)
				{
					finalPos += spherePositions[(x * gridBoard.Height) + y].normalized * gridBoard.GetCell(x, y);
				}
			}

			return finalPos * NODE_SPACING;
		}

		/// <summary>
		/// Resets the array containing equally spaced positions on a sphere <para/>
		/// Called when the scene is reset
		/// </summary>
		public static void ResetSpherePositions()
		{
			spherePositions = null;
		}
	}
}