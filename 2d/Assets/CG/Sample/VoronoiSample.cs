using System.Collections.Generic;
using System.Collections;
using System;

using UnityEngine;
using DelaunayVoronoi;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoronoiSample : DrawLineBehaviour
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
        DrawTriangulation(points, triangulation);

        var vornoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);
        DrawVoronoi(vornoiEdges);
    }

    void DrawTriangulation(IEnumerable<Point> points, IEnumerable<Triangle> triangulation)
    {
        if (points == null || triangulation == null)
        {
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Triangle> triangles = new List<Triangle>();
        List<Color> colors = new List<Color>();
        foreach (var item in points)
        {
            vertices.Add(item.Position);
            colors.Add(new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
        }

        triangles.AddRange(triangulation);

        int[] indices = new int[3 * triangles.Count];

        for (int i = 0; i < triangles.Count; i++)
        {
            var point0 = triangles[i].Vertices[0].Position;
            var point1 = triangles[i].Vertices[1].Position;
            var point2 = triangles[i].Vertices[2].Position;

            int p0 = vertices.IndexOf(point0);
            int p1 = vertices.IndexOf(point1);
            int p2 = vertices.IndexOf(point2);

            if (VertexHelper.GetPointPosition(point0, point1, point2) == 1)
            {
                indices[3 * i + 0] = p0;
                indices[3 * i + 1] = p1;
                indices[3 * i + 2] = p2;
            }
            else
            {
                indices[3 * i + 0] = p0;
                indices[3 * i + 1] = p2;
                indices[3 * i + 2] = p1;
            }
        }

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices;
        mesh.colors = colors.ToArray();
        meshFilter.mesh = mesh;
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


        List<Vector3> points = new List<Vector3>();
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
                }
                else if (insetctPoints.Count == 1 && otherPoints.Count == 1)
                {
                    points.Add(insetctPoints[0]);
                    points.Add(otherPoints[0]);
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
                }
            }
        }
        else
        {
            foreach (var edge in voronoiEdges)
            {
                points.Add(edge.Point1.Position);
                points.Add(edge.Point2.Position);
            }
        }

        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(Width, 0));

        points.Add(new Vector2(Width, 0));
        points.Add(new Vector2(Width, Height));

        points.Add(new Vector2(Width, Height));
        points.Add(new Vector2(0, Height));

        points.Add(new Vector2(0, Height));
        points.Add(new Vector2(0, 0));

        this.SetPoints(points);
    }
}