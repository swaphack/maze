using System.Collections.Generic;
using UnityEngine;

public class LineDrawBehaviour : MonoBehaviour
{
    protected List<Vector3> vertices = new List<Vector3>();

    /// <summary>
    /// 线段信息
    /// </summary>
    public struct LineNode
    {
        public int index;
        public Matrix2x2 matrix;
        public LineNode(Matrix2x2 mat)
        {
            index = 0;
            matrix = mat;
        }
    }

    /// <summary>
    /// 调试时绘制
    /// </summary>
    public bool IsDebugDraw = true;

    /// <summary>
    /// 所有点的信息
    /// </summary>
    protected Stack<LineNode> nodeInfos = new Stack<LineNode>();
    /// <summary>
    /// 当前点的信息
    /// </summary>
    protected LineNode currentNode = new LineNode(new Matrix2x2());
    /// <summary>
    /// 初始化当前点
    /// </summary>
    protected void InitCurrentNode()
    {
        currentNode = new LineNode(new Matrix2x2());
    }

    protected void Update()
    {
        OnDebugDraw();
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

        if (vertices == null || vertices.Count == 0)
        {
            return;
        }

        int index = vertices.Count / 2;
        for (int i = 0; i < index; i++)
        {
            Debug.DrawLine(vertices[i * 2], vertices[i * 2 + 1]);
        }
    }
}
