using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject startObject;
    public float TimeToRunFor;

    MCTS mcts;
    bool resultShown;

    float lastUpdateTime;

	void Start () {
        Application.runInBackground = true;
        mcts = new MCTS(new TTTBoard(), 100);
        Thread thread1 = new Thread(new ThreadStart(() => RunMCTS(mcts)));
        thread1.IsBackground = true;
        thread1.Start();
        
        #region Fibbonacci Sphere algorithm
        //List<Vector3> points = new List<Vector3>();
        //int samples = parent.TreeNode.Children.Capacity + 1;
        //float offset = 2f / samples;
        //float increment = Mathf.PI * (3 - Mathf.Sqrt(5));

        //for (int i = 0; i < samples; i++)
        //{
        //    float y = ((i * offset) - 1) + (offset / 2);
        //    float r = Mathf.Sqrt(1 - Mathf.Pow(y, 2));

        //    float phi = ((i + 1) % samples) * increment;

        //    float x = Mathf.Cos(phi) * r;
        //    float z = Mathf.Sin(phi) * r;

        //    points.Add(new Vector3(x, y, z));
        //}
        #endregion
    }
	
	void Update () {
        if (!mcts.Finished && lastUpdateTime + 1 < Time.time)
        {
            if (Time.time > TimeToRunFor)
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
            UIController.DisplayNodeInfo(mcts.Root);
            //-------------------------------

        }

        //if(NodeObject.selectedNode != null)
        //{
        //    NodeObject.selectedNode.transform.parent.GetComponentInChildren<Transform>(true).gameObject.SetActive(false);
        //    NodeObject.selectedNode.gameObject.GetComponentInChildren<Transform>(true).gameObject.SetActive(true);
        //    gameObject.SetActive(false);
        //    toggleSelected = false;
        //    NodeObject.selectedNode = null;
        //}
    }

    IEnumerator GenChildren(Node root, GameObject rootObject)
    {
        foreach (Node child in root.Children)
        {
            GameObject newNode = Instantiate(Resources.Load<GameObject>("Ball"));
            newNode.transform.parent = rootObject.transform;
            newNode.name = "D" + child.Depth + " C" + newNode.transform.parent.childCount + "/" + root.Children.Capacity;
            newNode.AddComponent<NodeObject>().Initialise(child);

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

