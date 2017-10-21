using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MCTS.Core;
using MCTS.Core.Games;
using System.Linq;

public class HashNode : MonoBehaviour
{
    private GameObject ParentNode;

    private Vector3 startingPosition;

    private Vector3 nextTarget;

    bool reachedTarget;

    private Dictionary<LineRenderer, GameObject> lines = new Dictionary<LineRenderer, GameObject>();

    public bool nested;

    public void Start()
    {
        //Save the starting position for this node
        startingPosition = transform.position;

        //Set an initial target
        nextTarget = startingPosition + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }

    public void Initialise(GameObject parent)
    {
        if(transform.parent != null)
        {
            nested = true;

            //Mark the parent object as a duplicate in the tree
            transform.parent.GetComponent<Renderer>().material.color = Color.red;
        }

        //Set the parent node reference
        ParentNode = parent;

        if (GetComponent<LineRenderer>() == null)
        {
            //Obtain a reference to this objects line renderer
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

            //Initialise the line renderer starting values
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            Color32 lineColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.material = Resources.Load<Material>("LineMat");

            //Map the new line renderer to the parent gameobject
            lines.Add(lineRenderer, parent);
        }
        else
        {
            GameObject newChildObject = new GameObject();
            newChildObject.transform.parent = transform;
            newChildObject.transform.position = transform.position;
            newChildObject.AddComponent<HashNode>().Initialise(parent);
        }
    }

    public void Update()
    {
        if (!nested)
        {
            //If the target position has been reached, assign a new target positon
            if ((transform.position - nextTarget).magnitude <= 0.1f)
            {
                nextTarget = startingPosition + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
            }

            //Move closer to the target position via linear interpolation
            transform.position = Vector3.Lerp(transform.position, nextTarget, 0.01f);
        }

        //Obtain the positions to draw the line between
        for(int i = 0; i < lines.Count; i++)
        {
            Vector3[] lineCoords = new Vector3[2];
            lineCoords[0] = transform.position;
            lineCoords[1] = lines[lines.Keys.ToArray()[i]].transform.position;
            lines.Keys.ToArray()[i].SetPositions(lineCoords);
        }        
    }
}
