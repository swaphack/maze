using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ConvexHullSample : DrawLineBehaviour
{
    /// <summary>
    /// 点个数
    /// </summary>
    [Range(0, 100)]
    public int PointCount = 100;
    [Range(0, 10)]
    public float maxWidth = 1;
    [Range(0, 10)]
    public float maxHeight = 1;

    // Start is called before the first frame update
    void Start()
    {
        InitConvexHull();
    }

    void InitConvexHull()
    {
        if (PointCount < 3)
        {
            return;
        }

        Vector2[] randomPoints = new Vector2[PointCount];

        for (int i = 0; i < PointCount; i++)
        {
            float x = UnityEngine.Random.Range(0, maxWidth);
            float y = UnityEngine.Random.Range(0, maxHeight);

            randomPoints[i] = new Vector2(x, y);
        }

        Array.Sort(randomPoints, (a, b) =>
        {
            int ret = a.x.CompareTo(b.x);
            if (ret == 0) return -a.y.CompareTo(b.y);
            return ret;
        });

        foreach (var item in randomPoints)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cube.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            cube.transform.position = new Vector3(item.x, item.y) + this.transform.localPosition;
            cube.transform.SetParent(this.transform);
        }

        Vector2[] outPoint = ConvexHull.GetConvexHull(randomPoints);
        if (outPoint == null || outPoint.Length < 3)
        {
            return;
        }

        int count = outPoint.Length;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            points.Add(outPoint[i % count]);
            points.Add(outPoint[(i + 1) % count]);
        }

        this.SetPoints(points);
    }
}
