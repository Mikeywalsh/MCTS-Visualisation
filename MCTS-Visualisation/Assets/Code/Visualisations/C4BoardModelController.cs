using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MCTS.Core.Games;
using MCTS.Core;

namespace MCTS.Visualisation
{
    public class C4BoardModelController : BoardModelController
    {
        public Material PlayerOneMat;
        public Material PlayerTwoMat;

        public override void Initialise()
        {
            board = new GameObject[6, 7];

            //Each child of this transform contains a column of counter gameobjects
            for (int x = 0; x < board.GetLength(1); x++)
            {
                for (int y = 0; y < board.GetLength(0); y++)
                {
                    board[y, x] = transform.GetChild(x).GetChild(y).gameObject;
                    board[y, x].SetActive(false);
                }
            }
            
            //Set the board to represent a new Connect 4 game state
            SetBoard(new C4Board());
        }

        public override void SetBoard(GridBasedBoard boardToSet)
        {
            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    if (boardToSet.GetCell(x, y) == 1)
                    {
                        //Show the counter object at this position and have it use the player one material
                        board[y, x].SetActive(true);
                        board[y, x].GetComponent<Renderer>().material = PlayerOneMat;
                    }
                    else if (boardToSet.GetCell(x, y) == 2)
                    {
                        //Show the counter object at this position and have it use the player two material
                        board[y, x].SetActive(true);
                        board[y, x].GetComponent<Renderer>().material = PlayerTwoMat;
                    }
                    else
                    {
                        //Hide the counter object at this position
                        board[y, x].SetActive(false);
                    }
                }
            }
        }
    }
}