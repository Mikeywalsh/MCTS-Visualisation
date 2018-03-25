using UnityEngine;

namespace MCTS.Visualisation.Hashing
{
    /// <summary>
    /// Used for controlling camera movement in the HashVisualisation scene
    /// </summary>
    public class HashCameraControl : MonoBehaviour
    {
        /// <summary>
        /// The speed of the camera, set via inspector
        /// </summary>
        public float Speed;

        /// <summary>
        /// The currently highlighted HashNode
        /// </summary>
        private HashNode currentHighlighted;

        /// <summary>
        /// The currently selected node index on the currently highlighted HashNode
        /// </summary>
        private int currentSelectedNodeIndex;

        void Update()
        {
            if (Forwards())
            {
                transform.Translate(Vector3.forward * 0.1f * Speed);
            }

            if (Backwards())
            {
                transform.Translate(Vector3.forward * -0.1f * Speed);
            }

            if (StrafeLeft())
            {
                transform.Translate(Vector3.right * -0.1f * Speed);
            }

            if (StrafeRight())
            {
                transform.Translate(Vector3.right * 0.1f * Speed);
            }

            if (Upwards())
            {
                transform.Translate(Vector3.up * 0.1f * Speed);
            }

            if (Downwards())
            {
                transform.Translate(Vector3.up * -0.1f * Speed);
            }

            if (PivotLeft())
            {
                transform.Rotate(new Vector3(0, -Speed, 0));
            }

            if (PivotRight())
            {
                transform.Rotate(new Vector3(0, Speed, 0));
            }

            if (PivotUpwards())
            {
                transform.Rotate(new Vector3(-Speed, 0, 0));
            }

            if (PivotDownwards())
            {
                transform.Rotate(new Vector3(Speed, 0, 0));
            }

            if (currentHighlighted != null && NextNode() && currentHighlighted.NodeCount > currentSelectedNodeIndex + 1)
            {
                currentSelectedNodeIndex++;
                HashUIController.DisplayNodeInfo(currentHighlighted.GetNode(currentSelectedNodeIndex), currentSelectedNodeIndex, currentHighlighted.NodeCount);
            }

            if (currentHighlighted != null && PreviousNode() && currentSelectedNodeIndex > 0)
            {
                currentSelectedNodeIndex--;
                HashUIController.DisplayNodeInfo(currentHighlighted.GetNode(currentSelectedNodeIndex), currentSelectedNodeIndex, currentHighlighted.NodeCount);
            }

            //See if there are any Hashnodes in front of the camera
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            //If the mouse is hovered over a HashNode object, highlight it
            if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<HashNode>() != null)
            {
                //Get a reference to the HashNode that was hit
                HashNode hitNode = hit.transform.GetComponent<HashNode>();

                //If the node being looked at is not the current highlighted node, make it the highlighted node
                if (currentHighlighted != hitNode)
                {
                    //Reset the color of the old highlighted node, if there is one
                    if (currentHighlighted != null)
                    {
                        currentHighlighted.SetColor();
                    }

                    //Make the node being hovered over the current highlighted node and set its color to yellow
                    currentHighlighted = hitNode;
                    Color highlightedColor = Color.yellow;
                    highlightedColor.a = currentHighlighted.Visibility;
                    hitNode.GetComponent<Renderer>().material.color = highlightedColor;

                    //Display information about the current node
                    HashUIController.DisplayBoardInfo(hitNode.BoardState);
                    currentSelectedNodeIndex = 0;
                    HashUIController.DisplayNodeInfo(hitNode.GetNode(currentSelectedNodeIndex), currentSelectedNodeIndex, hitNode.NodeCount);
                }
            }
            else if (currentHighlighted != null)
            {
                //If the camera is not looking at a HashNode object, reset the previously highlighted node and hide the node information panel
                currentHighlighted.SetColor();
                currentHighlighted = null;
                HashUIController.HideBoardInfo();
                currentSelectedNodeIndex = 0;
            }
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the StafeLeft movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the StrafeLeft movement</returns>
        public bool StrafeLeft()
        {
            return Input.GetKey(KeyCode.A) || Input.GetAxis("LeftThumbstickHorizontal") < -0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the StrafeRight movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the StrafeRight movement</returns>
        public bool StrafeRight()
        {
            return Input.GetKey(KeyCode.D) || Input.GetAxis("LeftThumbstickHorizontal") > 0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the Forwards movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the Forwards movement</returns>
        public bool Forwards()
        {
            return Input.GetKey(KeyCode.W) || Input.GetAxis("LeftThumbstickVertical") > 0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the Backwards movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the Backwards movement</returns>
        public bool Backwards()
        {
            return Input.GetKey(KeyCode.S) || Input.GetAxis("LeftThumbstickVertical") < -0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the Upwards movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the Upwards movement</returns>
        public bool Upwards()
        {
            return Input.GetKey(KeyCode.Space) || Input.GetButton("AButton");
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the Downwards movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the Downwards movement</returns>
        public bool Downwards()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetButton("XButton");
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the PivotLeft movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the PivotLeft movement</returns>
        public bool PivotLeft()
        {
            return Input.GetKey(KeyCode.Q) || Input.GetAxis("RightThumbstickHorizontal") < -0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the PivotRight movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the PivotRight movement</returns>
        public bool PivotRight()
        {
            return Input.GetKey(KeyCode.E) || Input.GetAxis("RightThumbstickHorizontal") > 0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the PivotUpwards movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the PivotUpwards movement</returns>
        public bool PivotUpwards()
        {
            return Input.GetKey(KeyCode.Z) || Input.GetAxis("RightThumbstickVertical") < -0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the PivotDownwards movement
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the PivotDownwards movement</returns>
        public bool PivotDownwards()
        {
            return Input.GetKey(KeyCode.X) || Input.GetAxis("RightThumbstickVertical") > 0.1f;
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the NextNode command
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the NextNode command</returns>
        public bool NextNode()
        {
            return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetButtonDown("RightTrigger");
        }

        /// <summary>
        /// True if the user has entered input that corresponds to the PreviousNode command
        /// </summary>
        /// <returns>A flag indicating if the user has entered input that corresponds to the PreviousNode command</returns>
        public bool PreviousNode()
        {
            return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetButtonDown("LeftTrigger");
        }
    }
}