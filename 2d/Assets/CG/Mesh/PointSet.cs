using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 点集合
/// </summary>
public class PointSet
{
    public List<Vector3> Points { get; } = new List<Vector3>();

    public PointSet()
    {

    }

    public PointSet(List<Vector3> points)
    {
        this.SetPoints(points);
    }

    /// <summary>
    /// 设置点 并进行排序，优先x 小，后y小的方式
    /// </summary>
    /// <param name="points"></param>
    public void SetPoints(List<Vector3> points)
    {
        Points.Clear();
        if (points == null) return;
        foreach (var item in points)
        {
            Points.Add(item);
        }

        Points.Sort((a, b) =>
        {
            int ret0 = a.x.CompareTo(b.x);
            int ret1 = b.x.CompareTo(b.y);
            if (ret0 > 1) return ret1;
            else return ret0;
        });
    }

    /// <summary>
    /// 查找距离标目最近的点
    /// </summary>
    /// <param name="inPoint"></param>
    /// <param name="outPoint"></param>
    /// <returns></returns>
    public bool FindClosestPoint(Vector3 inPoint, out Vector3 outPoint)
    {
        outPoint = Vector3.zero;
        if (Points.Count == 0)
        {
            return false;
        }

        Vector3 lastPoint = Vector3.zero;
        float lastDistance = -1;
        foreach (var item in Points)
        {
            float distance = Vector3.Distance(item, inPoint);
            if (lastDistance == -1 || lastDistance > distance)
            {
                lastDistance = distance;
                lastPoint = item;
            }
        }
        outPoint = lastPoint;
        return lastDistance != -1;
    }

    /// <summary>
    /// 在指定区间生成随机点
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="totalCount">总数量</param>
    /// <returns></returns>
    public static PointSet Create(float width, float height, int totalCount)
    {
        if (width <= 0 || height <= 0 || totalCount < 1)
        {
            return null;
        }
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < totalCount; i++)
        {
            float x = Random.Range(0, width);
            float y = Random.Range(0, height);
            points.Add(new Vector3(x, y));
        }

        return new PointSet(points);
    }

    /// <summary>
    /// 将区间划分成n个子区间，再在子区间生成随机点
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="horizontalCount">水平区间数</param>
    /// <param name="verticalCount">垂直区间数</param>
    /// <param name="totalCount">总数量</param>
    /// <returns></returns>
    public static PointSet Create(float width, float height, int horizontalCount, int verticalCount, int totalCount)
    {
        if (width <= 0 || height <= 0 || horizontalCount < 1 || verticalCount < 1 || totalCount < 1)
        {
            return null;
        }

        List<Vector3> points = new List<Vector3>();
        int index = 0;

        float perWidth = width / horizontalCount;
        float perHeight = height / verticalCount;
        int sizeCount = horizontalCount * verticalCount;

        for (int i = 0; i < totalCount; i++)
        {
            int m = (index % sizeCount) / horizontalCount;
            int n = (index % sizeCount) % horizontalCount;

            float orginX = n * perWidth;
            float originY = m * perHeight;

            float x = Random.Range(orginX, orginX + perWidth);
            float y = Random.Range(originY, originY + perHeight);
            points.Add(new Vector3(x, y));
            index++;
        }

        return new PointSet(points);
    }

    /// <summary>
    /// 生成点集合
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="horizontalCount"></param>
    /// <param name="verticalCount"></param>
    /// <returns></returns>
    public static PointSet Create(float width, float height, int horizontalCount, int verticalCount)
    {
        return Create(width, height, horizontalCount, verticalCount, horizontalCount * verticalCount);
    }
}
