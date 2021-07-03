
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 一维地形终点生成器
/// </summary>
public class Terrian1DMidpointGenerator
{
    public delegate void MidpointHandle(Vector2 midpoint, out Vector2 outPoint);
    /// <summary>
    /// 起始位置
    /// </summary>
    public Vector2 Src { get; set; }
    /// <summary>
    /// 终点位置
    /// </summary>
    public Vector2 Dest { get; set; }
    /// <summary>
    /// 生成处理
    /// </summary>
    public MidpointHandle GenerateHandle { get; set; }

    public List<Vector2> CreatePoints(int count)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(Src);
        points.Add(Dest);

        if (Src == Dest || GenerateHandle == null)
        {
            return points;
        }

        for (int i = 0; i < count; i++)
        {
            List<Vector2> newPoints = new List<Vector2>();
            for(int j = 0; j < points.Count-1; j++)
            {
                Vector2 src = points[j];
                Vector2 dest = points[j + 1];
                Vector2 midpoint = createMidpoint(src, dest);
                newPoints.Add(src);
                newPoints.Add(dest);
                newPoints.Add(midpoint);
            }
            points = newPoints;
        }

        return points;
    }

    protected Vector2 createMidpoint(Vector2 src, Vector2 dest)
    {
        Vector2 midpoint = 0.5f * (src + dest);

        Vector2 outPoint = midpoint;
        if (GenerateHandle != null)
        {
            GenerateHandle(midpoint, out outPoint);
        }
        return outPoint;
    }
}
