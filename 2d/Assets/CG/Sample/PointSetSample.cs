using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 点集合例子
/// </summary>
public class PointSetSample : DrawLineBehaviour
{
    public float Width = 20;
    public float Height = 20;

    public int HorizontalCount = 5;
    public int VerticalCount = 5;

    public int PointCount = 40;

    public int QuadMaxPointCount = 2;

    private PointSet pointSet = null;
    private QuadTree quadTree = new QuadTree();

    void Start()
    {
        pointSet = PointSet.Create(Width, Height, HorizontalCount, VerticalCount, PointCount);
        if (pointSet == null || pointSet.Points == null || pointSet.Points.Count == 0)
        {
            return;
        }
        quadTree.SetRectange(Vector3.zero, new Vector3(Width, Height, 0));
        quadTree.MaxPointCount = QuadMaxPointCount;
        quadTree.SetPoints(pointSet.Points);

        List<Vector3> points = new List<Vector3>();
        List<QuadTree.QuadNode> nodes = new List<QuadTree.QuadNode>();
        List<QuadTree.QuadNode> temp = new List<QuadTree.QuadNode>();
        temp.Add(quadTree.Root);

        do
        {
            nodes = new List<QuadTree.QuadNode>();
            foreach (var item in temp)
            {
                float x = item.Rectangle.x;
                float y = item.Rectangle.y;
                float w = item.Rectangle.width;
                float h = item.Rectangle.height;
                points.Add(new Vector3(x, y, 0));
                points.Add(new Vector3(x + w, y, 0));
                points.Add(new Vector3(x + w, y, 0));
                points.Add(new Vector3(x + w, y + h, 0));
                points.Add(new Vector3(x + w, y + h, 0));
                points.Add(new Vector3(x, y + h, 0));
                points.Add(new Vector3(x, y + h, 0));
                points.Add(new Vector3(x, y, 0));

                if (item.Children != null)
                {
                    foreach (var child in item.Children)
                    {
                        nodes.Add(child);
                    }
                }
            }
            temp = nodes;
        } while (temp.Count != 0);

        Vector3 outPoint;
        int randIndex = Random.Range(HorizontalCount * VerticalCount / 2, HorizontalCount * VerticalCount);
        Vector3 inPoint = pointSet.Points[randIndex];
        if (quadTree.FindClosestPoint(inPoint, out outPoint))
        {
            Debug.LogFormat("src point {0}", inPoint);
            Debug.LogFormat("dest point {0}", outPoint);
            points.Add(inPoint);
            points.Add(outPoint);
        }

        this.SetPoints(points);

        foreach (var item in pointSet.Points)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cube.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            cube.transform.position = item + this.transform.localPosition;
            cube.transform.SetParent(this.transform);
        }
    }
}
