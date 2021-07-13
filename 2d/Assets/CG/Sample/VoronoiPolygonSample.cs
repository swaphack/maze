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

        var vornoiPolygons = voronoi.GeneratePolygonsFromDelaunay(triangulation);
        DrawVoronoi(vornoiPolygons);
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

    void DrawVoronoi(HashSet<Polygon> voronoiPolygons)
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