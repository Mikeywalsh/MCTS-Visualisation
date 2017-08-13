using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// The main script for the application
/// Handles the running of the MCTS and the positioning of each <see cref="NodeObject"/>
/// Also handles the changeover between the menu UI and navigation UI
/// </summary>
public class MainController : MonoBehaviour
{
    /// <summary>
    /// The MCTS instance used to create the game tree
    /// Should be ran on another thread to avoid freezing of the application
    /// </summary>
    private MCTS<NodeObject> mcts;

    /// <summary>
    /// The root node object, which is the starting point for the MCTS
    /// </summary>
    private NodeObject rootNodeObject;

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
    void Start()
    {
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
            switch (UIController.GetGameChoice)
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
            mcts = new MCTS<NodeObject>(board, UIController.GetPlayoutInput);

            //Obtain the time to run mcts for from the input user amount
            timeToRunFor = UIController.GetTimeToRunInput;
            timeLeft = timeToRunFor;

            //Run mcts asyncronously
            RunMCTS(mcts);
            UIController.StartButtonPressed();
        }
        else
        {
            //Stop the MCTS early
            mcts.Finish();
        }
    }

    /// <summary>
    /// If the user has started running MCTS, then display information about it to the UI whilst it generates
    /// When the MCTS has finished generating, set the position of each <see cref="NodeObject"/> so that they can be rendered on-screen
    /// When each nodes position has been set, switch to the tree navigation UI
    /// </summary>
    void Update()
    {
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
            }
            UIController.UpdateProgressBar((1 - (timeLeft / timeToRunFor)) / 2, "Running MCTS   " + mcts.NodesVisited + " nodes     " + timeLeft.ToString("0.0") + "s/" + timeToRunFor.ToString("0.0") + "s");
        }

        //Return if the MCTS has not finished being created
        if (!mcts.Finished)
            return;

        //If the MCTS has finished being computed, start to create gameobjects for each node in the tree
        if (!startedVisualisation)
        {
            rootNodeObject = (NodeObject)mcts.Root;
            rootNodeObject.SetPosition();
            StartCoroutine(SetNodePosition(rootNodeObject));
            startedVisualisation = true;
        }

        //Display information on the progress bar about how many node objects have been created, until every node in the tree has its own gameobject
        if (!allNodesGenerated)
        {
            if (nodesGenerated < mcts.NodesVisited)
            {
                UIController.UpdateProgressBar(0.5f + ((float)nodesGenerated / mcts.NodesVisited / 2), "Creating node objects: " + nodesGenerated + "/" + mcts.NodesVisited);
            }
            else if (nodesGenerated == mcts.NodesVisited)
            {
                //If every node has had a gameobject created for it, then switch to the navigation UI and start to render the game tree
                UIController.SwitchToNavigationUI();
                Camera.main.GetComponent<LineDraw>().linesVisible = true;
                Camera.main.GetComponent<CameraControl>().CurrentNode = rootNodeObject;
                UIController.DisplayNodeInfo(mcts.Root);
                allNodesGenerated = true;
                LineDraw.SelectNode(rootNodeObject);
            }
        }
    }

    /// <summary>
    /// Sets the position of a <see cref="NodeObject"/> in the world, and all its children, recursively
    /// This method is an <see cref="IEnumerator"/> so the tree is given time to be created, instead of the program freezing whilst it creates the tree in one frame
    /// </summary>
    /// <param name="node">The starting node to set the position of</param>
    IEnumerator SetNodePosition(NodeObject node)
    {
        node.SetPosition();

        foreach (Node child in node.Children)
        {
            yield return new WaitForSeconds(.1f);

            StartCoroutine(SetNodePosition((NodeObject)child));
            nodesGenerated++;
        }
    }

    /// <summary>
    /// Runs MCTS until completion asyncronously and then disables the stop button
    /// </summary>
    /// <param name="m">The MCTS instance to run</param>
    static async void RunMCTS(MCTS<NodeObject> mcts)
    {
        await Task.Run(() => { while (!mcts.Finished) { mcts.Step(); } });
        UIController.StopButtonPressed();
    }
}
