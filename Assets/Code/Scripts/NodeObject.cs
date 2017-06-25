using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : MonoBehaviour {

    public static List<NodeObject> AllNodes = new List<NodeObject>();

    private NodeObject parent;

    private Node treeNode;

	public void Initialise (Node nodeInTree) {
        treeNode = nodeInTree;

        if (Depth == 0)
        {
            return;
        }

        parent = transform.parent.GetComponent<NodeObject>();

        if (Depth == 1)
        {
            #region Fibbonacci Sphere algorithm
            List<Vector3> points = new List<Vector3>();
            int samples = parent.TreeNode.Children.Capacity + 1;
            float offset = 2f / samples;
            float increment = Mathf.PI * (3 - Mathf.Sqrt(5));

            for (int i = 0; i < samples; i++)
            {
                float y = ((i * offset) - 1) + (offset / 2);
                float r = Mathf.Sqrt(1 - Mathf.Pow(y, 2));

                float phi = ((i + 1) % samples) * increment;

                float x = Mathf.Cos(phi) * r;
                float z = Mathf.Sin(phi) * r;

                points.Add(new Vector3(x, y, z));
            }
            #endregion
            transform.position = points[transform.parent.childCount] * 2;
        }
        else
        {
            transform.position = transform.parent.position + (4 * (transform.parent.position / Depth));
            //transform.localPosition += 

            Vector3 normal = transform.position;
            Vector3 tangent;
            Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
            Vector3 t2 = Vector3.Cross(normal, Vector3.up);

            if (t1.magnitude > t2.magnitude)
            {
                tangent = t1;
            }
            else
            {
                tangent = t2;
            }

            //if (Depth < 3)
            //{
            if (parent.TreeNode.Children.Count != 1)
            {
                Debug.Log("aaah");
                transform.position += tangent.normalized * (3);
                transform.RotateAround(transform.parent.position + (4 * (transform.parent.position / Depth)), transform.parent.position, (360 / parent.TreeNode.Children.Capacity) * parent.transform.childCount);
            }
            //}
            //else
            //{
            //    Debug.Log("OH SHIT " + Depth);
            //}
        }

        AllNodes.Add(this);
	}
	
	void Update () {
		
	}

    /// <summary>
    /// The parent nodeobject of this nodeobject
    /// </summary>
    public NodeObject Parent
    {
        get { return parent; }
    }

    /// <summary>
    /// This objects node in the game tree
    /// </summary>
    public Node TreeNode
    {
        get { return treeNode; }
    }

    /// <summary>
    /// The depth of this objects node in the game tree
    /// </summary>
    public int Depth
    {
        get { return treeNode.Depth; }
    }
}
