using UnityEngine;

namespace DelaunayVoronoi
{
    public class Edge
    {
        public Point Point1 { get; }
        public Point Point2 { get; }

        public Edge(Vector3 point1, Vector3 point2)
        {
            Point1 = new Point(point1);
            Point2 = new Point(point2);
        }

        public Edge(Point point1, Point point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var edge = obj as Edge;

            var samePoints = Point1 == edge.Point1 && Point2 == edge.Point2;
            var samePointsReversed = Point1 == edge.Point2 && Point2 == edge.Point1;
            return samePoints || samePointsReversed;
        }

        /// <summary>
        /// 是否相接
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsLinkWith(Edge edge)
        {
            if (edge == null) return false;

            return Point2 == edge.Point1 && Point1 != edge.Point2;
        }

        public override int GetHashCode()
        {
            int hCode = (int)Point1.X ^ (int)Point1.Y ^ (int)Point2.X ^ (int)Point2.Y;
            return hCode.GetHashCode();
        }
    }
}