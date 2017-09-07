using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This class is responsible for the drawing of lines between nodes to the screen <para/>
/// This should be attached to a camera being used to view the game tree
/// </summary>
public class LineDraw : MonoBehaviour
{
    /// <summary>
    /// A static list containing <see cref="ColoredLine"/> structs for each line that needs to be rendered <para/>
    /// Storing the lines this way instead of accessing each needed node via references is a huge speed increase
    /// </summary>
    public static List<ColoredLine> Lines = new List<ColoredLine>();

    /// <summary>
    /// Used to toggle whether or not the lines are visible
    /// </summary>
    public bool linesVisible;

    /// <summary>
    /// The material used for the lines
    /// </summary>
    private Material lineMat;

    /// <summary>
    /// A list of colors that is used for different line depths
    /// </summary>
    public static Color[] lineColors = new Color[] { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.white, Color.cyan };

    /// <summary>
    /// Called as soon as this object is created <para/>
    /// Creates the material used to render lines from the shader located in the Resources folder
    /// </summary>
    private void Awake()
    {
        lineMat = new Material(Resources.Load<Shader>("LineShader"));
    }

    /// <summary>
    /// Draws lines between every connected node in the <see cref="NodeObject.AllNodes"/> list
    /// </summary>
    /// <param name="projection">The projection matrix used to draw the lines. Differs depending on if drawing to scene or game view.</param>
    private void DrawLines(Matrix4x4 projection)
    {
        if (!linesVisible)
            return;

        GL.PushMatrix();
        lineMat.SetPass(0);
        GL.Begin(GL.LINES); 
        GL.LoadProjectionMatrix(projection);

        foreach(ColoredLine l in Lines)
        {
            GL.Color(l.LineColor);
            GL.Vertex(l.From);
            GL.Vertex(l.To);
        }

        GL.End();
        GL.PopMatrix();
    }

    /// <summary>
    /// Selects the provided node, replacing the contents of the <see cref="Lines"/> list with only lines relevant to the select nodes related nodes
    /// </summary>
    /// <param name="node">The node to select</param>
    public static void SelectNode(NodeObject node)
    {
        //Clear the list
        Lines = new List<ColoredLine>();
        NodeObject currentNode = node;

        //Back-track up the tree, drawing lines from the root node to the selected node
        while (currentNode.Parent != null)
        {
            Lines.Add(new ColoredLine(((NodeObject)currentNode.Parent).Position, currentNode.Position, LineDraw.lineColors[currentNode.Parent.Depth % LineDraw.lineColors.Length]));
            currentNode = (NodeObject)currentNode.Parent;
        }

        //Recursively add all the children of the selected node to the Lines list
        AddChildrenToList(node);
    }

    /// <summary>
    /// Recursively adds <see cref="ColoredLine"/> objects to the <see cref="Lines"/> list for all of a given nodes children
    /// </summary>
    /// <param name="node">A node which will recursively have its children added to the Lines list</param>
    public static void AddChildrenToList(NodeObject node)
    {
        for (int i = 0; i < node.Children.Count; i++)
        {
            Lines.Add(new ColoredLine(node.Position, ((NodeObject)node.Children[i]).Position, LineDraw.lineColors[node.Depth % LineDraw.lineColors.Length]));
            AddChildrenToList((NodeObject)node.Children[i]);
        }
    }

    /// <summary>
    /// For use with drawing lines between nodes in game view
    /// </summary>
    private void OnPostRender()
    {
        if (lineMat != null)
            DrawLines(GetComponent<Camera>().projectionMatrix);
        else
            Debug.LogError("Line shader not set");
    }

    #if UNITY_EDITOR
    /// <summary>
    /// For use with drawing lines between nodes in scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (lineMat != null)
            DrawLines(SceneView.lastActiveSceneView.camera.projectionMatrix);
    }
    #endif
}
