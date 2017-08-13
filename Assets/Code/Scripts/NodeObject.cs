using UnityEngine;

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
    /// Gives this <see cref=" NodeObject"/> a position in world space depending on its position in the game tree
    /// </summary>
	public void SetPosition()
    {
        //Play around with this value to change the structure of the tree depending on depth
        float depthMul = 60;

        //Root node, automatically starts at origin, does not require additional initialisation
        if (Depth == 0)
        {
            return;
        }

        NodeObject parentNode = (NodeObject)Parent;

        if (Depth == 1)             //If at depth 1, use the Fibonacci sphere algorithm to evenly distribute all depth 1 nodes in a sphere around the root node
        {
            #region Fibbonacci Sphere algorithm
            int samples = Parent.Children.Capacity + 1;
            float offset = 2f / samples;
            float increment = Mathf.PI * (3 - Mathf.Sqrt(5));

            float y = ((parentNode.ChildPositionsSet * offset) - 1) + (offset / 2);
            float r = Mathf.Sqrt(1 - Mathf.Pow(y, 2));
            float phi = ((parentNode.ChildPositionsSet + 1) % samples) * increment;
            float x = Mathf.Cos(phi) * r;
            float z = Mathf.Sin(phi) * r;
            #endregion
            Position = new Vector3(x, y, z) * depthMul;
        }
        else                        //If at any other depth, position the new node a set distance away from its parent node
        {
            Position = parentNode.Position;
            Position += parentNode.LocalPosition.normalized * depthMul * 2;
            Vector3 rotationPoint = Position;

            Vector3 normal = parentNode.LocalPosition;
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
                Position = Position.RotateAround(rotationPoint, rotationPoint - parentNode.Position, (360 / Parent.Children.Capacity) * parentNode.ChildPositionsSet);
            }
        }
        parentNode.ChildPositionsSet++;
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
