using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashCameraControl : MonoBehaviour {

    public float Speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Forwards())
        {
            transform.Translate(Vector3.forward * 0.1f * Speed);
        }

        if(Backwards())
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
            transform.Rotate(new Vector3(0, -1 * Speed, 0));
        }

        if (PivotRight())
        {
            transform.Rotate(new Vector3(0, 1 * Speed, 0));
        }

        if (PivotUpwards())
        {
            transform.Rotate(new Vector3(-1 * Speed, 0, 0));
        }

        if (PivotDownwards())
        {
            transform.Rotate(new Vector3(1 * Speed, 0, 0));
        }
    }

    public bool StrafeLeft()
    {
        return Input.GetKey(KeyCode.A) || Input.GetAxis("LeftThumbstickHorizontal") < -0.1f;
    }

    public bool StrafeRight()
    {
        return Input.GetKey(KeyCode.D) || Input.GetAxis("LeftThumbstickHorizontal") > 0.1f;
    }

    public bool Forwards()
    {
        return Input.GetKey(KeyCode.W) || Input.GetAxis("LeftThumbstickVertical") > 0.1f;
    }

    public bool Backwards()
    {
        return Input.GetKey(KeyCode.S) || Input.GetAxis("LeftThumbstickVertical") < -0.1f;
    }

    public bool Upwards()
    {
        return Input.GetKey(KeyCode.Space) || Input.GetButton("AButton");
    }

    public bool Downwards()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetButton("XButton");
    }

    public bool PivotLeft()
    {
        return Input.GetKey(KeyCode.Q) || Input.GetAxis("RightThumbstickHorizontal") < -0.1f;
    }

    public bool PivotRight()
    {
        return Input.GetKey(KeyCode.E) || Input.GetAxis("RightThumbstickHorizontal") > 0.1f;
    }

    public bool PivotUpwards()
    {
        return Input.GetKey(KeyCode.Z) || Input.GetAxis("RightThumbstickVertical") < -0.1f;
    }

    public bool PivotDownwards()
    {
        return Input.GetKey(KeyCode.X) || Input.GetAxis("RightThumbstickVertical") > 0.1f;
    }
}
