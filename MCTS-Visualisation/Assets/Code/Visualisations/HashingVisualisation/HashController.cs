using System.Collections.Generic;
using UnityEngine;
using MCTS.Core;
using MCTS.Core.Games;

namespace MCTS.Visualisation.Hashing
{
    /// <summary>
    /// Main controller singleton for the hashing visualisation <para/>
    /// In this visualisation, each board state is given its own position in world space and <see cref="LineRenderer"/> objects are used to create links between the nodes
    /// </summary>
    public class HashController : MonoBehaviour
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
        public static HashController Controller;

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
        }

        /// <summary>
        /// Called every frame <para/>
        /// Used to control the creation of new nodes when <see cref="playing"/> is true <para/>
        /// Also used to listen for user input
        /// </summary>
        public void Update()
        {
            if (mcts == null)
            {
                return;
            }

            //Allow the user to perform a step with the return key or Y button instead of pressing the step button
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("YButton"))
            {
                PerformStep(false);
            }

            //Allow the user to toggle the play/pause options with the p key or B button instead of pressing the respective buttons
            if (Input.GetKeyDown(KeyCode.P) || Input.GetButtonDown("BButton"))
            {
                TogglePlayPause();
            }


            if (playing && Time.time - lastSpawn > SPAWN_DELAY)
            {
                PerformStep(false);
                lastSpawn = Time.time;
            }
        }

        /// <summary>
        /// Called when the user enters the play/pause input <para/>
        /// Toggles play/pause depending on which one is currently active
        /// </summary>
        public void TogglePlayPause()
        {
            if (playing)
            {
                PauseButtonPressed();
            }
            else
            {
                PlayButtonPressed();
            }
        }

        /// <summary>
        /// Called when the play button is pressed <para/>
        /// Starts the animation of ndoes being creates and sets the pause button to be active
        /// </summary>
        public void PlayButtonPressed()
        {
            playing = true;
            HashUIController.PlayButtonPressed();
        }

        /// <summary>
        /// Called when the pause button is pressed <para/>
        /// Pauses the animation of nodes beng created and sets the play button to active again
        /// </summary>
        public void PauseButtonPressed()
        {
            playing = false;
            HashUIController.PauseButtonPressed();
        }

        /// <summary>
        /// Called when the start button is pressed <para/>
        /// Initialises the <see cref="mcts"/> tree search object and instantiates the root node <para/>
        /// Also creates as many starting nodes as the user specified
        /// </summary>
        public void StartButtonPressed()
        {
            //Create an empty board instance, which will have whatever game the user chooses assigned to it
            Board board;

            //Assign whatever game board the user has chosen to the board instance
            switch (HashUIController.GetGameChoice)
            {
                case 0:
                    board = new TTTBoard();
                    displayBoardModel = false;
                    break;
                case 1:
                    board = new C4Board();
                    displayBoardModel = true;

                    //Create a C4 Board GameObject and obtain a reference to its BoardModelController Component
                    GameObject boardModel = Instantiate(Resources.Load("C4 Board", typeof(GameObject))) as GameObject;
                    boardModelController = boardModel.GetComponent<BoardModelController>();
                    boardModelController.Initialise();
                    break;
                case 2:
                    board = new OthelloBoard();
                    displayBoardModel = false;
                    break;
                default:
                    throw new System.Exception("Unknown game type index has been input");
            }

            mcts = new TreeSearch<Node>(board); 

            //Calculate the position of the root node and add an object for it to the scene
            Vector3 rootNodePosition = BoardToPosition(mcts.Root.GameBoard);
            GameObject rootNode = Instantiate(Resources.Load("HashNode"), rootNodePosition, Quaternion.identity) as GameObject;
            rootNode.GetComponent<HashNode>().AddNode(null, mcts.Root, false);
            rootNode.GetComponent<HashNode>().Initialise(rootNodePosition);

            //Add the root node to the position and object map
            nodePositionMap.Add(rootNodePosition, rootNode);
            nodeObjectMap.Add(mcts.Root, rootNode);

            //Create the amount of starting nodes specified by the user
            for (int i = 0; i < HashUIController.GetStartingNodeInput(); i++)
            {
                PerformStep(true);
            }

            //Swap out the current menu panels
            HashUIController.SetMenuPanelActive(false);
            HashUIController.SetNavigationPanelActive(true);
        }

        /// <summary>
        /// Called when the step button is pressed <para/>
        /// Calls the <see cref="PerformStep(bool)"/> method as long as <see cref="playing"/> is false
        /// </summary>
        public void StepButtonPressed()
        {
            if (playing)
            {
                return;
            }

            PerformStep(false);
        }

        /// <summary>
        /// Performs a step of MCTS and creates a gameobject for the new <see cref="Node"/>'s board state if required <para/>
        /// Also creates any relevant links between related nodes
        /// </summary>
        /// <param name="fromMenu">Flag used to indicate whether this step is being performed from the menu or not. Used to prevent the spawning animation if a node is not being added when the visualisation has already been started</param>
        public void PerformStep(bool fromMenu)
        {
            //Perform an iteration of MCTS
            mcts.Step();

            //Get a reference to the newest node
            Node newestNode = mcts.AllNodes[mcts.AllNodes.Count - 1];

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
                GameObject newNodeObject = Instantiate(Resources.Load("HashNode"), fromMenu ? newNodePosition : nodeObjectMap[newestNode.Parent].transform.position, Quaternion.identity) as GameObject;

                //Map the newest node to the new node object
                nodeObjectMap.Add(newestNode, newNodeObject);

                //Map the hashed position of the newest node to the new node object
                nodePositionMap.Add(newNodePosition, newNodeObject);

                AllNodes.Add(newNodeObject.GetComponent<HashNode>());
            }

            //Initialise the newest hash node and add a mcts Node to it
            nodeObjectMap[newestNode].GetComponent<HashNode>().Initialise(newNodePosition);
            nodeObjectMap[newestNode].GetComponent<HashNode>().AddNode(nodeObjectMap[newestNode.Parent], newestNode, !fromMenu);

            HashUIController.SetTotalNodeText(nodeObjectMap.Count);
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

            for(int y = 0; y < gridBoard.Height; y++) 
            {
                for(int x = 0; x < gridBoard.Width; x++)
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