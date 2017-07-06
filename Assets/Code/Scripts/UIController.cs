using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singleton used to control the UI
/// </summary>
public class UIController : MonoBehaviour {

    /// <summary>
    /// The singleton reference for the UI controller being used
    /// </summary>
    private static UIController uiController;

    /// <summary>
    /// The text box used to display information about the current node in the UI
    /// </summary>
    public Text CurrentNodeText;

    /// <summary>
    /// TODO - Dynamic button creation with a Scroll View instead of a predetermined amount of buttons
    /// The transform of the gameobject that 'holds' the different child select buttons
    /// Each child of this transform is a buttom for selecting a child
    /// </summary>
    public Transform ChildButtonHolder;

    /// <summary>
    /// TODO - Dynamic button creation with a Scroll View instead of a predetermined amount of buttons 
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
    private static Color32 DEFAULT_BUTTON_COLOR = new Color32(255,255,255,140);

    /// <summary>
    /// The highlighted color for buttons
    /// </summary>
    private static Color32 HIGHLIGHTED_BUTTON_COLOR = new Color32(255, 255, 0, 200);

    /// <summary>
    /// Ran when the program starts
    /// There should only ever be one object with a UIController, so this should only be run once
    /// </summary>
    public void Start()
    {
        //Set the singleton reference when the program is first run
        if (uiController == null)
            uiController = this;

        //Setup the list of child buttons to use for displaying child node information
        ChildButtons = new List<Button>();

        //Add each child button to the list of child buttons
        for (int i = 0; i < ChildButtonHolder.childCount; i++)
        {
            ChildButtons.Add(ChildButtonHolder.GetChild(i).GetComponent<Button>());
        }
    }

    /// <summary>
    /// Displays information about the referenced <see cref="Node"/> object to the UI
    /// Will also update the child buttons with details about the referenced nodes children
    /// </summary>
    /// <param name="n">The node object to display the information of</param>
    public static void DisplayNodeInfo(Node n)
    {
        //Show information about the current node
        uiController.CurrentNodeText.text = "Current Node Depth: " + n.Depth +
                                            "\nTotal Score: " + n.TotalScore +
                                            "\nAverage Score: " + n.AverageScore +
                                            "\nVisits: " + n.Visits +
                                            "\nContents: " +
                                            n.GameBoard.ToString();

        //Calculate the best child of the current node, so that the user can see the most optimal choice
        Node bestChild = null;
        float bestChildScore = float.MinValue;
        
        foreach(Node child in n.Children)
        {
            if(child.AverageScore > bestChildScore)
            {
                bestChild = child;
                bestChildScore = bestChild.AverageScore;
            }
        }

        //TODO - Dynamic button creation with a Scroll View instead of a predetermined amount of buttons
        //Update the child buttons with details about the referenced nodes children
        for(int i = 0; i < uiController.ChildButtons.Count; i++)
        {
            if (i < n.Children.Count)
            {
                //Enable the button and set its text appropriately if there is a child for this button
                uiController.ChildButtons[i].gameObject.SetActive(true);
                uiController.ChildButtons[i].GetComponentInChildren<Text>().text = "Child: " + (i + 1) +
                                                                                    "\nAvg Score: " + n.Children[i].AverageScore.ToString("0.00");

                //Highlight the button if its corresponding node is the best child node to choose
                if(n.Children[i] == bestChild)
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
}
