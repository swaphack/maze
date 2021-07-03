using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct PathNode
{
    /// <summary>
    /// 顶点索引
    /// </summary>
    public int Index;
    /// <summary>
    /// 邻接点信息 {顶点索引，距离}
    /// </summary>
    public Dictionary<int, float> neighbors;
}


public class PathNodes
{
    /// <summary>
    /// 邻接点信息
    /// </summary>
    public Dictionary<Tuple<int, int>, float> neighbors;

    public Tuple<int, int> GetKey(int srcIndex, int destIndex)
    {
        int minIndex = Mathf.Min(srcIndex, destIndex);
        int maxIndex = Mathf.Max(srcIndex, destIndex);

        return new Tuple<int, int>(minIndex, maxIndex);
    }
}
