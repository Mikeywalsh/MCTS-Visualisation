using UnityEngine;
using UnityEngine.UI;
using MCTS.Core;

public class HashUIController : MonoBehaviour {

    private static HashUIController controller;

    public GameObject MenuPanel;

    public Text StartingNodeAmountField;

    public GameObject NavigationPanel;

    public Text TotalStepsText;

    public GameObject BoardInfoPanel;

    public GameObject NodeInfoPanel;

    public Text BoardInfoText;

    public Text NodeInfoText;

    public Button PlayButton;

    public Button PauseButton;

    public Button StepButton;

    public void Start()
    {
        controller = this;
    }

    public static void SetTotalNodeText(int newTotal)
    {
        controller.TotalStepsText.text = "Total Steps: " + newTotal;
    }

    public static void SetMenuPanelActive(bool active)
    {
        controller.MenuPanel.SetActive(active);
    }

    public static void SetNavigationPanelActive(bool active)
    {
        controller.NavigationPanel.SetActive(active);
    }

    public static int GetStartingNodeInput()
    {
        return int.Parse(controller.StartingNodeAmountField.text);
    }

    public static void DisplayBoardInfo(Board board, int nodeCount)
    {
        controller.NodeInfoPanel.SetActive(true);

        controller.BoardInfoText.text = board.ToRichString() + "\n\n <color=ffffff>Nodes: " + nodeCount + "</color>";
    }

    public static void HideBoardInfo()
    {
        controller.NodeInfoPanel.SetActive(false);
        controller.BoardInfoPanel.SetActive(false);
    }

    public static void DisplayNodeInfo(Node n, int index)
    {
        //Show information about the current node
        controller.BoardInfoPanel.SetActive(true);

        controller.NodeInfoText.text =  "Node: " + (index + 1) +
                                        "\nCurrent Node Depth: " + n.Depth +
                                        "\nCurrent Player: " + n.GameBoard.CurrentPlayer +
                                        "\nTotal Score: " + n.TotalScore +
                                        "\nAverage Score: " + n.AverageScore +
                                        "\nVisits: " + n.Visits;
    }

    public static void PlayButtonPressed()
    {
        controller.PlayButton.gameObject.SetActive(false);
        controller.StepButton.gameObject.SetActive(false);
        controller.PauseButton.gameObject.SetActive(true);
    }

    public static void PauseButtonPressed()
    {
        controller.PlayButton.gameObject.SetActive(true);
        controller.StepButton.gameObject.SetActive(true);
        controller.PauseButton.gameObject.SetActive(false);
    }
}
