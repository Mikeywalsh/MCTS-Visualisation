﻿using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This class is responsible for the drawing of lines between nodes to the screen
/// This should be attached to a camera being used to view the game tree
/// </summary>
public class LineDraw : MonoBehaviour
{
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
    private static Color[] lineColors = new Color[] { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.white, Color.cyan };

    /// <summary>
    /// Called as soon as this object is created
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
        
        foreach(NodeObject n in NodeObject.AllNodes)
        {
            if (n.gameObject.activeInHierarchy)
            {
                GL.Color(lineColors[n.ParentNode.Depth % lineColors.Length]);
                GL.Vertex(n.ParentNode.transform.position);
                GL.Vertex(n.transform.position);
            }
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
