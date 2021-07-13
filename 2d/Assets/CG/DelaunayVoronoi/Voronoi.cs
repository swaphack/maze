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

        public HashSet<Polygon> GeneratePolygonsFromDelaunay(IEnumerable<Triangle> triangulation)
        {
            var voronoiPolygons = new HashSet<Polygon>();
            foreach (var triangle in triangulation)
            {
                var polygon = new Polygon();
                foreach (var point in triangle.Vertices)
                {
                    foreach (var adjacentTriangle in point.AdjacentTriangles)
                    {
                        polygon.Add(adjacentTriangle.Circumcenter);
                    }
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