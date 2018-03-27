using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MCTS.Core;

namespace MCTS.Visualisation.Tree
{
    /// <summary>
    /// A singleton used to control the UI
    /// </summary>
    public class TreeUIController : MonoBehaviour
    {
        /// <summary>
        /// The singleton reference for the UI controller being used
        /// </summary>
        private static TreeUIController uiController;

        /// <summary>
        /// Parent object for all elements of the navigation UI
        /// </summary>
        public GameObject NavigationElements;

        /// <summary>
        /// Parent object for all elements of the menu UI
        /// </summary>
        public GameObject MenuElements;

        /// <summary>
        /// The dropdown menu that allows the user to select which game to run MCTS on
        /// </summary>
        public Dropdown GameChoiceDropdown;

        /// <summary>
        /// The dropdown menu that allows the user to select what visualisation type to use
        /// </summary>
        public Dropdown VisualistationChoiceDropdown;

        /// <summary>
        /// The button which either starts MCTS or finished it early, depending on when it is pressed
        /// </summary>
        public Button StartStopButton;

        /// <summary>
        /// The button which allows the user to return to the main menu
        /// </summary>
        public Button BackToMenuButton;

        /// <summary>
        /// The progress bar which displays how much progress has been made to running MCTS and visualising it
        /// </summary>
        public ProgressBar VisualisationProgressBar;

        /// <summary>
        /// The input field which allows the user to specify the time the MCTS will run for
        /// </summary>
        public InputField TimeToRunInput;

        /// <summary>
        /// The text box used to display information about the current node in the UI
        /// </summary>
        public Text CurrentNodeText;

        /// <summary>
        /// The image being used to represent the current board state <para/>
        /// The image will be inactive if the displayBoardModel flag in <see cref="TreeController"/> is set to false
        /// </summary>
        public GameObject BoardModelDisplay;

        /// <summary>
        /// TODO - Dynamic button creation with a Scroll View instead of a predetermined amount of buttons <para/>
        /// The transform of the gameobject that 'holds' the different child select buttons <para/>
        /// Each child of this transform is a buttom for selecting a child
        /// </summary>
        public Transform ChildButtonHolder;

        /// <summary>
        /// TODO - Dynamic button creation with a Scroll View instead of a predetermined amount of buttons <para/>
        /// A list of buttons for selecting child nodes
        /// </summary>
        public List<Button> ChildButtons;

        /// <summary>
        /// This button allows the user to navigate backwards through the game tree
        /// </summary>
        public Button BackToParentButton;

        /// <summary>
        /// The default color for buttons
        /// </summary>
        private static Color32 DEFAULT_BUTTON_COLOR = new Color32(255, 255, 255, 140);

        /// <summary>
        /// The highlighted color for buttons
        /// </summary>
        private static Color32 HIGHLIGHTED_BUTTON_COLOR = new Color32(255, 255, 0, 200);

        /// <summary>
        /// Ran when the program starts <para/>
        /// There should only ever be one object with a TreeUIController, so this should only be run once
        /// </summary>
        public void Start()
        {
            //If the singleton reference is not null, then there are more than one UIcontrollers, which is not allowed
            if (uiController != null)
                throw new Exception("UIController is a Singleton, there cannot be more than one instance");

            //Set the singleton static reference to be this instance
            uiController = this;

            //Setup the list of child buttons to use for displaying child node information
            ChildButtons = new List<Button>();

            //Add each child button to the list of child buttons
            for (int i = 0; i < ChildButtonHolder.childCount; i++)
            {
                ChildButtons.Add(ChildButtonHolder.GetChild(i).GetComponent<Button>());
                ChildButtonHolder.GetChild(i).GetComponent<Button>().gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// Called when the start button has been <para/>
        /// This will disable all forms on input on the main menu whilst MCTS runs <para/>
        /// The start button is also changed to the stop button
        /// </summary>
        public static void StartButtonPressed()
        {
            //Change the text of the button to inform the user that the MCTS can be finished early
            uiController.StartStopButton.GetComponentInChildren<Text>().text = "Finish Early";

            //Disable every form of input on the main menu
            uiController.GameChoiceDropdown.interactable = false;
            uiController.TimeToRunInput.interactable = false;
            uiController.BackToMenuButton.gameObject.SetActive(false);

            //Enable the progress bar
            uiController.VisualisationProgressBar.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called when the stop button has been pressed, signalling the early finish of the MCTS algorithm <para/>
        /// The start/stop button is disabled whilst the program attempts to visualise the result
        /// </summary>
        public static void StopButtonPressed()
        {
            uiController.StartStopButton.interactable = false;
        }

        /// <summary>
        /// Update the visualisation progress bar with a new progress value and message to display
        /// </summary>
        /// <param name="progress">The new progress value</param>
        /// <param name="message">The message to display</param>
        public static void UpdateProgressBar(float progress, string message)
        {
            uiController.VisualisationProgressBar.SetProgress(progress);
            uiController.VisualisationProgressBar.SetText(message);
        }

        /// <summary>
        /// Called when the MCTS visualisation is ready <para/>
        /// Changes the main menu UI to the tree navigation UI
        /// </summary>
        public static void SwitchToNavigationUI()
        {
            uiController.MenuElements.SetActive(false);
            uiController.NavigationElements.SetActive(true);
        }

        /// <summary>
        /// Displays information about the referenced <see cref="Node"/> object to the UI <para/>
        /// Will also update the child buttons with details about the referenced nodes children
        /// </summary>
        /// <param name="n">The node object to display the information of</param>
        public static void DisplayNodeInfo(Node n)
        {
            //Display an image of the board state the current node represents if the displayBoardModel flag is set on the TreeController
            uiController.BoardModelDisplay.SetActive(TreeController.Controller.displayBoardModel);

            //Show information about the current node
            uiController.CurrentNodeText.text = "Current Node Depth: " + n.Depth +
                                                "\nCurrent Player: " + n.GameBoard.CurrentPlayer +
                                                "\nTotal Score: " + n.TotalScore +
                                                "\nAverage Score: " + n.AverageScore +
                                                "\nVisits: " + n.Visits +
                                                "\nContents: " +
                                                (TreeController.Controller.displayBoardModel ? "" : n.GameBoard.ToRichString());

            //Get the best child node of this node, if it has children nodes that have been simulated
            Node bestChild = n.GetBestChild();

            //TODO - Dynamic button creation with a Scroll View instead of a predetermined amount of buttons
            //Update the child buttons with details about the referenced nodes children
            for (int i = 0; i < uiController.ChildButtons.Count; i++)
            {
                if (i < n.Children.Count)
                {
                    //Enable the button and set its text appropriately if there is a child for this button
                    uiController.ChildButtons[i].gameObject.SetActive(true);
                    uiController.ChildButtons[i].GetComponentInChildren<Text>().text = "Child: " + (i + 1) +
                                                                                        "\nVisits: " + n.Children[i].Visits;

                    //Highlight the button if its corresponding node is the best child node to choose
                    if (n.Children[i] == bestChild)
                    {
                        uiController.ChildButtons[i].GetComponent<Image>().color = HIGHLIGHTED_BUTTON_COLOR;
                    }
                    else
                    {
                        uiController.ChildButtons[i].GetComponent<Image>().color = DEFAULT_BUTTON_COLOR;
                    }
                }
                else
                {
                    //If there is no child for this button, then disable the button
                    uiController.ChildButtons[i].gameObject.SetActive(false);
                }
            }

            //Disable the back to parent button if the new node has no parent
            uiController.BackToParentButton.interactable = (n.Parent != null);
        }

        /// <summary>
        /// Validates the time input when it is changed, so that the start button cannot be pressed until it is valid
        /// </summary>
        public void ValidateInput()
        {
            float timeInput;

            bool validTimeInput = float.TryParse(TimeToRunInput.text, out timeInput);

            if (validTimeInput)
            {
                if (timeInput < 1)
                {
                    validTimeInput = false;
                }
            }

            if (validTimeInput)
            {
                StartStopButton.interactable = true;
            }
            else
            {
                StartStopButton.interactable = false;
            }
        }

        /// <summary>
        /// Gets the current selected game choice and returns it as an integer index value
        /// </summary>
        public static int GetGameChoice
        {
            get
            {
                return uiController.GameChoiceDropdown.value;
            }
        }

        /// <summary>
        /// Gets the current selected visualisation choice and returns it as an integer index value
        /// </summary>
        public static int GetVisualisationChoice
        {
            get
            {
                return uiController.VisualistationChoiceDropdown.value;
            }
        }

        /// <summary>
        /// Gets the current input value from the time to run input field as a float
        /// </summary>
        public static float GetTimeToRunInput
        {
            get
            {
                return float.Parse(uiController.TimeToRunInput.text);
            }
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
            SceneController.ResetCurrentScene();
        }
    }
}