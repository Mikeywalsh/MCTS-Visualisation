using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public NodeObject CurrentNode;

	void Start () {
		
	}

    /// <summary>
    /// Called every frame, rotates the camera around the root node
    /// </summary>
    void Update()
    {        
        transform.LookAt(transform.parent.position);
        transform.parent.position = Vector3.Lerp(transform.parent.position, CurrentNode.transform.position, 0.1f);

        int inputNum = GetNumericalInput();

        if(inputNum != -1)
        {
            if(CurrentNode.transform.childCount >= inputNum)
            {
                CurrentNode = CurrentNode.transform.GetChild(inputNum - 1).GetComponent<NodeObject>();
                CurrentNode.SelectNode();
                UIController.DisplayNodeInfo(CurrentNode.TreeNode);
            }
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if(CurrentNode.transform.parent != null)
            {
                CurrentNode = CurrentNode.transform.parent.GetComponent<NodeObject>();
                CurrentNode.SelectNode();
                UIController.DisplayNodeInfo(CurrentNode.TreeNode);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && transform.localPosition.magnitude > 100)
        {
            transform.position += (transform.localPosition.normalized * 75);
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.position -= (transform.localPosition.normalized * 75);
        }
        else
        {
            transform.RotateAround(transform.parent.position, Vector3.up, 1);
        }
    }

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
