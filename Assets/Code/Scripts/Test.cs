using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {

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
            lastUpdateTime = Time.time;
            Debug.Log(mcts.Plays);
        }

        if (!resultShown && mcts.Finished)
        {
            Debug.Log("----------------");
            Debug.Log("Finished!");
            Debug.Log("Total nodes: " + mcts.Plays);
            Node bestNode = mcts.Root;

            foreach(Node c in bestNode.Children)
            {
                Debug.Log("Score: " + c.AverageScore + "    Total: " + c.TotalScore + "     Visits: " + c.Visits + "     Children: " + c.Children.Count + c.GameState);
            }
            Debug.Log("----------------");

            while(true)
            {
                bestNode = mcts.BestNodeChoice(bestNode);
                if (bestNode == null)
                    break;

                Debug.Log("Choose: " + bestNode.AverageScore + bestNode.GameState);
            }

            resultShown = true;
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
