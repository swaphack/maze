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

            var samePoints = Point1.Equals(edge.Point1) && Point2.Equals(edge.Point2);
            var samePointsReversed = Point1.Equals(edge.Point2) && Point2.Equals(edge.Point1);
            return samePoints || samePointsReversed;
        }


        public Vector3 GetDirection(Point destPoint)
        {
            if (Point1.Equals(destPoint)) return Point1.Position - Point2.Position;
            else if (Point2.Equals(destPoint)) return Point2.Position - Point1.Position;

            return Vector3.zero;
        }

        /// <summary>
        /// 获取另一个端点
        /// </summary>
        /// <param name="onePoint"></param>
        /// <returns></returns>
        public Point GetOtherPoint(Point onePoint)
        {
            if (Point1.Equals(onePoint))
            {
                return Point2;
            }
            else if (Point2.Equals(onePoint))
            {
                return Point1;
            }
            return null;
        }

        /// <summary>
        /// 获取共端点另一个不相同的点
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public Point GetOtherPointWithSharedPoint(Edge edge)
        {
            var point = GetSharedPoint(edge);
            if (point == null) return null;
            return GetOtherPoint(point);
        }

        /// <summary>
        /// 查找共点
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public Point GetSharedPoint(Edge edge)
        {
            if (edge == null) return null;
            if (this.Point1.Equals(edge.Point1) || this.Point1.Equals(edge.Point2))
            {
                return this.Point1;
            }
            if (this.Point2.Equals(edge.Point1) || this.Point2.Equals(edge.Point2))
            {
                return this.Point2;
            }
            return null;
        }

        public override int GetHashCode()
        {
            int hCode = Point1.GetHashCode()^Point2.GetHashCode();
            return hCode.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(Edge)} {Point1:#}-{Point2:#}";
        }
    }
}