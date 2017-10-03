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

    public float ArcAngle { get; private set; }

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
            ArcAngle = 360;
            return;
        }

        NodeObject parentObject = (NodeObject)Parent;

        if (Depth == 1)             //If at depth 1, use the Fibonacci sphere algorithm to evenly distribute all depth 1 nodes in a sphere around the root node
        {
            Position = new Vector3(60, 0, 0);

            Position = Position.RotateAround(Vector3.zero, Vector3.up, parentObject.ChildPositionsSet * (parentObject.ArcAngle / parentObject.Children.Count));
        }
        else                        //If at any other depth, position the new node a set distance away from its parent node
        {
            Position = parentObject.Position;
            Position += (parentObject.LocalPosition.normalized * depthMul);
            Position = Position.RotateAround(parentObject.Position, Vector3.up, -(parentObject.ArcAngle / 2) + (parentObject.ChildPositionsSet * (parentObject.ArcAngle / parentObject.Children.Count)));
            //Position = parentNode.Position;
            //Position += parentNode.LocalPosition.normalized * depthMul * 2;
            //Vector3 rotationPoint = Position;

            //Vector3 normal = parentNode.LocalPosition;
            //Vector3 tangent;
            //Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
            //Vector3 t2 = Vector3.Cross(normal, Vector3.up);

            //if (t1.magnitude > t2.magnitude)
            //{
            //    tangent = t1;
            //}
            //else
            //{
            //    tangent = t2;
            //}

            //if (Parent.Children.Count != 1)
            //{
            //    Position += tangent.normalized * depthMul * 0.25f;
            //    Position = Position.RotateAround(rotationPoint, rotationPoint - parentNode.Position, (360 / Parent.Children.Capacity) * parentNode.ChildPositionsSet);
            //}
        }
        //Position *= Depth * 0.45f;
        //Position += new Vector3(0, parentObject.Position.y - 5f, 0);
        ArcAngle = parentObject.ArcAngle / parentObject.Children.Count;
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
