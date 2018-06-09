using UnityEngine;
using UnityEngine.UI;
using MCTS.Core;

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
		/// The dropdown menu that allows the user to select which game to run MCTS on
		/// </summary>
		public Dropdown GameChoiceDropdown;

		/// <summary>
		/// Input field specifying the starting amount amount of nodes to create
		/// </summary>
		public InputField StartingNodeAmountField;

		/// <summary>
		/// The panel in which all navigation UI elements are held
		/// </summary>
		public GameObject NavigationPanel;

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
		/// The start button, which is used to start the hash visualisation
		/// </summary>
		public Button StartButton;

		/// <summary>
		/// The play button, which is used to start the playing animation
		/// </summary>
		public Button PlayButton;

		/// <summary>
		/// The pause button, which is used to pause the playing animation
		/// </summary>
		public Button PauseButton;

		/// <summary>
		/// The step button, which is used to perform one MCTS step at a time
		/// </summary>
		public Button StepButton;

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
		/// Gets the user input amount of starting nodes to create from the <see cref="StartingNodeAmountField"/> input field
		/// </summary>
		/// <returns>The user input amount of starting nodes to create</returns>
		public static int GetStartingNodeInput()
		{
			return int.Parse(controller.StartingNodeAmountField.text);
		}

		/// <summary>
		/// Gets the current selected game choice and returns it as an integer index value
		/// </summary>
		public static int GetGameChoice
		{
			get
			{
				return controller.GameChoiceDropdown.value;
			}
		}

		/// <summary>
		/// Validates the starting node input when it is changed, so that the start button cannot be pressed until it is valid
		/// </summary>
		public void ValidateInput()
		{
			if (StartingNodeAmountField.text.Length == 0)
			{
				StartButton.interactable = false;
			}
			else
			{
				StartButton.interactable = true;
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
		/// Called when the play button is pressed <para/>
		/// Stops the playing animation, disables the play button and re-enables the pause button
		/// </summary>
		public static void PlayButtonPressed()
		{
			controller.PlayButton.gameObject.SetActive(false);
			controller.StepButton.gameObject.SetActive(false);
			controller.PauseButton.gameObject.SetActive(true);
		}

		/// <summary>
		/// Called when the pause button is pressed <para/>
		/// Stops the playing animation, disables the pause button and re-enables the play button
		/// </summary>
		public static void PauseButtonPressed()
		{
			controller.PlayButton.gameObject.SetActive(true);
			controller.StepButton.gameObject.SetActive(true);
			controller.PauseButton.gameObject.SetActive(false);
		}

		/// <summary>
		/// Called when the back to menu button is pressed <para/>
		/// Changes the current scene to be the main menu
		/// </summary>
		public void BackToMenuButtonPressed()
		{
			SceneController.LoadMainMenu();
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