using System.Collections.Generic;
using UnityEngine;

public class DrawPointBehaviour : MonoBehaviour
{
    /// <summary>
    /// 顶点
    /// </summary>
    protected List<Vector3> vertices = new List<Vector3>();

    /// <summary>
    /// opengl 绘制
    /// </summary>
    public bool IsOpenGLDraw = true;

    void OnRenderObject()
    {
        if (!IsOpenGLDraw)
        {
            return;
        }

        if (vertices == null || vertices.Count < 2)
        {
            return;
        }

        GL.PushMatrix();
        GL.MultMatrix(this.transform.localToWorldMatrix);
        GL.Color(new Color(0, 0, 0));

        GL.Begin(GL.LINES);
        int index = vertices.Count / 2;
        for (int i = 0; i < index; i++)
        {
            GL.Vertex(vertices[i * 2]);
            GL.Vertex(vertices[i * 2 + 1]);
        }
        GL.End();

        GL.PopMatrix();
    }
}
