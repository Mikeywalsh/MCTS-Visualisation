using MCTS.Core;
using MCTS.Core.Games;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MCTS.Visualisation.Festival
{
	public class FestivalClientController : MonoBehaviour {

		/// <summary>
		/// The boardcontroller, which controls the board model used to play the game on
		/// </summary>
		public C4BoardModelController BoardController;

		/// <summary>
		/// The column highlighting gameobject, used to indicate selected columns
		/// </summary>
		public GameObject columnHighlighter;

		/// <summary>
		/// TextMesh indicating who's turn it is, or the winner at the end of the game
		/// </summary>
		public TextMeshPro TurnText;

		/// <summary>
		/// The ID of the current selected column, -1 if no columns are selected
		/// </summary>
		private int selectedColumnID = -1;

		/// <summary>
		/// Flag indicating if a move can be made in the chosen column
		/// </summary>
		private bool validMove = false;

		/// <summary>
		/// The board being played on
		/// </summary>
		private C4Board board;

		/// <summary>
		/// The client object used to communicate with a remote server
		/// </summary>
		private GameClient client;

		/// <summary>
		/// The connection panel for the client
		/// </summary>
		public GameObject ClientWaitingPanel;

		/// <summary>
		/// Input field for the IP address of the server
		/// </summary>
		public InputField InputIPAddress;

		/// <summary>
		/// Input field for the port of the server
		/// </summary>
		public InputField InputPort;

		/// <summary>
		/// Reference to the connect button
		/// </summary>
		public Button ConnectButton;

		/// <summary>
		/// Reference to the restart game button
		/// </summary>
		public Button RestartGameButton;

		/// <summary>
		/// Reference to the connecting text
		/// </summary>
		public Text ConnectingText;

		/// <summary>
		/// Reference to the disconnect text;
		/// </summary>
		public Text DisconnectText;

		void Start() {
			//Create a new game board
			board = new C4Board();

			//Initialise the game client
			client = new GameClient(Connected, ClientConnectionFailed, MakeMoveOnBoard, HandleDisconnect, RestartGame);			

			//Show the client waiting panel and hide the gameboard and turn text
			ClientWaitingPanel.SetActive(true);
			BoardController.gameObject.SetActive(false);
			TurnText.gameObject.SetActive(false);
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.P))
			{
				//Make the move on the client object so that it can be sent to the server
				client.ClientMove = new C4Move(0);

				//Make the move locally
				MakeMoveOnBoard(0);
			}
			
			//Check to see if the user is hovering over a column
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			//If the user is hovered over a column and it is the local players turn and the current game is not over
			if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<C4BoardColumn>() != null && board.CurrentPlayer == 1 && board.Winner == -1 && Input.touchCount == 1)
			{
				//Get a reference to the column that was hit and set the current selected column ID
				C4BoardColumn hitColumn = hit.transform.GetComponent<C4BoardColumn>();
				selectedColumnID = hitColumn.ColumnID;

				//Determine if a move can be made in this column, if it is full then a move cannot be made
				validMove = (board.GetCell(selectedColumnID, 5) == 0);

				//Change the colour of the column highlighter to indicate if this move would be valid or not
				columnHighlighter.GetComponent<Renderer>().material.color = new Color(validMove? 0:1, validMove? 1:0, 0, 125 / 255f);

				//Set the column highlighter to active and move it to the highlighted column
				columnHighlighter.SetActive(true);
				columnHighlighter.transform.position = new Vector3(0, 9.5f, hitColumn.transform.position.z + 9);
			}
			else
			{
				//If not hovering over a column, hide the column highlighter and set the current selected column ID to -1
				columnHighlighter.SetActive(false);
				selectedColumnID = -1;
			}

			//If it is the local users turn, and they have selected a valid column and clicked, then make a move
			if(Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended && selectedColumnID != -1 && validMove)
			{
				//Make the move on the client object so that it can be sent to the server
				client.ClientMove = new C4Move(selectedColumnID);

				//Make the move locally
				MakeMoveOnBoard(selectedColumnID);
			}
		}

		private void MakeMoveOnBoard(Move move)
		{
			MakeMoveOnBoard(((C4Move)move).X);
		}

		private void MakeMoveOnBoard(int col)
		{
			//Make a move on the current board
			board.MakeMove(new C4Move(col));

			//Update the board model
			BoardController.SetBoard(board);

			//Update the turn text
			if (board.Winner == 2)
			{
				TurnText.text = "The AI has won, unlucky...";
			}
			else if (board.Winner == 1)
			{
				TurnText.text = "You have won! Congratulations!";
			}
			else if(board.Winner == 0)
			{
				TurnText.text = "You have drew!";
			}
			else
			{
				if(board.CurrentPlayer == 1)
				{
					TurnText.text = "Your Turn...";
				}
				else if(board.CurrentPlayer == 2)
				{
					TurnText.text = "AI's Turn...";
				}
			}

			//Show the reset game button if the game is over or it is the local players turn
			if(board.Winner != -1 || board.CurrentPlayer == 1)
			{
				RestartGameButton.gameObject.SetActive(true);
			}
			else
			{
				RestartGameButton.gameObject.SetActive(false);

			}
		}

		public void AttemptClientConnection()
		{
			//Check the validity of the input IP and port
			IPAddress serverIP;
			short serverPort;

			if (IPAddress.TryParse(InputIPAddress.text, out serverIP) && short.TryParse(InputPort.text, out serverPort))
			{
				//Disable the connect button so that the user has to wait while a connection is attempted
				ConnectButton.gameObject.SetActive(false);
				ConnectingText.gameObject.SetActive(true);

				//Attempt a connection with the input IP and port
				client.AttemptConnect(IPAddress.Parse(InputIPAddress.text), short.Parse(InputPort.text));
			}
		}

		#region Connection related callbacks
		private void ClientConnectionFailed()
		{
			//Re-enable the connect button to allow the user to try to connect again
			Debug.Log("Could not connect to server, please try again...");

			//Only do this if the user hasn't left the scene
			if (ConnectButton != null && ConnectingText != null)
			{
				ConnectButton.gameObject.SetActive(true);
				ConnectingText.gameObject.SetActive(false);
			}
		}

		private void Connected()
		{
			//Hide the waiting panel
			ClientWaitingPanel.SetActive(false);

			//Start a new game
			RestartGame();

			//Show the gameboard and turn text, and hide the disconnect text
			BoardController.gameObject.SetActive(true);
			TurnText.gameObject.SetActive(true);
			DisconnectText.gameObject.SetActive(false);
		}

		private void HandleDisconnect()
		{
			//Restart the scene without reloading it
			Start();

			//Re-enable the connect button
			ConnectButton.gameObject.SetActive(true);
			ConnectingText.gameObject.SetActive(false);

			//Alert the user that they were disconnected
			DisconnectText.gameObject.SetActive(true);
		}

		private void ResetScene()
		{
			SceneManager.LoadScene(0);
		}
		#endregion

		public void RestartButtonPressed()
		{
			if (board.Winner == -1 && !client.ResetFlag)
			{
				client.ResetFlag = true;
				return;
			}

			RestartGame();
		}

		public void RestartGame()
		{
			//Create a new game board
			board = new C4Board();

			//Set the initial board state
			BoardController.Initialise();

			//Set the board reference on the client object to the new board
			client.GameBoard = board;

			//Hide the restart game button
			RestartGameButton.gameObject.SetActive(false);

			//Reset the turn text
			TurnText.text = "Your Turn...";
		}
	}
}
