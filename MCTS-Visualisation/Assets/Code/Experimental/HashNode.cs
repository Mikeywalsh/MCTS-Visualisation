using System.Collections.Generic;
using UnityEngine;
using MCTS.Core;
using System.Linq;

public class HashNode : MonoBehaviour
{
    /// <summary>
    /// The size of this <see cref="HashNode"/> <para/>
    /// Calculated as a function of the <see cref="TotalVisits"/> of this <see cref="HashNode"/>
    /// </summary>
    public float NodeSize { get; private set; }

    /// <summary>
    /// The line thickness of any <see cref="LineRenderer"/>'s attached to this <see cref=HashNode"/>
    /// </summary>
    public float LineThickness { get; private set; }

    /// <summary>
    /// The anchorered origin position of this node in world space
    /// </summary>
    private Vector3 originPosition;

    /// <summary>
    /// The next target position of this nodes <see cref="GameObject"/>, which it will slowly move towards
    /// </summary>
    private Vector3 nextTarget;

    /// <summary>
    /// A flag indication whether or not the <see cref="nextTarget"/> position has been reached
    /// </summary>
    private bool reachedTarget;

    /// <summary>
    /// A mapping of child <see cref="LineRenderer"/>'s and gameobjects that they link to
    /// </summary>
    private List<LineConnection> connections = new List<LineConnection>();

    /// <summary>
    /// A list of <see cref="Node"/>'s that this <see cref="HashNode"/> contains
    /// </summary>
    private List<Node> containedNodes = new List<Node>();

    /// <summary>
    /// The board state that this <see cref="HashNode"/> represents
    /// </summary>
    public Board BoardState { get; private set; }

    /// <summary>
    /// The color of this <see cref="HashNode"/>, which will change depending on its properties
    /// </summary>
    private Color nodeColor = Color.white;

    /// <summary>
    /// The minimum size a <see cref="HashNode"/> object can be
    /// </summary>
    private const float MINIMUM_SIZE = 1f;

    /// <summary>
    /// The maximum size a <see cref="HashNode"/> objet can be
    /// </summary>
    private const float MAXIMUM_SIZE = 7f;
    
    /// <summary>
    /// The minimum line thickness that an attached <see cref="LineRenderer"/> can have
    /// </summary>
    private const float MINIMUM_LINE_THICKNESS = 0.1f;

    /// <summary>
    /// The maximum line thickness that an attached <see cref="LineRenderer"/> can have
    /// </summary>
    private const float MAXIMUM_LINE_THICKNESS = 0.5f;

    /// <summary>
    /// The rate at which a <see cref="HashNode"/> object scales with its amount of visits
    /// </summary>
    private const float SCALE_FACTOR = 0.1f;

    /// <summary>
    /// Initialises this <see cref="HashNode"/>, assigning it an origin position and picking its initial target position
    /// </summary>
    /// <param name="origin">The origin position which this <see cref="HashNode"/> will be anchored to</param>
    public void Initialise(Vector3 origin)
    {
        //Set the initial size of this node
        NodeSize = MINIMUM_SIZE;
        transform.localScale = Vector3.one * NodeSize;

        //Save the origin position for this node
        originPosition = origin;

        //Set an initial target
        nextTarget = originPosition + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
    }

    /// <summary>
    /// Adjusts the scale of this hash node depending on the sum of the total number of visits for each contained node
    /// </summary>
    /// 
    public void AdjustSize()
    {
        NodeSize = MINIMUM_SIZE + (MAXIMUM_SIZE - MINIMUM_SIZE) * (1 - Mathf.Exp(-SCALE_FACTOR * TotalVisits)) / (1 + Mathf.Exp(-SCALE_FACTOR * TotalVisits));
        LineThickness = MINIMUM_LINE_THICKNESS + (MAXIMUM_LINE_THICKNESS - MINIMUM_LINE_THICKNESS) * (1 - Mathf.Exp(-SCALE_FACTOR * TotalVisits)) / (1 + Mathf.Exp(-SCALE_FACTOR * TotalVisits));

        //Alter the scale of this node depending on the sum of the the total number of viits of all child nodes of this hashnode
        transform.localScale = Vector3.one * NodeSize;

        for(int i = 0; i < connections.Count; i++)
        {
            //Recursively alter the size of all connected hash nodes in the hierachy
            connections[i].ConnectedNode.AdjustSize();

            //Alter the thickness of the lines connecting this HashNode to the connceted HashNode
            connections[i].Line.startWidth = LineThickness;
            connections[i].Line.endWidth = connections[i].ConnectedNode.LineThickness;
        }
    }

    /// <summary>
    /// Adds a new <see cref="Node"/> to this hash node
    /// </summary>
    /// <param name="lineTarget"></param>
    /// <param name="newNode"></param>
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

        //If this node contains a terminal node, mark it as such
        if (ContainsTerminalNode)
        {
            nodeColor = Color.green;
            SetColor();
        }

        //If this hashnode already has one line renderer, then it contains a duplicate board state in the tree, mark it accordingly
        if (transform.childCount > 0)
        {
            //Mark this hashnode as a duplicate in the tree
            if (!ContainsTerminalNode)
            {
                nodeColor = Color.red;
                SetColor();
            }
        }

        GameObject newChildObject = new GameObject();
        newChildObject.transform.parent = transform;
        newChildObject.transform.position = transform.position;
        newChildObject.name = "Line " + (transform.childCount);

        //Add a line renderer to the new child object and obtain a reference to it
        LineRenderer newLine = newChildObject.AddComponent<LineRenderer>();

        //Initialise the line renderer starting values
        newLine.startWidth = MINIMUM_LINE_THICKNESS;
        newLine.endWidth = MINIMUM_LINE_THICKNESS;
        Color32 lineColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        newLine.startColor = lineColor;
        newLine.endColor = lineColor;
        newLine.material = Resources.Load<Material>("LineMat");

        //Map the new line renderer to the parent gameobject
        connections.Add(new LineConnection(lineTarget.GetComponent<HashNode>(), newLine));

        //Adjust the sze of this node depending on the total amount of visits its contained nodes have
        AdjustSize();
    }

    public void Update()
    {
        //If the target position has been reached, assign a new target positon
        if ((transform.position - nextTarget).magnitude <= 0.1f)
        {
            nextTarget = originPosition + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
        }

        //Move closer to the target position via linear interpolation
        transform.position = Vector3.Lerp(transform.position, nextTarget, 0.01f);

        //Obtain the positions to draw the line between
        for (int i = 0; i < connections.Count; i++)
        {
            Vector3[] lineCoords = new Vector3[2];
            lineCoords[0] = transform.position;
            lineCoords[1] = connections[i].ConnectedNode.transform.position;
            connections[i].Line.SetPositions(lineCoords);
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

    /// <summary>
    /// A flag indicating if the <see cref="BoardState"/> of this <see cref="HashNode"/> is terminal
    /// </summary>
    public bool ContainsTerminalNode
    {
        get
        {
            return BoardState.Winner != -1;
        }
    }

    /// <summary>
    /// Property containing the sum of the total number of visits from each contained node
    /// </summary>
    private int TotalVisits
    {
        get
        {
            int total = 0;

            foreach (Node n in containedNodes)
            {
                total += n.Visits;
            }

            return total;
        }
    }
}
