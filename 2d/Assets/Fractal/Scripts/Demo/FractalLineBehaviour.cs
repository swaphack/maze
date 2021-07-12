using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 分形中的直线
/// </summary>
public class FractalLineBehaviour : DrawLineBehaviour
{
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
}
