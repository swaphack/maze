using System.Collections.Generic;
using System.Collections;
using System;

using UnityEngine;
using DelaunayVoronoi;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DelaunayTriangleSample : MonoBehaviour
{
    private DelaunayTriangulator delaunay = new DelaunayTriangulator();
    public float Width = 100;
    public float Height = 100;
    /// <summary>
    /// 顶点数
    /// </summary>
    public int PointCount = 10;

    private void Start()
    {
        InitTriangulation(PointCount);
    }

    protected void InitTriangulation(int pointCount)
    {
        var points = delaunay.GeneratePoints(pointCount, Width, Height);
        var triangulation = delaunay.BowyerWatson(points);
        DrawTriangulation(points, triangulation);
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
}