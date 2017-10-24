using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MCTS.Core;
using MCTS.Core.Games;
using System.Linq;

public class HashNode : MonoBehaviour
{
    private Vector3 originPosition;

    private Vector3 nextTarget;

    private bool reachedTarget;

    private Dictionary<LineRenderer, GameObject> lines = new Dictionary<LineRenderer, GameObject>();

    private List<Node> containedNodes = new List<Node>();

    public Board BoardState { get; private set; }

    private Color nodeColor = Color.white;

    public void Initialise(Vector3 origin)
    {
        //Save the origin position for this node
        originPosition = origin;

        //Set an initial target
        nextTarget = originPosition + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }

    public void AddNode(GameObject lineTarget, Node newNode)
    {
        containedNodes.Add(newNode);

        if(BoardState == null)
        {
            BoardState = newNode.GameBoard;
        }

        //If there is no line target, then return immeditately and do not create a LineRenderer
        if(lineTarget == null)
        {
            return;
        }

        //If this hashnode already has one line renderer, then it contains a duplicate board state in the tree, mark it accordingly
        if (transform.childCount > 0)
        {
            //Mark this hashnode as a duplicate in the tree
            nodeColor = Color.red;
            SetColor();
        }

        GameObject newChildObject = new GameObject();
        newChildObject.transform.parent = transform;
        newChildObject.transform.position = transform.position;
        newChildObject.name = "Line " + (transform.childCount);

        //Add a line renderer to the new child object and obtain a reference to it
        LineRenderer newLine = newChildObject.AddComponent<LineRenderer>();

        //Initialise the line renderer starting values
        newLine.startWidth = 0.1f;
        newLine.endWidth = 0.1f;
        Color32 lineColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        newLine.startColor = lineColor;
        newLine.endColor = lineColor;
        newLine.material = Resources.Load<Material>("LineMat");

        //Map the new line renderer to the parent gameobject
        lines.Add(newLine, lineTarget);
    }

    public void Update()
    {
        //If the target position has been reached, assign a new target positon
        if ((transform.position - nextTarget).magnitude <= 0.1f)
        {
            nextTarget = originPosition + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        }

        //Move closer to the target position via linear interpolation
        transform.position = Vector3.Lerp(transform.position, nextTarget, 0.01f);

        //Obtain the positions to draw the line between
        for (int i = 0; i < lines.Count; i++)
        {
            Vector3[] lineCoords = new Vector3[2];
            lineCoords[0] = transform.position;
            lineCoords[1] = lines[lines.Keys.ToArray()[i]].transform.position;
            lines.Keys.ToArray()[i].SetPositions(lineCoords);
        }
    }

    public void SetColor()
    {
        GetComponent<Renderer>().material.color = nodeColor;
    }

    /// <summary>
    /// Gets the amount of nodes that share the board state of this HashNode
    /// </summary>
    public int NodeCount
    {
        get { return containedNodes.Count; }
    }

    /// <summary>
    /// Gets the node at the provided index from the <see cref="containedNodes"/> list
    /// </summary>
    /// <param name="index">The index to get the node from</param>
    /// <returns>The node at the provided index from the <see cref="containedNodes"/> list</returns>
    public Node GetNode(int index)
    {
        return containedNodes[index];
    }
}
