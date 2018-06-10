using MCTS.Core;
using MCTS.Core.Games;
using MCTS.Visualisation.Hashing;
using System.Collections.Generic;
using System.Threading.Tasks;
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
		/// Flag indicating how the board is displayed to the user <para/>
		/// True if displaying a board model <para/>
		/// False if displaying a rich text board display
		/// </summary>
		public bool displayBoardModel;

		/// <summary>
		/// The board model display being used for this visualisation <para/>
		/// Null if the <see cref="displayBoardModel"/> flag is set to false
		/// </summary>
		public BoardModelController boardModelController;

		/// <summary>
		/// Singleton reference to the active HashController
		/// </summary>
		public static FestivalServerController Controller;

		/// <summary>
		/// The server object used to communicate with a remote client
		/// </summary>
		private GameServer server;

		/// <summary>
		/// The main board being played on
		/// </summary>
		private C4Board board;

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

		private List<C4BoardModelController> FinalNodes = new List<C4BoardModelController>();

		/// <summary>
		/// The MCTS used to provide node data
		/// </summary>
		private TreeSearch<Node> mcts;

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

		private int currentNodeIndex = 1;

		private float MCTSStartTime = 0f;

		private const int NODE_LIMIT = 50000;

		private const int HASHNODE_LIMIT = 30;

		private Move moveToMake = null;

		private bool startMakingMove = false;

		private float makingMoveStartTime = 0f;

		private const float MAKE_MOVE_TIME = 1f;

		/// <summary>
		/// The delay in seconds between nodes being created when <see cref="playing"/> is active
		/// </summary>
		private const float SPAWN_DELAY = 0.2f;

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
		}

		/// <summary>
		/// Called every frame <para/>
		/// Used to control the creation of new nodes when <see cref="playing"/> is true <para/>
		/// Also used to listen for user input
		/// </summary>
		public void Update()
		{
			//If there is a root node, make the camera look at it
			if(AllNodes != null && AllNodes.Count > 0 && AllNodes[0] != null)
			{
				Camera.main.transform.LookAt(Vector3.Lerp(Camera.main.transform.position + Camera.main.transform.forward, AllNodes[0].transform.position, 0.005f));
			}

			//If starting to make a move, pull all hashnodes into the root as a nice animation, then make the move
			if(startMakingMove)
			{
				for(int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position, AllNodes[0].transform.position, Time.time - makingMoveStartTime);
				}

				if(makingMoveStartTime < Time.time - MAKE_MOVE_TIME - 0.5f)
				{
					GameObject temp = Instantiate(Resources.Load<GameObject>("C4 Board"));
					temp.transform.position = AllNodes[0].transform.position;
					temp.transform.right = temp.transform.position - Camera.main.transform.position;
					temp.layer = 0;
					for(int i = 0; i < transform.childCount; i++)
					{
						Destroy(transform.GetChild(i).gameObject);
					}

					MakeMoveOnBoard(moveToMake);
					server.ServerMove = moveToMake;
					startMakingMove = false;
				}
			}

			//Return if there is no tree search happening
			if (mcts == null)
			{
				return;
			}

			//If MCTS has been running too long, or has generated enough nodes, then stop it
			if(MCTSStartTime < Time.time - 5 || mcts.AllNodes.Count > NODE_LIMIT)
			{
				runMCTS = false;
			}

			//If the hashnode limit has been reached, make the best move and send it to the client
			if(AllNodes.Count >= HASHNODE_LIMIT)
			{
				playing = false;
				startMakingMove = true;
				makingMoveStartTime = Time.time;
				moveToMake = mcts.Root.GetBestChild().GameBoard.LastMoveMade;		
				mcts = null;
			}

			if(Input.GetKeyDown(KeyCode.P))
			{
				PerformStep();
				Debug.Log("MCTS Nodes: " + mcts.AllNodes.Count);
			}

			//Perform a step every interval, defined as the SPAWN_DELAY constant
			if (playing && Time.time - lastSpawn > SPAWN_DELAY && AllNodes.Count < HASHNODE_LIMIT)
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
			server = new GameServer(FestivalServerUIController.GetPortInput(), ResetScene, ClientConnected, MakeMoveOnBoard, ResetScene, ResetGame);
			server.StartListening();

			//Alert the user that the server is listening for a client connection
			FestivalServerUIController.BeginListening();
		}

		/// <summary>
		/// Called when the client wishes to reset the game
		/// </summary>
		private void ResetGame()
		{
			board = new C4Board();
			server.GameBoard = board;
			FinalNodes = new List<C4BoardModelController>();
		}

		/// <summary>
		/// Called when a move is made on the board, either locally or by a remote client
		/// </summary>
		/// <param name="m">The move to make</param>
		private void MakeMoveOnBoard(Move m)
		{
			//Make the move on the board
			board.MakeMove(m);

			//If the current player is now the client, do nothing and wait
			if(board.CurrentPlayer == 1)
			{
				return;
			}

			//Create an MCTS instance
			mcts = new TreeSearch<Node>(board);

			//Refresh the current node index
			currentNodeIndex = 1;

			//Reset dictionaries and lists
			nodePositionMap = new Dictionary<Vector3, GameObject>();
			nodeObjectMap = new Dictionary<Node, GameObject>();
			AllNodes = new List<HashNode>();

			//Calculate the position of the root node and add an object for it to the scene
			Vector3 rootNodePosition = BoardToPosition(mcts.Root.GameBoard);
			GameObject rootNode = Instantiate(Resources.Load("HashNode"), rootNodePosition, Quaternion.identity) as GameObject;
			rootNode.transform.parent = transform;
			rootNode.GetComponent<HashNode>().AddNode(null, mcts.Root, false);
			rootNode.GetComponent<HashNode>().Initialise(rootNodePosition);

			//Add the root node to the position and object maps and allnodes list
			nodePositionMap.Add(rootNodePosition, rootNode);
			nodeObjectMap.Add(mcts.Root, rootNode);
			AllNodes.Add(rootNode.GetComponent<HashNode>());
			playing = true;

			//Adjust the size of the new root
			rootNode.GetComponent<HashNode>().AdjustSize();

			//Start running MCTS
			MCTSStartTime = Time.time;
			runMCTS = true;
			RunMCTS(mcts);
		}

		private async void RunMCTS(TreeSearch<Node> m)
		{
			await Task.Factory.StartNew(() => { while (runMCTS && mcts != null) { mcts.Step(); } });
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
			//Create an empty board instance for the root node
			Board board;

			//Create a C4 board to be used
			board = new C4Board();
			displayBoardModel = true;

			//Create a C4 Board GameObject and obtain a reference to its BoardModelController Component
			GameObject boardModel = Instantiate(Resources.Load("C4 Board", typeof(GameObject))) as GameObject;
			boardModelController = boardModel.GetComponent<BoardModelController>();
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
			//If the current node index is greater than the amount of nodes, do nothing
			if(currentNodeIndex >= mcts.AllNodes.Count)
			{
				Debug.Log("Not enough nodes in current TreeSearch instance to perform a step...");
				return;
			}

			//Get a reference to the newest node
			Node newestNode = mcts.AllNodes[currentNodeIndex];
			currentNodeIndex++;

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

			//Initialise the newest hash node and add a mcts Node to it
			nodeObjectMap[newestNode].GetComponent<HashNode>().Initialise(newNodePosition);
			if (newestNode.Parent != null)
			{
				nodeObjectMap[newestNode].GetComponent<HashNode>().AddNode(nodeObjectMap[newestNode.Parent], newestNode, true);
			}

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