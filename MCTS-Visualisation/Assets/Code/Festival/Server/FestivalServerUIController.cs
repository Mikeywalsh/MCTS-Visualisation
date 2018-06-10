using UnityEngine;
using UnityEngine.UI;
using MCTS.Core;
using System.Net;
using System.Net.Sockets;

namespace MCTS.Visualisation.Festival
{
	public class FestivalServerUIController : MonoBehaviour
	{
		/// <summary>
		/// A singleton reference to the UI controller
		/// </summary>
		private static FestivalServerUIController controller;

		/// <summary>
		/// The panel in which all menu UI elements are held
		/// </summary>
		public GameObject MenuPanel;

		/// <summary>
		/// Input field specifying the port that the server will listen on
		/// </summary>
		public InputField ServerPortField;

		/// <summary>
		/// The panel in which all navigation UI elements are held
		/// </summary>
		public GameObject NavigationPanel;

		/// <summary>
		/// Text which shows the local IPv4 address of this machine
		/// </summary>
		public Text IPAddressText;

		/// <summary>
		/// The text field which displays the total amount of steps that have been performed
		/// </summary>
		public Text TotalStepsText;

		/// <summary>
		/// The panel which displays the current board as rich text <para/>
		/// Used if the displayBoardModel flag of <see cref="FestivalServerController"/> is set to false
		/// </summary>
		public GameObject BoardInfoPanelText;

		/// <summary>
		/// The panel which displays the current board as a 3D model <para/>
		/// Used if the displayBoardModel flag of <see cref="FestivalServerController"/> is set to true
		/// </summary>
		public GameObject BoardInfoPanelModel;

		/// <summary>
		/// The panel which shows information about the current node
		/// </summary>
		public GameObject NodeInfoPanel;

		/// <summary>
		/// The text field which shows the string representation of the current board
		/// </summary>
		public Text BoardInfoText;

		/// <summary>
		/// The text field which shows the metadata about the current node
		/// </summary>
		public Text NodeInfoText;

		/// <summary>
		/// The text field which alerts the user that the server is listening for a client connection
		/// </summary>
		public Text ConnectionText;

		/// <summary>
		/// The start button, which is used to start listening for a client connection
		/// </summary>
		public Button StartServerButton;

		/// <summary>
		/// Assigns the singleton reference when the scene is loaded, throwing an exception if one already exists
		/// </summary>
		public void Start()
		{
			//If the singleton reference is not null, throw an exception
			if (controller != null)
			{
				throw new System.Exception("Singleton reference already exists!");
			}

			//Assign the singleton reference
			controller = this;

			//Set the IP address text
			SetIPAddressText();
		}

		/// <summary>
		/// Sets the IP address text field to the IPv4 address of this machine
		/// </summary>
		public static void SetIPAddressText()
		{
			//Obtain the IP address of this machine
			IPAddress[] allLocalIPs = Dns.GetHostAddresses(Dns.GetHostName());
			IPAddress localIP = IPAddress.Parse("127.0.0.1");

			//Find the IPv4 address of this machine
			foreach (IPAddress address in allLocalIPs)
			{
				if (address.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = address;
					break;
				}
			}

			//Set the text of the IP address field
			controller.IPAddressText.text = localIP.ToString();
		}

		/// <summary>
		/// Called when the server starts listening, makes the port field uninteractable and swaps the connection button for text
		/// </summary>
		public static void BeginListening()
		{
			controller.ServerPortField.interactable = false;
			controller.StartServerButton.gameObject.SetActive(false);
			controller.ConnectionText.gameObject.SetActive(true);
		}

		/// <summary>
		/// Called whenever a connection has been made, or listening has failed
		/// </summary>
		public static void StopListening()
		{
			controller.ServerPortField.interactable = true;
			controller.StartServerButton.gameObject.SetActive(true);
			controller.ConnectionText.gameObject.SetActive(false);
		}

		/// <summary>
		/// Set the total node text to the specified amount
		/// </summary>
		/// <param name="newTotal">The new total node amount</param>
		public static void SetTotalNodeText(int newTotal)
		{
			controller.TotalStepsText.text = "Total Steps: " + newTotal;
		}

		/// <summary>
		/// Shows or hides the menu panel depending on the provided flag
		/// </summary>
		/// <param name="active">True if showing the menu panel, false if hiding it</param>
		public static void SetMenuPanelActive(bool active)
		{
			controller.MenuPanel.SetActive(active);
		}

		/// <summary>
		/// Shows or hides the navigation panel depending on the provided flag
		/// </summary>
		/// <param name="active">True if showing the navigation panel, false if hiding it</param>
		public static void SetNavigationPanelActive(bool active)
		{
			controller.NavigationPanel.SetActive(active);
		}

		/// <summary>
		/// Gets the port to listen on from the <see cref="ServerPortField"/>
		/// </summary>
		/// <returns>The user input amount of starting nodes to create</returns>
		public static short GetPortInput()
		{
			return short.Parse(controller.ServerPortField.text);
		}

		/// <summary>
		/// Validates the starting node input when it is changed, so that the start button cannot be pressed until it is valid
		/// </summary>
		public void ValidateInput()
		{
			if (ServerPortField.text.Length != 0 && GetPortInput() > 0)
			{
				StartServerButton.interactable = true;
			}
			else
			{
				StartServerButton.interactable = false;
			}
		}

		/// <summary>
		/// Shows the board info panel and displays information about the current board and the amount of nodes which share it
		/// </summary>
		/// <param name="board">The board to display information about</param>
		public static void DisplayBoardInfo(Board board)
		{
			if (FestivalServerController.Controller.displayBoardModel)
			{
				controller.BoardInfoPanelModel.SetActive(true);
				FestivalServerController.Controller.boardModelController.SetBoard((GridBasedBoard)board);
			}
			else
			{
				controller.BoardInfoPanelText.SetActive(true);
				controller.BoardInfoText.text = board.ToRichString();
			}
		}

		/// <summary>
		/// Hides the board info panel
		/// </summary>
		public static void HideBoardInfo()
		{
			controller.NodeInfoPanel.SetActive(false);
			controller.BoardInfoPanelText.SetActive(false);
			controller.BoardInfoPanelModel.SetActive(false);
		}

		/// <summary>
		/// Displays information about the passed in node
		/// </summary>
		/// <param name="n">The node to display information about</param>
		/// <param name="index">The index of the passed in node in relation to the current hashnode object</param>
		/// <param name="nodeCount">The amount of nodes contained in the currently selected <see cref="HashNode"/></param>
		public static void DisplayNodeInfo(Node n, int index, int nodeCount)
		{
			//Show information about the current node
			controller.NodeInfoPanel.SetActive(true);

			controller.NodeInfoText.text = "Nodes: " + nodeCount +
											"\nCurrent Node: " + (index + 1) +
											"\nCurrent Node Depth: " + n.Depth +
											"\nCurrent Player: " + n.GameBoard.CurrentPlayer +
											"\nTotal Score: " + n.TotalScore +
											"\nAverage Score: " + n.AverageScore +
											"\nVisits: " + n.Visits;
		}

		/// <summary>
		/// Called when the reset button is pressed <para/>
		/// Reloads the current scene
		/// </summary>
		public void ResetButtonPressed()
		{
			FestivalServerController.ResetSpherePositions();
			SceneController.ResetCurrentScene();
		}
	}
}