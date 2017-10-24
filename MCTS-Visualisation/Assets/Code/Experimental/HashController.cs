using System.Collections.Generic;
using UnityEngine;
using MCTS.Core;
using MCTS.Core.Games;

public class HashController : MonoBehaviour
{
    public Dictionary<Vector3, GameObject> nodePositionMap = new Dictionary<Vector3, GameObject>();

    public Dictionary<Node, GameObject> nodeObjectMap = new Dictionary<Node, GameObject>();

    public TreeSearch<Node> mcts;

    private float lastSpawn;

    private const float SPAWN_DELAY = 0.1f;

    bool playing = false;

    public void Update()
    {
        //Allow the user to perform a step with the return key or Y button instead of pressing the step button
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("YButton"))
        {
            PerformStep(false);
        }

        if(mcts != null && playing)
        {
            if(Time.time - lastSpawn > SPAWN_DELAY)
            {
                PerformStep(false);
                lastSpawn = Time.time;
            }
        }
    }

    public void PlayButtonPressed()
    {
        playing = true;
        HashUIController.PlayButtonPressed();
    }

    public void PauseButtonPressed()
    {
        playing = false;
        HashUIController.PauseButtonPressed();
    }

    public void StartButtonPressed()
    {
        mcts = new TreeSearch<Node>(new TTTBoard());

        //Calculate the position of the root node and add an object for it to the scene
        Vector3 rootNodePosition = BoardToPosition((TTTBoard)mcts.Root.GameBoard);
        GameObject rootNode = Instantiate(Resources.Load("Ball"), rootNodePosition, Quaternion.identity) as GameObject;
        rootNode.GetComponent<HashNode>().AddNode(null, mcts.Root);

        //Add the root node to the position and object map
        nodePositionMap.Add(rootNodePosition, rootNode);
        nodeObjectMap.Add(mcts.Root, rootNode);

        for (int i = 0; i < HashUIController.GetStartingNodeInput(); i++)
        {
            PerformStep(true);
        }

        //Swap out the current menu panels
        HashUIController.SetMenuPanelActive(false);
        HashUIController.SetNavigationPanelActive(true);
    }

    public void StepButtonPressed()
    {
        if(playing)
        {
            return;
        }

        PerformStep(false);
    }

    public void PerformStep(bool fromMenu)
    {
        //Perform an iteration of MCTS
        mcts.Step();

        //Get a reference to the newest node
        Node newestNode = mcts.AllNodes[mcts.AllNodes.Count - 1];

        //Hash the board contents of the newest node to obtain a positon
        Vector3 newNodePosition = BoardToPosition((TTTBoard)newestNode.GameBoard);

        //If the current board state already exists, then don't create a new node, but create a line to the existing node
        if (nodePositionMap.ContainsKey(newNodePosition))
        {
            //Map the newest node to the existing node object
            nodeObjectMap.Add(newestNode, nodePositionMap[newNodePosition]);
        }
        else
        {
            //Instantiate the new node object at the hashed position
            GameObject newNodeObject = Instantiate(Resources.Load("Ball"), fromMenu? newNodePosition : nodeObjectMap[newestNode.Parent].transform.position, Quaternion.identity) as GameObject;

            //Map the newest node to the new node object
            nodeObjectMap.Add(newestNode, newNodeObject);

            //Map the hashed position of the newest node to the new node object
            nodePositionMap.Add(newNodePosition, newNodeObject);
        }

        //Initialise the newest hash node and add a mcts Node to ite
        nodeObjectMap[newestNode].GetComponent<HashNode>().Initialise(newNodePosition);
        nodeObjectMap[newestNode].GetComponent<HashNode>().AddNode(nodeObjectMap[newestNode.Parent], newestNode);

        HashUIController.SetTotalNodeText(nodeObjectMap.Count);
    }

    public Vector3 BoardToPosition(TTTBoard board)
    {
        float xPos = 0;
        float yPos = 0;
        float zPos = 0;

        for(int y = 0; y < 3; y++)
        {
            xPos += Mathf.Pow(3, y) * board.GetCell(0, y);
            yPos += Mathf.Pow(3, y) * board.GetCell(1, y);
            zPos += Mathf.Pow(3, y) * board.GetCell(2, y);
        }

        return new Vector3(xPos, yPos, zPos) * 2;
    }

}
