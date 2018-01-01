using UnityEngine;

namespace MCTS.Visualisation.Play
{
    public class PlayCameraControl : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Vector3.zero);
            transform.RotateAround(Vector3.zero, Vector3.up, 1);
        }
    }

}