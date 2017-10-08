using UnityEngine;
using MCTS.Core;

/// <summary>
/// An extension upon <see cref="Node"/>, used for representing nodes of a Monte Carlo Tree Search graphically
/// </summary>
public class NodeObject : Node {

    /// <summary>
    /// The position of this Node in the world
    /// </summary>
    public Vector3 Position { get; private set; }

    /// <summary>
    /// The amount of this nodes children that have had their positions set so far
    /// </summary>
    public int ChildPositionsSet { get; private set; }

    /// <summary>
    /// The angle allowance this node has, used when visualising as a 2D disk
    /// </summary>
    public float ArcAngle { get; private set; }

    /// <summary>
    /// Gives this <see cref=" NodeObject"/> a position in world space depending on its position in the game tree
    /// </summary>
	public void SetPosition(VisualisationType visualisationType)
    {
        NodeObject parentObject = (NodeObject)Parent;

        //Play around with this value to change the structure of the tree depending on depth
        float depthMul = 60;

        if (visualisationType == VisualisationType.Standard3D)
        {
            #region Standard 3D node placement
            //Root node, automatically starts at origin, does not require additional initialisation
            if (Depth == 0)
            {
                return;
            }

            if (Depth == 1)             //If at depth 1, use the Fibonacci sphere algorithm to evenly distribute all depth 1 nodes in a sphere around the root node
            {
                #region Fibbonacci Sphere algorithm
                int samples = Parent.Children.Capacity + 1;
                float offset = 2f / samples;
                float increment = Mathf.PI * (3 - Mathf.Sqrt(5));

                float y = ((parentObject.ChildPositionsSet * offset) - 1) + (offset / 2);
                float r = Mathf.Sqrt(1 - Mathf.Pow(y, 2));
                float phi = ((parentObject.ChildPositionsSet + 1) % samples) * increment;
                float x = Mathf.Cos(phi) * r;
                float z = Mathf.Sin(phi) * r;
                #endregion
                Position = new Vector3(x, y, z) * depthMul;
            }
            else                        //If at any other depth, position the new node a set distance away from its parent node
            {
                Position = parentObject.Position;
                Position += parentObject.LocalPosition.normalized * depthMul * 2;
                Vector3 rotationPoint = Position;

                Vector3 normal = parentObject.LocalPosition;
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

                if (Parent.Children.Count != 1)
                {
                    Position += tangent.normalized * depthMul * 0.25f;
                    Position = Position.RotateAround(rotationPoint, rotationPoint - parentObject.Position, (360 / Parent.Children.Capacity) * parentObject.ChildPositionsSet);
                }
            }
            #endregion
        }
        else if(visualisationType == VisualisationType.Disk2D)
        {
            #region Disk 2D node placement
            //Root node, automatically starts at origin, does not require additional initialisation
            if (Depth == 0)
            {
                ArcAngle = 360;
                return;
            }

            if (Depth == 1)             //If at depth 1, place the new node on the edge of a circle around the origin
            {
                Position = new Vector3(60, 0, 0);
                Position = Position.RotateAround(Vector3.zero, Vector3.up, parentObject.ChildPositionsSet * (parentObject.ArcAngle / parentObject.Children.Count));
            }
            else                        //If at any other depth, position the new node a set distance away from its parent node
            {
                Position = parentObject.Position;
                Position += (parentObject.LocalPosition.normalized * depthMul);
                Position = Position.RotateAround(parentObject.Position, Vector3.up, -(parentObject.ArcAngle / 2) + (parentObject.ChildPositionsSet * (parentObject.ArcAngle / parentObject.Children.Count)));
            }
            ArcAngle = parentObject.ArcAngle / parentObject.Children.Count;
            #endregion
        }
        else
        {
            throw new System.Exception("Unknown visualisation type: " + visualisationType.ToString() + " encountered");
        }

        parentObject.ChildPositionsSet++;
    }    

    /// <summary>
    /// The local position of this node, relative to its parent
    /// </summary>
    public Vector3 LocalPosition
    {
        get
        {
            if (Parent == null)
            {
                return Position;
            }
            else
            {
                return Position - ((NodeObject)Parent).Position;
            }
        }
    }
}
