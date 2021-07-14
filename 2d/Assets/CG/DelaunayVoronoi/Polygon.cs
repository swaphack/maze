using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class Polygon
    {
        public HashSet<Point> Points { get; } = new HashSet<Point>();
        public List<Edge> Edges { get; } = new List<Edge>();

        public Polygon()
        {

        }

        public bool Contains(Point point)
        {
            if (point == null) return false;

            return Points.Where(o => o == point).Count() != 0;
        }

        public bool Contains(Edge edge)
        {
            if (edge == null) return false;

            return Edges.Where(o => o.Equals(edge)).Count() != 0;
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

        public void Add(Point point)
        {
            if (point == null) return;
            this.Points.Add(point);
        }

        /// <summary>
        /// 获取凸包
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetConvexHull()
        {
            List<Vector3> points = new List<Vector3>();
            foreach (var item in this.Points)
            {
                points.Add(item.Position);
            }

            return ConvexHull.GetConvexHull(points.ToArray());
        }

        public Vector3[] GetPositions()
        {
            List<Vector3> points = new List<Vector3>();
            foreach (var item in this.Points)
            {
                points.Add(item.Position);
            }

            return points.ToArray();
        }
    }
}
