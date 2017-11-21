using UnityEngine.SceneManagement;

namespace MCTS.Visualisation
{
    /// <summary>
    /// Convenience class used to handle loading of different scenes
    /// </summary>
    public static class SceneController
    {
        /// <summary>
        /// Called when the back to menu button is pressed <para/>
        /// Changes the current scene to be the main menu
        /// </summary>
        public static void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Change the current scene to be the tree visualisation scene
        /// </summary>
        public static void LoadTreeVisualisation()
        {
            SceneManager.LoadScene("TreeVisualisation");
        }

        /// <summary>
        /// Change the current scene to be the hashing visualisation scene
        /// </summary>
        public static void LoadHashingVisualisation()
        {
            SceneManager.LoadScene("HashingVisualisation");
        }

        /// <summary>
        /// Change the current scene to be the play connect four scene
        /// </summary>
        public static void LoadPlayConnectFour()
        {
            SceneManager.LoadScene("PlayConnect4");
        }

        /// <summary>
        /// Reloads the current scene
        /// </summary>
        public static void ResetCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}