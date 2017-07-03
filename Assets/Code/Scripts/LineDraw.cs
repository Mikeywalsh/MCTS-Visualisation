using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class is responsible for the drawing of lines between nodes to the screen
/// This should be attached to a camera being used to view the 
/// </summary>
public class LineDraw : MonoBehaviour
{
    /// <summary>
    /// The material used for the lines
    /// </summary>
    private Material lineMat;

    /// <summary>
    /// A list of colors that is used for different line depths
    /// </summary>
    private static Color[] lineColors = new Color[] { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.white, Color.cyan };

    /// <summary>
    /// Called as soon as this object is created
    /// Lo
    /// </summary>
    private void Awake()
    {
        lineMat = new Material(Resources.Load<Shader>("LineShader"));
    }

    /// <summary>
    /// Called every frame, rotates the camera around the root node
    /// </summary>
    void Update()
    {
        transform.RotateAround(Vector3.zero, new Vector3(0, 1, 0), 1);
    }

    /// <summary>
    /// Draws lines between every connected node in the <see cref="NodeObject.AllNodes"/> list
    /// </summary>
    /// <param name="projection">The projection matrix used to draw the lines. Differs depending on if drawing to scene or game view.</param>
    private void DrawLines(Matrix4x4 projection)
    {
        GL.PushMatrix();
        lineMat.SetPass(0);
        GL.Begin(GL.LINES); 
        GL.LoadProjectionMatrix(projection);
        
        foreach(NodeObject n in NodeObject.AllNodes)
        {
            GL.Color(lineColors[n.Parent.Depth % lineColors.Length]);
            GL.Vertex(n.Parent.transform.position);
            GL.Vertex(n.transform.position);
        }

        GL.End();
        GL.PopMatrix();
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

    /// <summary>
    /// For use with drawing lines between nodes in scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (lineMat != null)
            DrawLines(SceneView.lastActiveSceneView.camera.projectionMatrix);
    }
}
