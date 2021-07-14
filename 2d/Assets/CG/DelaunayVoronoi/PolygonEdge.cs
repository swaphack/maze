using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DelaunayVoronoi
{
    /// <summary>
    /// 多边形的边
    /// </summary>
    public class PolygonEdge
    {
        /// <summary>
        /// 边
        /// </summary>
        public Edge Edge { get; }
        /// <summary>
        /// 共顶点边
        /// </summary>
        public Dictionary<Point, List<PolygonEdge>> SharedPointEdges { get; } = new Dictionary<Point, List<PolygonEdge>>();
        /// <summary>
        /// 共边多边形
        /// </summary>
        public HashSet<Polygon> SharedEdgePolygons { get; } = new HashSet<Polygon>();

        public PolygonEdge(Edge edge)
        {
            Edge = edge;
        }

        /// <summary>
        /// 添加共顶点边
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygonEdge"></param>
        private void _addSharedPointEdge(Point point, PolygonEdge polygonEdge)
        {
            if (!this.SharedPointEdges.ContainsKey(point))
            {
                this.SharedPointEdges.Add(point, new List<PolygonEdge>());
            }
            this.SharedPointEdges[point].Add(polygonEdge);
        }

        /// <summary>
        /// 添加共顶点边
        /// </summary>
        /// <param name="polygonEdge"></param>
        /// <returns></returns>
        public Point AddSharedPointEdge(PolygonEdge polygonEdge)
        {
            if (polygonEdge == null || Edge.Equals(polygonEdge.Edge)) return null;
            var point = this.Edge.GetSharedPoint(polygonEdge.Edge);
            if (point == null) return null;

            this._addSharedPointEdge(point, polygonEdge);
            polygonEdge._addSharedPointEdge(point, this);

            return point;
        }
        /// <summary>
        /// 获取共点的边
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public List<PolygonEdge> GetPolygonEdges(Point point)
        {
            List<PolygonEdge> values = null;
            if (SharedPointEdges.TryGetValue(point, out values))
            {
                return values;
            }
            return null;
        }
        /// <summary>
        /// 获取构成凸包的边
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public PolygonEdge GetConvexPolygonEdge(Point endPoint)
        {
            var startPoint = this.Edge.GetOtherPoint(endPoint);
            if (startPoint == null)
            {
                return null;
            }
            List<PolygonEdge> values = this.GetPolygonEdges(endPoint);
            if (values != null && values.Count > 0)
            {
                List<PolygonEdge> sharedEdges = new List<PolygonEdge>();
                foreach (var item in values)
                {
                    var e0 = item.Edge.GetOtherPoint(endPoint);
                    if (e0 != null)
                    {
                        if (VertexHelper.GetPointPosition(e0.Position, startPoint.Position, endPoint.Position) == 1)
                        {// 右边
                            sharedEdges.Add(item);
                        }
                    }
                    
                }
                if (sharedEdges.Count == 0)
                {
                    return null;
                }
                sharedEdges.Sort((a, b) => {
                    var e0 = a.Edge.GetOtherPoint(endPoint);
                    var e1 = b.Edge.GetOtherPoint(endPoint);

                    var v0 = e0.Position - endPoint.Position;
                    var v1 = e1.Position - endPoint.Position;
                    var v2 = startPoint.Position - endPoint.Position;
                    float a0 = Vector3.Angle(v0, v2);
                    float a1 = Vector3.Angle(v1, v2);
                    return a0.CompareTo(a1);
                });

                return sharedEdges[0];
            }
            else
            {
                return null;
            }
        }


        public void LinkTo(Polygon polygon)
        {
            if (polygon == null)
            {
                return;
            }
            this.SharedEdgePolygons.Add(polygon);
        }
        /// <summary>
        /// 创建多边形的边
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static Dictionary<Edge, PolygonEdge> CreatePolygonEdges(HashSet<Edge> edges)
        {
            if (edges == null || edges.Count == 0)
            {
                return null;
            }

            Dictionary<Edge,PolygonEdge> polygonEdges = new Dictionary<Edge,PolygonEdge>();
            HashSet<Point> sharedPoints = new HashSet<Point>();
            foreach(var item in edges)
            {
                if (!polygonEdges.ContainsKey(item))
                {
                    polygonEdges.Add(item, new PolygonEdge(item));
                }
                var temp = polygonEdges[item];
                foreach (var polygonEdge in polygonEdges)
                {
                    var point = polygonEdge.Value.AddSharedPointEdge(temp);
                    if (point != null)
                    {
                        sharedPoints.Add(point);

                        //Debug.LogFormat("shared point {0}", point.ToString());
                    }
                }
            }

            

            return polygonEdges;
        }

        /// <summary>
        /// 获取凸多边形 从左到右,顺时针方向
        /// </summary>
        /// <param name="polygonEdge"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static Polygon GetConvexPolygon(PolygonEdge polygonEdge)
        {
            if (polygonEdge == null || polygonEdge.SharedPointEdges.Count == 0) 
                return null;

            Polygon polygon = new Polygon();
            polygon.Add(polygonEdge.Edge);

            var temp = polygonEdge;
            var endPoint = polygonEdge.Edge.Point2;
            do
            {
                temp = temp.GetConvexPolygonEdge(endPoint);
                if (temp == null)
                {
                    return null;
                }

                if (temp == polygonEdge)
                {
                    break;
                }
                endPoint = temp.Edge.GetOtherPoint(endPoint);
                polygon.Add(temp.Edge);

            } while (true);

            return polygon;
        }
    }
}
