using UnityEngine;
using UnityEngine.SceneManagement;

namespace MCTS.Visualisation.Menu
{
    /// <summary>
    /// A singleton which controls the logic behind user interaction with UI elements in the main menu
    /// </summary>
    public class UIController : MonoBehaviour
    {
        /// <summary>
        /// A singleton reference to the UIController
        /// </summary>
        private UIController controller;

        /// <summary>
        /// Assign the singleton reference when the scene has started
        /// </summary>
        public void Start()
        {
            //If a singleton reference already exists, throw an exception explaining so
            if(controller != null)
            {
                throw new System.Exception("UIController singleton is already assigned tO");
            }

            //Assign the singleton reference
            controller = this;
        }

        /// <summary>
        /// This method will be called when the tree visualisation button is pressed
        /// </summary>
        public static void TreeVisualisationButtonPressed()
        {
            SceneManager.LoadScene("TreeVisualisation");
        }

        /// <summary>
        /// This method will be called when the hashing visualisation button is pressed
        /// </summary>
        public static void HashingVisualisationButtonPressed()
        {
            SceneManager.LoadScene("HashingVisualisation");
        }

        /// <summary>
        /// This method will be called when the play connect four button is pressed
        /// </summary>
        public static void PlayConnectFourButtonPressed()
        {
            SceneManager.LoadScene("PlayConnect4");
        }
    }
}