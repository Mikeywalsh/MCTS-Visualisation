using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {

    MCTS mcts;
    bool resultShown;

	void Start () {
        Application.runInBackground = true;
        mcts = new MCTS();
        Thread thread1 = new Thread(new ThreadStart(() => RunMCTS(mcts)));
        thread1.Start();
    }
	
	void Update () {
        if (!mcts.Finished)
            Debug.Log(mcts.Plays);

        if (!resultShown && mcts.Finished)
        {
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
