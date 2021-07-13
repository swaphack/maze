using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class Voronoi
    {
        public HashSet<Edge> GenerateEdgesFromDelaunay(IEnumerable<Triangle> triangulation)
        {
            var voronoiEdges = new HashSet<Edge>();
            foreach (var triangle in triangulation)
            {
                foreach (var neighbor in triangle.TrianglesWithSharedEdge)
                {
                    var edge = new Edge(triangle.Circumcenter, neighbor.Circumcenter);
                    voronoiEdges.Add(edge);
                }
            }

            return voronoiEdges;
        }

        public HashSet<Polygon> GeneratePolygonsFromDelaunay(IEnumerable<Triangle> triangulation, Rect rect)
        {
            var points = new HashSet<Point>();
            foreach (var triangle in triangulation)
            {
                foreach (var point in triangle.Vertices)
                {
                    if (rect.Contains(point.Position)
                        && point.Position != Vector3.zero && point.Position != new Vector3(rect.width, 0)
                        && point.Position != new Vector3(rect.width, rect.height) && point.Position != new Vector3(0, rect.height))
                    {
                        points.Add(point);
                    }
                }
            }

            var voronoiPolygons = new HashSet<Polygon>();
            foreach (var point in points)
            {
                var polygon = new Polygon();
                foreach (var adjacentTriangle in point.AdjacentTriangles)
                {
                    polygon.Add(adjacentTriangle.Circumcenter);
                }

                if (polygon.Points.Count >= 3)
                {
                    voronoiPolygons.Add(polygon);
                }
            }

            return voronoiPolygons;
        }
    }
}