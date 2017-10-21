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
		
        if(Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * 0.1f * Speed);
        }

        if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * -0.1f * Speed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -1 * Speed, 0));
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, 1 * Speed, 0));
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.up * 0.1f * Speed);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.up * -0.1f * Speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.right * -0.1f * Speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * 0.1f * Speed);
        }
    }
}
