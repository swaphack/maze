using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 四叉树 左下角开始
/// </summary>
public class QuadTree
{
    /// <summary>
    /// 四叉树节点
    /// </summary>
    public class QuadNode
    {
        public const int LEFT_BOTTOM = 0;
        public const int LEFT_TOP = 1;
        public const int RIGHT_BOTTOM = 2;
        public const int RIGHT_TOP = 3;


        /// <summary>
        /// 是否已经分割
        /// </summary>
        public bool IsSpilt { get { return Children != null; } }

        /// <summary>
        /// 点坐标
        /// </summary>
        public List<Vector3> Points { get; } = new List<Vector3>();

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 矩形框
        /// </summary>
        public Rect Rectangle { get; }
        /// <summary>
        /// 父节点
        /// </summary>
        public QuadNode Parent { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<QuadNode> Children { get; private set; }

        public QuadNode(int index, Rect rect)
        {
            Rectangle = rect;
            Index = index;
        }

        /// <summary>
        /// 分离
        /// </summary>
        public void QuadSplit()
        {
            if (IsSpilt)
            {
                return;
            }

            Children = new List<QuadNode>(4);

            float width = Rectangle.width * 0.5f;
            float height = Rectangle.height * 0.5f;

            Children.Add(new QuadNode(LEFT_BOTTOM, new Rect(Rectangle.x, Rectangle.y, width, height)));
            Children.Add(new QuadNode(LEFT_TOP, new Rect(Rectangle.x, Rectangle.y + height, width, height)));
            Children.Add(new QuadNode(RIGHT_BOTTOM, new Rect(Rectangle.x + width, Rectangle.y, width, height)));
            Children.Add(new QuadNode(RIGHT_TOP, new Rect(Rectangle.x + width, Rectangle.y + height, width, height)));

            foreach(var item in Children)
            {
                item.Parent = this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="excludePoints"></param>
        /// <param name="maxCount"></param>
        public void AddPoints(List<Vector3> points, out List<Vector3> excludePoints, int maxCount)
        {
            excludePoints = null;
            if (points == null || points.Count == 0)
            {
                return;
            }

            excludePoints = new List<Vector3>();
            foreach (var item in points)
            {
                if (!this.AddPoint(item, maxCount))
                {
                    excludePoints.Add(item);
                }
            }
        }

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public bool AddPoint(Vector3 point, int maxCount)
        {
            if (!this.IsInRange(point))
            {
                return false;
            }

            if (!IsSpilt)
            {
                this.Points.Add(point);
                if (this.Points.Count > maxCount)
                {
                    this.QuadSplit();

                    var points = new List<Vector3>();
                    points.AddRange(this.Points);
                    this.Points.Clear();
                    foreach (var item in points)
                    {
                        foreach (var child in Children)
                        {
                            child.AddPoint(item, maxCount);
                        }
                    }
                }
                return true;
            }
            else
            {
                foreach (var item in Children)
                {
                    if (item.IsInRange(point))
                    {
                        item.AddPoint(point, maxCount);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 是否在范围内
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInRange(Vector3 point)
        {
            return point.x >= Rectangle.x && point.x < (Rectangle.x + Rectangle.width)
                && point.y >= Rectangle.y && point.y < (Rectangle.y + Rectangle.height);
        }

        /// <summary>
        /// 是否包含点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public QuadNode FindPoint(Vector3 point)
        {
            if (!IsInRange(point))
            {
                return null;
            }

            Debug.LogFormat("Rect {0}, point {1}", Rectangle, point);

            if (IsSpilt)
            {
                foreach (var item in Children)
                {
                    var node = item.FindPoint(point);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            else
            {
                int index = Points.IndexOf(point);
                if (index != -1)
                {
                    return this;
                }
            }

            return null;
        }

        /// <summary>
        /// 查找距离目标最近的点
        /// </summary>
        /// <param name="inPoint"></param>
        /// <param name="outPoint"></param>
        /// <returns></returns>
        public bool FindClosestPoint(Vector3 inPoint, out Vector3 outPoint)
        {
            outPoint = Vector3.zero;
            if (!IsSpilt)
            {
                if (Points.Count == 0)
                {
                    return false;
                }

                List<Vector3> temp = new List<Vector3>();
                foreach(var item in Points)
                {
                    if (item != inPoint)
                    {
                        temp.Add(item);
                    }
                }
                if (temp.Count == 0)
                {
                    return false;
                }

                PointSet pointSet = new PointSet(temp);
                return pointSet.FindClosestPoint(inPoint, out outPoint);
            }
            else
            {
                List<Vector3> temp = new List<Vector3>();
                foreach (var item in Children)
                {
                    Vector3 tempPoint;
                    if (item.FindClosestPoint(inPoint, out tempPoint))
                    {
                        temp.Add(tempPoint);
                    }
                }

                if (temp.Count == 0)
                {
                    return false;
                }

                PointSet pointSet = new PointSet(temp);
                if (pointSet.FindClosestPoint(inPoint, out outPoint))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取相交的子区域
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public bool GetIntersectNodes(Rect rect, ref List<QuadNode> nodes)
        {
            if (!rect.Overlaps(this.Rectangle))
            {
                return false;
            }

            if (!IsSpilt)
            {
                if (Points.Count > 0)
                {
                    nodes.Add(this);
                }
            }
            else
            {
                foreach (var item in Children)
                {
                    item.GetIntersectNodes(rect, ref nodes);
                }
            }

            return nodes.Count > 0;
        }
    }
    /// <summary>
    /// 根节点
    /// </summary>
    public QuadNode Root { get; private set; }
    /// <summary>
    /// 叶子节点最大顶点数
    /// </summary>
    public int MaxPointCount { get; set; }

    public QuadTree()
    {
    }

    /// <summary>
    /// 设置矩形
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="size"></param>
    public void SetRectange(Vector3 origin, Vector3 size)
    {
        Root = new QuadNode(QuadNode.LEFT_BOTTOM, new Rect(origin, size));
    }

    /// <summary>
    /// 添加坐标点
    /// </summary>
    /// <param name="point"></param>
    public void AddPoint(Vector3 point)
    {
        if (Root == null)
        {
            return;
        }
        Root.AddPoint(point, MaxPointCount);
    }

    /// <summary>
    /// 设置坐标点集合
    /// </summary>
    /// <param name="points"></param>
    public void SetPoints(List<Vector3> points)
    {
        if (Root == null)
        {
            return;
        }
        if (points == null || points.Count == 0)
        {
            return;
        }

        List<Vector3> excludePoints = null;
        Root.AddPoints(points, out excludePoints, MaxPointCount);
    }

    /// <summary>
    /// 查找最近点
    /// </summary>
    /// <param name="inPoint"></param>
    /// <param name="outPoint"></param>
    /// <returns></returns>
    public bool FindClosestPoint(Vector3 inPoint, out Vector3 outPoint)
    {
        outPoint = Vector3.zero;

        if (Root == null)
        {
            return false;
        }

        // 找到目标所在的四叉树叶节点
        QuadNode node = Root.FindPoint(inPoint);
        if (node == null)
        {
            return false;
        }

        // 先找到距离目标最近的点
        Vector3 point;
        if (!node.FindClosestPoint(inPoint, out point))
        {
            if (node.Parent != null)
            {
                if (!node.Parent.FindClosestPoint(inPoint, out point))
                {
                    return false;
                }
            }
        }

        Debug.LogFormat("First Closest Point {0}", point);

        // 用寻找点作为中心点，边长为距离差的两倍的矩形，去检测所有相交的矩形
        float w = Mathf.Abs(point.x - inPoint.x);
        float h = Mathf.Abs(point.y - inPoint.y);

        // 以搜索点为圆心，距离为半径，做外接矩形
        // 以搜索点为矩形中心，距离差为长宽，做矩形
        float xx = inPoint.x - w;
        float yy = inPoint.y - h;
        float ww = 2 * w;
        float hh = 2 * h;

        // 搜索相交区域
        Rect rect = new Rect(xx, yy, ww, hh);
        Debug.LogFormat("Intersect {0}", rect);

        List<QuadNode> nodes = new List<QuadNode>();
        if (!Root.GetIntersectNodes(rect, ref nodes))
        {
            return false;
        }
        // 找到目标点
        List<Vector3> points = new List<Vector3>();
        foreach (var item in nodes)
        {
            points.AddRange(item.Points);
        }
        points.Remove(inPoint);
        // 寻找最近点
        PointSet pointSet = new PointSet(points);
        return pointSet.FindClosestPoint(inPoint, out outPoint);
    }
}
