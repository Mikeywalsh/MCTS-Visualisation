using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    public Shader lineShader;
    private Material lineMat;
    private GameObject ball;

    private List<GameObject> ballList = new List<GameObject>();

    private void Awake()
    {
        lineMat = new Material(Resources.Load<Shader>("LineShader"));
        ball = Resources.Load<GameObject>("Ball");

        #region Fibbonacci Sphere algorithm
        List<Vector3> points = new List<Vector3>();
        int samples = 100;
        float offset = 2f / samples;
        float increment = Mathf.PI * (3 - Mathf.Sqrt(5));

        for(int i = 0; i < samples; i++)
        {
            float y = ((i * offset) - 1) + (offset / 2);
            float r = Mathf.Sqrt(1 - Mathf.Pow(y, 2));

            float phi = ((i + 1) % samples) * increment;

            float x = Mathf.Cos(phi) * r;
            float z = Mathf.Sin(phi) * r;

            points.Add(new Vector3(x, y, z));
        }
        #endregion

        for (int i = 0; i < samples; i++)
        {
            GameObject newBall = Instantiate(ball, points[i] * 2, Quaternion.identity);
            ballList.Add(newBall);
        }
    }

    // Update is called once per frame
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
        GL.Color(Color.green);
        
        for(int x = 0; x < ballList.Count; x++)
        {
            GL.Vertex3(0, 0, 0);
            GL.Vertex(ballList[x].transform.position);
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
