using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class Voronoi
    {
        public IEnumerable<Edge> GenerateEdgesFromDelaunay(IEnumerable<Triangle> triangulation)
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

        public class Polygon
        {
            public HashSet<Point> Points { get; } = new HashSet<Point>();
            public List<Edge> Edges { get; } = new List<Edge>();

            public HashSet<Polygon> Neighbor { get; } = new HashSet<Polygon>();

            public Polygon()
            {

            }
            public bool Contains(Edge edge)
            {
                if (edge == null) return false;

                return Edges.Where(o=>o.Equals(edge)) != null;
            }

            public Edge GetTailEdge()
            {
                if (Edges.Count == 0) return null;
                return Edges[Edges.Count - 1];
            }

            public void Add(Edge edge)
            {
                if (edge == null) return;

                if (this.Contains(edge))
                {
                    return;
                }
                Edges.Add(edge);
                this.Points.Add(edge.Point1);
                this.Points.Add(edge.Point2);
            }

            public bool Contains(Point point)
            {
                if (point == null) return false;

                return Points.Where(o => o == point) != null;
            }
        }

        private List<Polygon> ContainsEdge(List<Polygon> polygons, Edge edge)
        {

            if (polygons == null || edge == null) return null;

            List<Polygon> includePolygons = new List<Polygon>();

            foreach (var item in polygons)
            {
                if (item.Contains(edge))
                {
                    //includePolygons.Add()
                }
            }

            return includePolygons;
        }

        public IEnumerable<Polygon> GeneratePolygonFromDelaunay(List<Edge> voronoiEdges)
        {
            voronoiEdges.Sort((a, b)=>VertexHelper.Compare(a.Point1.Position, b.Point1.Position));

            List<Polygon> polygons = new List<Polygon>();

            foreach(var edge in voronoiEdges)
            {
                /*
                var polygon = new Polygon();
                if (!ContainsEdge(polygons, edge, ref polygon))
                {
                    polygon = new Polygon();
                    polygons.Add(polygon);
                }
                else
                {

                }
                if (polygon.Edges.Count == 0)
                {
                    polygon.Add(edge);
                }
                else if (!polygon.Contains(edge))
                {
                    var tailEdge = polygon.GetTailEdge();
                    if (tailEdge != null 
                        && tailEdge.IsLinkWith(edge)
                        && VertexHelper.GetPointPosition(edge.Point2.Position, tailEdge.Point1.Position, tailEdge.Point2.Position) == 1)
                    {
                        polygon.Add(edge);
                    }
                } 
                */
            }

            return polygons;
        }
    }
}