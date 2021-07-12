using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 画直线
/// </summary>
public class DrawLineBehaviour : MonoBehaviour
{
    /// <summary>
    /// 顶点
    /// </summary>
    protected List<Vector3> linePoints = new List<Vector3>();
    /// <summary>
    /// 调试时绘制
    /// </summary>
    public bool IsDebugDraw = false;
    /// <summary>
    /// opengl 绘制
    /// </summary>
    public bool IsOpenGLDraw = true;

    protected void Update()
    {
        OnDebugDraw();
    }

    void OnRenderObject()
    {
        if (!IsOpenGLDraw)
        {
            return;
        }

        if (linePoints == null || linePoints.Count < 2)
        {
            return;
        }

        GL.PushMatrix();
        GL.MultMatrix(this.transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        int index = linePoints.Count / 2;
        for (int i = 0; i < index; i++)
        {
            GL.Vertex(linePoints[i * 2]);
            GL.Vertex(linePoints[i * 2 + 1]);
        }
        GL.End();

        GL.PopMatrix();
    }

    /// <summary>
    /// 调试时绘制
    /// </summary>
    protected void OnDebugDraw()
    {
        if (!IsDebugDraw)
        {
            return;
        }

        if (linePoints == null || linePoints.Count < 2)
        {
            return;
        }

        var offset = this.transform.position;

        int index = linePoints.Count / 2;
        for (int i = 0; i < index; i++)
        {
            Debug.DrawLine(linePoints[i * 2] + offset, linePoints[i * 2 + 1] + offset);
        }
    }

    /// <summary>
    /// 设置点坐标
    /// </summary>
    /// <param name="points"></param>
    public void SetPoints(List<Vector3> points)
    {
        this.linePoints = points;
    }
}
