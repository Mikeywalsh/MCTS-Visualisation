using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    public Shader lineShader;
    private Material lineMat;
    private static Color[] lineColors = new Color[] { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.white, Color.cyan };

    private void Awake()
    {
        lineMat = new Material(Resources.Load<Shader>("LineShader"));
    }

    void Update()
    {
        transform.RotateAround(Vector3.zero, new Vector3(0, 1, 0), 1);
    }

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

    private void OnPostRender()
    {
        if (lineMat != null)
            DrawLines(GetComponent<Camera>().projectionMatrix);
        else
            Debug.LogError("Line shader not set");
    }

    private void OnDrawGizmos()
    {
        if (lineMat != null)
            DrawLines(SceneView.lastActiveSceneView.camera.projectionMatrix);
    }
}
