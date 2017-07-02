using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : MonoBehaviour {

    public static int MaxDepth;
    public static List<NodeObject> AllNodes = new List<NodeObject>();

    private NodeObject parent;

    private Node treeNode;

	public void Initialise (Node nodeInTree) {
        treeNode = nodeInTree;
        float depthMul = Mathf.Clamp(12 - Depth, 1, 10);

        //Root node, automatically starts at origin, does not require additional initialisation
        if (Depth == 0)
        {
            return;
        }

        //Set the parent node
        parent = transform.parent.GetComponent<NodeObject>();

        if (Depth == 1)             //If at depth 1, use the Fibonacci sphere algorithm to evenly distribute all depth 1 nodes in a sphere around the root node
        {
            #region Fibbonacci Sphere algorithm
            int samples = parent.TreeNode.Children.Capacity + 1;
            float offset = 2f / samples;
            float increment = Mathf.PI * (3 - Mathf.Sqrt(5));

            float y = ((transform.parent.childCount * offset) - 1) + (offset / 2);
            float r = Mathf.Sqrt(1 - Mathf.Pow(y, 2));
            float phi = ((transform.parent.childCount + 1) % samples) * increment;
            float x = Mathf.Cos(phi) * r;
            float z = Mathf.Sin(phi) * r;
            #endregion
            transform.position = new Vector3(x, y, z) * depthMul;
        }
        else                        //If at any other depth, position the new node a set distance away from its parent node
        {
            transform.position = transform.parent.position;
            transform.position += transform.parent.localPosition.normalized * depthMul;
            Vector3 rotationPoint = transform.position;

            Vector3 normal = transform.parent.localPosition;
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

            if (parent.TreeNode.Children.Count != 1)
            {
                transform.position += tangent.normalized * depthMul;
                transform.RotateAround(rotationPoint, rotationPoint - transform.parent.position, (360 / parent.TreeNode.Children.Capacity) * parent.transform.childCount);
            }
        }

        //Reset the rotation of this node object so that the local position for any children is correct
        transform.rotation = Quaternion.identity;

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
