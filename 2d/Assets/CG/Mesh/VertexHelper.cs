using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// 顶点工具
/// </summary>
public class VertexHelper
{
    /// <summary>
    /// 顶点排序
    /// 从低到高排序
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static Vector3[] SortVertices(Vector3[] vertices)
    {
        if (vertices == null || vertices.Length == 0)
        {
            return null;
        }
        Vector3[] temp = new Vector3[vertices.Length];
        Array.Copy(vertices, temp, vertices.Length);
        Array.Sort(temp, VertexHelper.Compare);
        return temp;
    }

    public static int Compare(Vector3 a, Vector3 b)
    {
        int cx = a.x.CompareTo(b.x);
        int cy = a.y.CompareTo(b.y);
        int cz = a.z.CompareTo(b.z);
        if (cx < 0)
        {
            return cx;
        }
        else if (cx == 0)
        {
            if (cy < 0) return cy;
            else if (cy == 0) return cz;
            else return cy;
        }
        else
        {
            return 1;
        }
    }

    public static int CompareUp(Vector3 a, Vector3 b)
    {
        int cx = a.x.CompareTo(b.x);
        int cy = a.y.CompareTo(b.y);
        if (cx < 0)
        {
            return cx;
        }
        else if (cx == 0)
        {
            if (cy > 0) return -1;
            else if (cy == 0) return 0;
            else return 1;
        }
        else
        {
            return 1;
        }
    }
    /// <summary>
    /// 获取边
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <returns></returns>
    public static Tuple<int, int> GetKey(int srcIndex, int destIndex)
    {
        int minIndex = Mathf.Min(srcIndex, destIndex);
        int maxIndex = Mathf.Max(srcIndex, destIndex);

        return new Tuple<int, int>(minIndex, maxIndex);
    }

    /// <summary>
    /// 获取三角形顶点顺序
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static Tuple<int, int, int> GetKey(int p0, int p1, int p2)
    {
        int m0 = Mathf.Min(p0, p1, p2);
        int m2 = Mathf.Max(p0, p1, p2);
        int m1 = p0 + p1 + p2 - m0 - m2;

        return new Tuple<int, int, int>(m0, m1, m2);
    }

    /// <summary>
    /// 获取距离
    /// </summary>
    /// <param name="srcPosition"></param>
    /// <param name="destPosition"></param>
    /// <returns></returns>
    public static float GetDistance(Vector3 srcPosition, Vector3 destPosition)
    {
        return Vector3.Distance(srcPosition, destPosition);
    }

    /// <summary>
    /// 获取角度
    /// </summary>
    /// <param name="srcPosition"></param>
    /// <param name="destPosition"></param>
    /// <param name="thirdPosition"></param>
    /// <returns></returns>
    public static float GetAngle(Vector3 srcPosition, Vector3 destPosition, Vector3 thirdPosition)
    {
        Vector3 v0 = srcPosition - thirdPosition;
        Vector3 v1 = destPosition - thirdPosition;

        return Vector3.Angle(v0, v1);
    }

    /// <summary>
    /// 获取三角形外接圆圆心坐标
    /// 重心
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static Vector3 GetCentreOfGravity(Vector3 a, Vector3 b, Vector3 c)
    {
        return 1.0f / 3.0f * (a + b + c);
    }

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

    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }

    /// <summary>
    /// 求线段交点
    /// </summary>
    /// <param name="a">ab</param>
    /// <param name="b"></param>
    /// <param name="c">cd</param>
    /// <param name="d"></param>
    /// <param name="IntrPos"></param>
    /// <returns></returns>
    public static bool GetIntersectPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 IntrPos)
    {
        Vector2 ab = b - a;
        Vector2 ac = c - a;
        float abXac = Cross(ab, ac);

        Vector2 ad = d - a;
        float abXad = Cross(ab, ad);

        if (abXac * abXad >= 0)
        {
            return false;
        }

        //以线段cd为准，是否ab在同一侧
        Vector2 cd = d - c;
        Vector2 ca = a - c;
        Vector2 cb = b - c;

        float cdXca = Cross(cd, ca);
        float cdXcb = Cross(cd, cb);
        if (cdXca * cdXcb >= 0)
        {
            return false;
        }
        //计算交点坐标  
        float t = Cross(a - c, d - c) / Cross(d - c, b - a);
        float dx = t * (b.x - a.x);
        float dy = t * (b.y - a.y);

        IntrPos = new Vector2() { x = a.x + dx, y = a.y + dy };
        return true;
    }

    /// <summary>
    /// 是否共面
    /// </summary>
    /// <param name="segPoint0"></param>
    /// <param name="segPoint1"></param>
    /// <param name="checkPoint0"></param>
    /// <param name="checkPoint1"></param>
    /// <returns></returns>
    public static bool IsInSameSide(Vector2 segPoint0, Vector2 segPoint1, Vector2 checkPoint0, Vector2 checkPoint1)
    {
        var pointA = segPoint0;
        var pointB = segPoint1;

        var pointC = checkPoint0;
        var pointD = checkPoint1;

        float c = (pointC.y - pointA.y) * (pointA.x - pointB.x) - (pointC.x - pointA.x) * (pointA.y - pointB.y);
        float d = (pointD.y - pointA.y) * (pointA.x - pointB.x) - (pointD.x - pointA.x) * (pointA.y - pointB.y);

        return c * d > 0;
    }
}
