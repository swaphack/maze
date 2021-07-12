
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 一维地形中点生成器
/// </summary>
public class MidpointTerrian
{
    public delegate void MidpointHandle(Vector3 startPoint, Vector3 midPoint, Vector3 endPoint);
    /// <summary>
    /// 起始位置
    /// </summary>
    public Vector3 Src { get; set; }
    /// <summary>
    /// 终点位置
    /// </summary>
    public Vector3 Dest { get; set; }
    /// <summary>
    /// 偏移位置
    /// </summary>
    public Vector3 OffsetPosition { get; set; }

    /// <summary>
    /// 生成点
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<Vector3> CreatePoints(int count)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(Src);
        points.Add(Dest);

        if (Src == Dest)
        {
            return points;
        }

        Vector3 offset0 = OffsetPosition;
        Vector3 offset1 = -OffsetPosition;

        Vector3 minOffset, maxOffset;
        FractalHelper.GetRange(offset0, offset1, out minOffset, out maxOffset);

        for (int i = 0; i < count; i++)
        {
            float value = 1.0f / Mathf.Pow(2, i);
            offset0 = minOffset * value;
            offset1 = maxOffset * value;
            List<Vector3> newPoints = new List<Vector3>();
            for (int j = 0; j < points.Count - 1; j++)
            {
                Vector3 src = points[j];
                Vector3 dest = points[j + 1];
                Vector3 midpoint = FractalHelper.GetMidpoint(src, dest);
                Vector3 offset = FractalHelper.GetRandomPosition(offset0, offset1);
                midpoint += offset;
                newPoints.Add(src);
                newPoints.Add(midpoint);
            }
            newPoints.Add(points[points.Count - 1]);
            points = newPoints;
        }

        return points;
    }
}
