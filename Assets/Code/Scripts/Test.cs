using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject startObject;

    private float timeToRunFor;
    private float timeLeft;

    MCTS mcts;
    bool resultShown;
    bool allNodesGenerated;

    float lastUpdateTime;

    int nodesGenerated = 0;

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
            //Initialise MCTS
            mcts = new MCTS(new TTTBoard(), UIController.GetPlayoutInput);

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
    /// Temporary and very messy
    /// </summary>
	void Update () {
        if (mcts == null)
            return;

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

        if (!resultShown && mcts.Finished)
        {
            Debug.Log("----------------");
            Debug.Log("Finished!");
            Debug.Log("Total nodes: " + mcts.NodesVisited);
            Node bestNode = mcts.Root;
            Debug.Log("----------------");

            while(true)
            {
                bestNode = mcts.BestNodeChoice(bestNode);
                if (bestNode == null)
                    break;

                Debug.Log("Choose: " + bestNode.AverageScore + bestNode.GameBoard);
            }
            resultShown = true;
            //-------------------------------
            startObject.GetComponent<NodeObject>().Initialise(mcts.Root);
            StartCoroutine(GenChildren(mcts.Root, startObject));
            //-------------------------------
        }

        if (!allNodesGenerated && mcts.Finished)
        {
            if (nodesGenerated < mcts.NodesVisited)
            {
                UIController.UpdateProgressBar(0.5f + ((float)nodesGenerated / mcts.NodesVisited / 2), "Creating node objects: " + nodesGenerated + "/" + mcts.NodesVisited);
            }
            else if(nodesGenerated == mcts.NodesVisited)
            {
                UIController.SwitchToNavigationUI();
                Camera.main.GetComponent<LineDraw>().linesVisible = true;
                UIController.DisplayNodeInfo(mcts.Root);
                allNodesGenerated = true;
            }
        }
    }

    IEnumerator GenChildren(Node root, GameObject rootObject)
    {
        foreach (Node child in root.Children)
        {
            GameObject newNode = Instantiate(Resources.Load<GameObject>("Ball"));
            newNode.transform.parent = rootObject.transform;
            newNode.name = "D" + child.Depth + " C" + newNode.transform.parent.childCount + "/" + root.Children.Capacity;
            newNode.AddComponent<NodeObject>().Initialise(child);
            nodesGenerated++;

            yield return new WaitForSeconds(.1f);

            StartCoroutine(GenChildren(child, newNode));
        }
    }

    static void RunMCTS(MCTS m)
    {
        while (!m.Finished)
        {
            m.Step();
        }
    }
}

