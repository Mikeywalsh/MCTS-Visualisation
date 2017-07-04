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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(CurrentNode.transform.childCount > 0)
            {
                CurrentNode = CurrentNode.transform.GetChild(0).GetComponent<NodeObject>();
                //transform.parent.position = CurrentNode.transform.position;
                CurrentNode.SelectNode();
            }
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if(CurrentNode.transform.parent != null)
            {
                CurrentNode = CurrentNode.transform.parent.GetComponent<NodeObject>();
                //transform.parent.position = CurrentNode.transform.position;
                CurrentNode.SelectNode();
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
}
