using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A <see cref="MonoBehaviour"/> class used for representing nodes of a Monte Carlo Tree Search graphically
/// A NodeObject is created for each node the tree
/// </summary>
public class NodeObject : MonoBehaviour {

    /// <summary>
    /// A list of all nodes within the scene
    /// Active nodes in the list will be used to draw lines between them and their parent
    /// </summary>
    public static List<NodeObject> AllNodes = new List<NodeObject>();

    /// <summary>
    /// A list of currently inactive nodes in the scene
    /// They are stored to keep references to them, so that they can be re-enabled
    /// </summary>
    public static List<GameObject> InactiveNodes = new List<GameObject>();

    /// <summary>
    /// This NodeObjects parent NodeObject
    /// </summary>
    private NodeObject parentNode;

    /// <summary>
    /// This objects node in the game tree
    /// </summary>
    private Node treeNode;

    //TEMP
    public bool toggleSelected;

    public void Update()
    {
        if (toggleSelected)
        {
            SelectNode();
            toggleSelected = false;
        }
    }

    /// <summary>
    /// Selects this node, making only this node and its children visable
    /// Should only be called if the previously selected node is 1 generation from this node
    /// That is, if the previous node is this nodes parent or child
    /// </summary>
    public void SelectNode()
    {
        //Create a new list of inactive nodes to replace the previous list
        List<GameObject> newInactiveNodes = new List<GameObject>();

        //Cycle through each inactive node, setting them to active if they are this nodes child
        for (int i = 0; i < InactiveNodes.Count; i++)
        {
            if (InactiveNodes[i].transform.parent == transform)
            {
                InactiveNodes[i].SetActive(true);
            }
            else
            {
                //Store the still inactive nodes in the new inactive list
                newInactiveNodes.Add(InactiveNodes[i]);
            }
        }

        //Replace the list of inactive nodes with the new list
        InactiveNodes = newInactiveNodes;

        //Disable all of this nodes siblings, if it has any
        if (parentNode != null)
        {
            for (int i = 0; i < parentNode.transform.childCount; i++)
            {
                Transform child = parentNode.transform.GetChild(i);
                if (child != transform)
                {
                    InactiveNodes.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Initialises this <see cref=" NodeObject"/>, giving it a position in world space depending on its position in the game tree
    /// </summary>
    /// <param name="nodeInTree">The node in the MCTS that this NodeObject represents</param>
	public void Initialise (Node nodeInTree) {
        //Assign this NodeObject's treeNode
        treeNode = nodeInTree;

        //Play around with this value to change the structure of the tree depending on depth
        float depthMul = 60;//  Mathf.Clamp(60 - (Depth * 5), 10, 60);

        //Root node, automatically starts at origin, does not require additional initialisation
        if (Depth == 0)
        {
            return;
        }

        //Set the parent node
        parentNode = transform.parent.GetComponent<NodeObject>();

        if (Depth == 1)             //If at depth 1, use the Fibonacci sphere algorithm to evenly distribute all depth 1 nodes in a sphere around the root node
        {
            #region Fibbonacci Sphere algorithm
            int samples = parentNode.TreeNode.Children.Capacity + 1;
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
            transform.position += transform.parent.localPosition.normalized * depthMul * 2;
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

            if (parentNode.TreeNode.Children.Count != 1)
            {
                transform.position += tangent.normalized * depthMul * 0.25f;
                transform.RotateAround(rotationPoint, rotationPoint - transform.parent.position, (360 / parentNode.TreeNode.Children.Capacity) * parentNode.transform.childCount);
            }
        }

        //Reset the rotation of this node object so that the local position for any children is correct
        transform.rotation = Quaternion.identity;

        AllNodes.Add(this);
	}

    /// <summary>
    /// The parent nodeobject of this nodeobject
    /// </summary>
    public NodeObject ParentNode
    {
        get { return parentNode; }
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
