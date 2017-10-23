using UnityEngine;
using UnityEngine.UI;

public class HashUIController : MonoBehaviour {

    private static HashUIController controller;

    public GameObject MenuPanel;

    public Text StartingNodeAmountField;

    public GameObject NavigationPanel;

    public Text TotalNodeText;

    public void Start()
    {
        controller = this;
    }

    public static void SetTotalNodeText(int newTotal)
    {
        controller.TotalNodeText.text = "Total Nodes: " + newTotal;
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
}
