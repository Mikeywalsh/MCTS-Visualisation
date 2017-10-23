using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MCTS.Core;
using MCTS.Core.Games;
using System.Linq;

public class HashNode : MonoBehaviour
{
    private Vector3 startingPosition;

    private Vector3 nextTarget;

    bool reachedTarget;

    private Dictionary<LineRenderer, GameObject> lines = new Dictionary<LineRenderer, GameObject>();

    List<Node> containedNodes = new List<Node>();

    public void Start()
    {
        //Save the starting position for this node
        startingPosition = transform.position;

        //Set an initial target
        nextTarget = startingPosition + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }

    public void AddNode(GameObject lineTarget, Node newNode)
    {
        containedNodes.Add(newNode);

        //If this hashnode already has one line renderer, then it contains a duplicate board state in the tree, mark it accordingly
        if (transform.childCount > 0)
        {
            //Mark this hashnode as a duplicate in the tree
            GetComponent<Renderer>().material.color = Color.red;
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
            nextTarget = startingPosition + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        }

        //Move closer to the target position via linear interpolation
        transform.position = Vector3.Lerp(transform.position, nextTarget, 0.01f);

        //Obtain the positions to draw the line between
        for (int i = 0; i < lines.Count; i++)
        {
            Vector3[] lineCoords = new Vector3[2];
            LineRenderer lineObject = lines.Keys.ToArray()[i];
            lineCoords[0] = lineObject.transform.position;
            lineCoords[1] = lines[lineObject].transform.position;
            lines.Keys.ToArray()[i].SetPositions(lineCoords);
        }
    }
}
