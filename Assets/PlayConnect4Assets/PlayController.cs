using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to play a game of connect 4 against an opponent using MCTS
/// This code is hacked together and is not thouroughly tested/of high quality
/// </summary>
public class PlayController : MonoBehaviour {

    private C4Board board;
    private MCTS mcts;
    private Thread aiThread;

    private float timeToRunFor;
    private float timeLeft;

    public GameObject moveButtons;
    public Text boardDisplayText;
    public Text winnerText;
    public Text AITurnProgressText;

	void Start () {
        board = new C4Board();
        boardDisplayText.text = board.ToString();
	}
	
	void Update () {
        if (mcts != null)
        {
            //While the MCTS is still running, display progress information about the time remaining and the amounts of nodes created to the user
            if (!mcts.Finished)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    mcts.Finish();
                }
                AITurnProgressText.text = mcts.NodesVisited + " nodes       " + timeLeft + "/" + timeToRunFor + "s";
                return;
            }

            //If the MCTS has finished, get the best child node choice
            Node bestChild = null;
            float bestChildScore = float.MinValue;

            foreach(Node child in mcts.Root.Children)
            {
                if(child.AverageScore > bestChildScore)
                {
                    bestChild = child;
                    bestChildScore = child.AverageScore;
                }
            }

            //Determine what move was made on the best child
            for(int y = 0; y < board.Height; y++)
            {
                for(int x = 0; x < board.Width; x++)
                {
                    //If 2 cells don't match up, then the move was made in this cell
                    if(board.GetCell(x,y) != ((C4Board)bestChild.GameBoard).GetCell(x,y))
                    {
                        MakeMoveOnBoard(x);
                        mcts = null;
                        moveButtons.SetActive(true);
                    }
                }
            }
        }

	}

    public void StartAITurn()
    {
        //Initialise MCTS on the given game board
        mcts = new MCTS(board, 100);

        //Run mcts in another thread
        aiThread = new Thread(new ThreadStart(() => RunMCTS(mcts)));
        aiThread.IsBackground = true;
        aiThread.Start();
        timeToRunFor = 10;
        timeLeft = timeToRunFor;
        //}
        //else
        //{
        //    //Stop the MCTS early
        //    mcts.Finish();
        //    UIController.StopButtonPressed();
        //}
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

    public void MakeMoveOnBoard(int xPos)
    {
        board.MakeMove(new C4Move(xPos));
        boardDisplayText.text = board.ToString();

        if(board.CurrentPlayer == 2)
        {
            moveButtons.SetActive(false);
            StartAITurn();
        }

        if(board.Winner != -1)
        {
            if(board.Winner == 0)
            {
                winnerText.text = "DRAW";
            }
            else
            {
                winnerText.text = "WINNER IS PLAYER " + board.Winner;
            }
        }
    }
}
