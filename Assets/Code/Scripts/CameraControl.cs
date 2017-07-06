﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    /// <summary>
    /// The currently selected node
    /// Information about the current node and its children is displayed in the UI
    /// </summary>
    public NodeObject CurrentNode;

    /// <summary>
    /// Called every frame, rotates the camera around the root node
    /// </summary>
    void Update()
    {
        //Make the camera always look directly at the focus point
        transform.LookAt(transform.parent.position);

        //Smoothly translate the focus point towards the current node
        transform.parent.position = Vector3.Lerp(transform.parent.position, CurrentNode.transform.position, 0.1f);

        //Allow the user to navigate the tree with number keys
        int inputNum = GetNumericalInput();
        if (inputNum != -1)
        {
            SelectChildNode(inputNum - 1);
        }

        //Allow the user to travel back to the parent node with the backspace key
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SelectParentNode();
        }

        //Allow the user to zoom in and out of the game tree with the scroll wheel
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && transform.localPosition.magnitude > 250)
        {
            transform.position += (transform.localPosition.normalized * 75);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.position -= (transform.localPosition.normalized * 75);
        }
        else
        {
            transform.RotateAround(transform.parent.position, Vector3.up, 1);
        }
    }

    /// <summary>
    /// Changes the currently selected node to be a child of the currently selected node
    /// </summary>
    /// <param name="childIndex">The child index of the new selected node</param>
    public void SelectChildNode(int childIndex)
    {
        if (CurrentNode.transform.childCount > childIndex)
        {
            CurrentNode = CurrentNode.transform.GetChild(childIndex).GetComponent<NodeObject>();
            CurrentNode.SelectNode();
            UIController.DisplayNodeInfo(CurrentNode.TreeNode);
        }
    }

    /// <summary>
    /// Changes the current node to be the parent of the current node, if it has a parent
    /// </summary>
    public void SelectParentNode()
    {
        if (CurrentNode.transform.parent != null)
        {
            CurrentNode = CurrentNode.transform.parent.GetComponent<NodeObject>();
            CurrentNode.SelectNode();
            UIController.DisplayNodeInfo(CurrentNode.TreeNode);
        }
    }

    /// <summary>
    /// Returns an integer representing the value of the numerical key, if one has been pressed this frame
    /// Allows the user to navigate the tree with keys instead of the on-screen buttons
    /// </summary>
    /// <returns>An integer representing the value of the numerical key that was pressed. -1 if no key was pressed</returns>
    private int GetNumericalInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            return 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            return 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            return 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            return 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            return 5;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            return 6;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            return 7;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            return 8;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            return 9;


        return -1;
    }
}
