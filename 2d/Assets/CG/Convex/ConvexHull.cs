using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 凸包
/// </summary>
public class ConvexHull
{
    /// <summary>
    /// 获取凸包
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Vector2[] GetConvexHull(Vector2[] points)
    {
        if (points == null || points.Length < 3)
        {
            return null;
        }

        // 排序
        Array.Sort(points, (a, b) =>
        {
            int ret = a.x.CompareTo(b.x);
            if (ret == 0) return -a.y.CompareTo(b.y);
            return ret;
        });

        List<Vector2> upList = new List<Vector2>();
        upList.Add(points[0]);
        upList.Add(points[1]);

        for (var i = 2; i < points.Length; i++)
        {
            upList.Add(points[i]);
            while (upList.Count >= 3)
            {
                var a = upList[upList.Count - 3];
                var b = upList[upList.Count - 2];
                var c = upList[upList.Count - 1];

                if (VertexHelper.GetPointPosition(c, a, b) != 1)
                {
                    upList.Remove(b);
                }
                else
                {
                    break;
                }
            }
        }

        var lowerList = new List<Vector2>();
        lowerList.Add(points[points.Length - 1]);
        lowerList.Add(points[points.Length - 2]);

        for (var i = points.Length - 3; i >= 0; i--)
        {
            lowerList.Add(points[i]);
            while (lowerList.Count >= 3)
            {
                var a = lowerList[lowerList.Count - 3];
                var b = lowerList[lowerList.Count - 2];
                var c = lowerList[lowerList.Count - 1];
                if (VertexHelper.GetPointPosition(c, a, b) != 1)
                {
                    lowerList.Remove(b);
                }
                else
                {
                    break;
                }
            }
        }

        lowerList.RemoveAt(0);
        lowerList.RemoveAt(lowerList.Count - 1);

        upList.AddRange(lowerList);

        return upList.ToArray();
    }

    /// <summary>
    /// 获取凸包
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static Vector3[] GetConvexHull(Vector3[] points)
    {
        if (points == null || points.Length < 3)
        {
            return null;
        }

        // 排序
        Array.Sort(points, (a, b) =>
        {
            int ret = a.x.CompareTo(b.x);
            if (ret == 0) return -a.y.CompareTo(b.y);
            return ret;
        });


        List<Vector3> upList = new List<Vector3>();
        upList.Add(points[0]);
        upList.Add(points[1]);

        for (var i = 2; i < points.Length; i++)
        {
            upList.Add(points[i]);
            while (upList.Count >= 3)
            {
                var a = upList[upList.Count - 3];
                var b = upList[upList.Count - 2];
                var c = upList[upList.Count - 1];

                if (VertexHelper.GetPointPosition(c, a, b) != 1)
                {
                    upList.Remove(b);
                }
                else
                {
                    break;
                }
            }
        }

        var lowerList = new List<Vector3>();
        lowerList.Add(points[points.Length - 1]);
        lowerList.Add(points[points.Length - 2]);

        for (var i = points.Length - 3; i >= 0; i--)
        {
            lowerList.Add(points[i]);
            while (lowerList.Count >= 3)
            {
                var a = lowerList[lowerList.Count - 3];
                var b = lowerList[lowerList.Count - 2];
                var c = lowerList[lowerList.Count - 1];
                if (VertexHelper.GetPointPosition(c, a, b) != 1)
                {
                    lowerList.Remove(b);
                }
                else
                {
                    break;
                }
            }
        }

        lowerList.RemoveAt(0);
        lowerList.RemoveAt(lowerList.Count - 1);

        upList.AddRange(lowerList);

        return upList.ToArray();
    }
}
