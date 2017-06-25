using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject startObject;

    MCTS mcts;
    bool resultShown;

    float lastUpdateTime;

	void Start () {
        Application.runInBackground = true;
        mcts = new MCTS(new TTTBoard(), 100);
        Thread thread1 = new Thread(new ThreadStart(() => RunMCTS(mcts)));
        thread1.Start();
    }
	
	void Update () {
        if (!mcts.Finished && lastUpdateTime + 1 < Time.time)
        {
            if (Time.time > 10)
            {
                mcts.FinishEarly();
            }
            lastUpdateTime = Time.time;
            Debug.Log(mcts.NodesVisited);
        }

        if (!resultShown && mcts.Finished)
        {
            Debug.Log("----------------");
            Debug.Log("Finished!");
            Debug.Log("Total nodes: " + mcts.NodesVisited);
            Node bestNode = mcts.Root;

            foreach(Node c in bestNode.Children)
            {
                Debug.Log("Score: " + c.AverageScore + "    Total: " + c.TotalScore + "     Visits: " + c.Visits + "     Children: " + c.Children.Count + c.GameBoard);
            }
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
    }

    IEnumerator GenChildren(Node root, GameObject rootObject)
    {
        foreach (Node child in root.Children)
        {
            GameObject newNode = new GameObject();
            newNode.transform.parent = rootObject.transform;
            newNode.name = "D" + child.Depth + " C" + newNode.transform.parent.childCount;
            newNode.AddComponent<NodeObject>().Initialise(child);

            yield return new WaitForSeconds(1f);

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
