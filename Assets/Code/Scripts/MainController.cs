using System.Collections;
using System.Threading;
using UnityEngine;

/// <summary>
/// The main script for the application
/// Handles the running of the MCTS and the creation of <see cref="NodeObject"/>s for each node
/// Also handles the changeover between the menu UI and navigation UI
/// </summary>
public class MainController : MonoBehaviour {

    /// <summary>
    /// The gameobject of the root node, used as a starting point for rendering the game tree
    /// </summary>
    public NodeObject RootNodeObject;

    /// <summary>
    /// The MCTS instance used to create the game tree
    /// Should be ran on another thread to avoid freezing of the application
    /// </summary>
    private MCTS mcts;

    /// <summary>
    /// The time to run MCTS for, input by the user
    /// </summary>
    private float timeToRunFor;

    /// <summary>
    /// The time left to run the MCTS for
    /// Starts at <see cref="timeToRunFor"/> and counts down to zero
    /// </summary>
    private float timeLeft;

    /// <summary>
    /// Used as a toggle to start the visualisation process of assigning each node a gameobject
    /// </summary>
    bool startedVisualisation;

    /// <summary>
    /// Used as a flag to display progress made on the visualisation process whilst each nodes does not have a gameobject assigned
    /// </summary>
    bool allNodesGenerated;

    /// <summary>
    /// The current amount of node objects that have been created
    /// </summary>
    int nodesGenerated;

    /// <summary>
    /// Ensure that the application runs in the background when it is started
    /// </summary>
    void Start() {
        Application.runInBackground = true;
    }

    /// <summary>
    /// Called when the start/stop button is pressed
    /// If MCTS is not running, then it will be started
    /// If MCTS is running, this will make it finish early
    /// </summary>
    public void StartStopButtonPressed()
    {
        //Starts or ends MCTS depending on when the button is pressed
        if (mcts == null)
        {
            //Create an empty board instance, which will have whatever game the user chooses assigned to it
            Board board;

            //Assign whatever game board the user has chosen to the board instance
            switch(UIController.GetGameChoice)
            {
                case 0:
                    board = new TTTBoard();
                    break;
                case 1:
                    board = new C4Board();
                    break;
                default:
                    throw new System.Exception("Unknown game type index has been input");
            }

            //Initialise MCTS on the given game board
            mcts = new MCTS(board, UIController.GetPlayoutInput);

            //Run mcts in another thread
            Thread mctsThread = new Thread(new ThreadStart(() => RunMCTS(mcts)));
            mctsThread.IsBackground = true;
            mctsThread.Start();
            timeToRunFor = UIController.GetTimeToRunInput;
            timeLeft = timeToRunFor;
            UIController.StartButtonPressed();
        }
        else
        {
            //Stop the MCTS early
            mcts.Finish();
            UIController.StopButtonPressed();
        }
    }
	
    /// <summary>
    /// If the user has started running MCTS, then display information about it to the UI whilst it generates
    /// When the MCTS has finished generating, start created a <see cref="NodeObject"/> for each <see cref="Node"/>, so that they can be rendered on-screen
    /// When each Node has a corresponding NodeObject, switch to the tree navigation UI
    /// </summary>
	void Update () {
        //Don't do anything until the user has started running the MCTS
        if (mcts == null)
            return;

        //While the MCTS is still running, display progress information about the time remaining and the amounts of nodes created to the user
        if (!mcts.Finished)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                mcts.Finish();
                UIController.StopButtonPressed();
            }
            UIController.UpdateProgressBar((1 - (timeLeft / timeToRunFor)) / 2, "Running MCTS   " + mcts.NodesVisited + " nodes     " + timeLeft.ToString("0.0") + "s/" + timeToRunFor.ToString("0.0") + "s");
        }

        //Return if the MCTS has not finished being created
        if (!mcts.Finished)
            return;

        //If the MCTS has finished being computed, start to create gameobjects for each node in the tree
        if (!startedVisualisation)
        {
            RootNodeObject.Initialise(mcts.Root);
            StartCoroutine(GenChildren(mcts.Root, RootNodeObject.gameObject));
            startedVisualisation = true;
        }

        //Display information on the progress bar about how many node objects have been created, until every node in the tree has its own gameobject
        if (!allNodesGenerated)
        {
            if (nodesGenerated < mcts.NodesVisited)
            {
                UIController.UpdateProgressBar(0.5f + ((float)nodesGenerated / mcts.NodesVisited / 2), "Creating node objects: " + nodesGenerated + "/" + mcts.NodesVisited);
            }
            else if(nodesGenerated == mcts.NodesVisited)
            {
                //If every node has had a gameobject created for it, then switch to the navigation UI and start to render the game tree
                UIController.SwitchToNavigationUI();
                Camera.main.GetComponent<LineDraw>().linesVisible = true;
                UIController.DisplayNodeInfo(mcts.Root);
                allNodesGenerated = true;
            }
        }
    }

    /// <summary>
    /// Creates child gameobjects for a nodes children, recursively
    /// If the root node is passed in as the parent node, then the entire tree will be created
    /// This method is an <see cref="IEnumerator"/> so the tree is given time to be created, instead of the program freezing whilst it creates the tree in one frame
    /// </summary>
    /// <param name="parentNode">The starting node to create a child object hierarchy of </param>
    /// <param name="parentNodeObject">The gameobject of the parent node</param>
    IEnumerator GenChildren(Node parentNode, GameObject parentNodeObject)
    {
        foreach (Node child in parentNode.Children)
        {
            GameObject newNode = Instantiate(Resources.Load<GameObject>("Ball"));
            newNode.transform.parent = parentNodeObject.transform;
            newNode.name = "D" + child.Depth + " C" + newNode.transform.parent.childCount + "/" + parentNode.Children.Capacity;
            newNode.AddComponent<NodeObject>().Initialise(child);
            nodesGenerated++;

            yield return new WaitForSeconds(.1f);

            StartCoroutine(GenChildren(child, newNode));
        }
    }

    /// <summary>
    /// Runs MCTS until completion
    /// Should be ran on another thread to avoid freezing of the application
    /// </summary>
    /// <param name="m">The MCTS instance to run</param>
    static void RunMCTS(MCTS m)
    {
        while (!m.Finished)
        {
            m.Step();
        }
    }
}

