using System.Collections.Generic;
using System;

using UnityEngine;
using DelaunayVoronoi;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoronoiPolygonSample : DrawLineBehaviour
{
    private DelaunayTriangulator delaunay = new DelaunayTriangulator();
    private Voronoi voronoi = new Voronoi();

    public float Width = 10;
    public float Height = 10;
    /// <summary>
    /// 顶点数
    /// </summary>
    public int PointCount = 10;
    /// <summary>
    /// 扩展边
    /// </summary>
    public bool ExpandEdge = false;
    /// <summary>
    /// 去掉而外的边
    /// </summary>
    public bool CutOffEdge = false;

    private void Start()
    {
        InitVoronoi(PointCount);
    }

    protected void InitVoronoi(int pointCount)
    {
        var points = delaunay.GeneratePoints(pointCount, Width, Height);
        var triangulation = delaunay.BowyerWatson(points);
        DrawTriangulation(triangulation);

        var vornoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);
        DrawVoronoi(vornoiEdges);
    }

    void DrawVoronoi(HashSet<Edge> voronoiEdges)
    {
        List<Edge> rectEdges = new List<Edge>();
        rectEdges.Add(new Edge(new Point(0, 0), new Point(Width, 0)));
        rectEdges.Add(new Edge(new Point(Width, 0), new Point(Width, Height)));
        rectEdges.Add(new Edge(new Point(Width, Height), new Point(0, Height)));
        rectEdges.Add(new Edge(new Point(0, Height), new Point(0, 0)));

        Rect rect = new Rect(0, 0, Width, Height);
        if (ExpandEdge)
        {
            Dictionary<Point, List<Edge>> samePointEdges = new Dictionary<Point, List<Edge>>();
            foreach (var edge in voronoiEdges)
            {
                if (!samePointEdges.ContainsKey(edge.Point1))
                {
                    samePointEdges.Add(edge.Point1, new List<Edge>());
                }
                samePointEdges[edge.Point1].Add(edge);

                if (!samePointEdges.ContainsKey(edge.Point2))
                {
                    samePointEdges.Add(edge.Point2, new List<Edge>());
                }
                samePointEdges[edge.Point2].Add(edge);
            }

            foreach (var item in samePointEdges)
            {
                if (item.Value.Count == 2)
                {
                    if (rect.Contains(item.Key.Position))
                    {
                        var d0 = item.Value[0].GetDirection(item.Key);
                        var d1 = item.Value[1].GetDirection(item.Key);
                        var offset = (d0.normalized + d1.normalized) * (Width + Height);
                        voronoiEdges.Add(new Edge(item.Key.Position, item.Key.Position + offset));
                    }
                }
            }
        }


        List<Edge> allEdges = new List<Edge>();
        HashSet<Vector3> points = new HashSet<Vector3>();
        if (CutOffEdge)
        {
            foreach (var edge in voronoiEdges)
            {
                List<Vector2> insetctPoints = new List<Vector2>();
                List<Vector2> otherPoints = new List<Vector2>();
                foreach (var item in rectEdges)
                {
                    // 求交点
                    Vector2 intersectPoint = Vector2.zero;
                    if (VertexHelper.GetIntersectPoint(
                        item.Point1.Position, item.Point2.Position,
                        edge.Point1.Position, edge.Point2.Position,
                        ref intersectPoint))
                    {
                        insetctPoints.Add(intersectPoint);

                        // 剔除另一端不满足的点
                        if (VertexHelper.GetPointPosition(edge.Point1.Position, item.Point1.Position, item.Point2.Position) == 1)
                        {
                            otherPoints.Add(edge.Point2.Position);
                        }
                        else
                        {
                            otherPoints.Add(edge.Point1.Position);
                        }
                    }
                }
                if (insetctPoints.Count == 2)
                {
                    points.Add(insetctPoints[0]);
                    points.Add(insetctPoints[1]);

                    allEdges.Add(new Edge(insetctPoints[0], insetctPoints[1]));
                }
                else if (insetctPoints.Count == 1 && otherPoints.Count == 1)
                {
                    points.Add(insetctPoints[0]);
                    points.Add(otherPoints[0]);

                    allEdges.Add(new Edge(insetctPoints[0], otherPoints[0]));
                }
                else
                {
                    //排除大区域外的点
                    if (!rect.Contains(edge.Point1.Position)
                        && !rect.Contains(edge.Point2.Position))
                    {
                        continue;
                    }
                    points.Add(edge.Point1.Position);
                    points.Add(edge.Point2.Position);

                    allEdges.Add(edge);
                }
            }
        }
        else
        {
            foreach (var edge in voronoiEdges)
            {
                points.Add(edge.Point1.Position);
                points.Add(edge.Point2.Position);

                allEdges.Add(edge);
            }
        }

        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(Width, 0));
        points.Add(new Vector2(Width, Height));
        points.Add(new Vector2(0, Height));

        List<Vector3> sortPoints = new List<Vector3>();
        sortPoints.AddRange(points);

        sortPoints.Sort((a, b) => {
            int ret = a.x.CompareTo(b.x);
            if (ret == 0) return -a.y.CompareTo(b.y);
            return ret;
        });

        // 从左下角开始查找
        HashSet<Polygon> polygons = new HashSet<Polygon>();


    }

    void DrawTriangulation(IEnumerable<Triangle> triangulation)
    {
        if (triangulation == null)
        {
            return;
        }

        List<Vector3> vertices = new List<Vector3>();

        foreach(var item in triangulation)
        {
            var point0 = item.Vertices[0].Position;
            var point1 = item.Vertices[1].Position;
            var point2 = item.Vertices[2].Position;

            vertices.Add(point0); vertices.Add(point1);
            vertices.Add(point1); vertices.Add(point2);
            vertices.Add(point2); vertices.Add(point0);
        }

        this.SetPoints(vertices);
    }

    void DrawPolygon(HashSet<Polygon> voronoiPolygons)
    {
        Debug.LogFormat("voronoi polygon count {0}", voronoiPolygons.Count);
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Color> colors = new List<Color>();

        foreach (var item in voronoiPolygons)
        {
            var convexHullPoints = item.GetConvexHull();
            if (convexHullPoints != null && convexHullPoints.Length > 3)
            {
                int[] polygonIndices = new int[convexHullPoints.Length];
                for (int i = 0; i < convexHullPoints.Length; i++)
                {
                    var point = convexHullPoints[i];
                    int index = vertices.IndexOf(convexHullPoints[i]);
                    if (index == -1)
                    {
                        vertices.Add(point);
                        polygonIndices[i] = vertices.Count - 1;
                        float value = UnityEngine.Random.value;
                        colors.Add(new Color(value, 0.5f, 1- value));
                    }
                    else
                    {
                        polygonIndices[i] = index;
                    }
                }

                for (int i = 0; i < convexHullPoints.Length - 2; i++)
                {
                    indices.Add(polygonIndices[0]);
                    indices.Add(polygonIndices[i + 1]);
                    indices.Add(polygonIndices[i + 2]);
                }
            }
        }
        Debug.LogFormat("vertice count {0}", vertices.Count);
        Debug.LogFormat("indices count {0}", indices.Count);
        Debug.LogFormat("colors count {0}", colors.Count);
        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.colors = colors.ToArray();
        meshFilter.mesh = mesh;
    }
}