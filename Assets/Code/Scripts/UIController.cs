using System.Collections;
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
    /// Ran when the program starts
    /// There should only ever be one object with a UIController, so this should only be run once
    /// </summary>
    public void Start()
    {
        //Set the singleton reference when the program is first run
        if (uiController == null)
            uiController = this;
    }

    /// <summary>
    /// Displays information about the referenced <see cref="Node"/> object to the UI
    /// </summary>
    /// <param name="n">The node object to display the information of</param>
    public static void DisplayNodeInfo(Node n)
    {
        uiController.CurrentNodeText.text = "Current Node Depth: " + n.Depth +
                                            "\nTotal Score: " + n.TotalScore +
                                            "\nAverage Score: " + n.AverageScore +
                                            "\nVisits: " + n.Visits +
                                            "\nContents: " +
                                            n.GameBoard.ToString();
    }
}
