using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 凸包
/// </summary>
public class ConvexHull
{
    /// <summary>
    /// 在平面上，获取点与线的位置关系
    /// 返回结果：-1左边，0线上，1右边
    /// </summary>
    /// <param name="point"></param>
    /// <param name="src"></param>
    /// <param name="dest"></param>
    /// <returns>-1左边，0线上，1右边</returns>
    public static int GetPointPosition(Vector2 point, Vector2 src, Vector2 dest)
    {
        Vector2 v1 = point - src;
        Vector2 v2 = point - dest;

        float value = v1.x * v2.y - v1.y * v2.x;
        if (value > 0) return -1;
        else if (value < 0) return 1;
        else return 0;
    }

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
            if (ret == 0)
            {
                return b.y.CompareTo(a.y);
            }

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

                if (GetPointPosition(c, a, b) != 1)
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
                if (GetPointPosition(c, a, b) != 1)
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
