using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点区域
/// 01 11
/// 00 10
/// </summary>
public class PointRegion
{
    /// <summary>
    /// 顶点格子
    /// </summary>
    public class GridDetail
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 起始坐标
        /// </summary>
        public Vector3 Orgin { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public Vector3 Size { get; set; }
        /// <summary>
        /// 包含点
        /// </summary>
        public Dictionary<int, PointDetail> Points { get; }
        /// <summary>
        /// 相邻格子
        /// </summary>
        private List<int> neighborGrids = new List<int>();
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool Empty
        {
            get
            {
                return Points.Count == 0;
            }
        }

        public GridDetail(int index)
        {
            Index = index;
            Points = new Dictionary<int, PointDetail>();
        }

        /// <summary>
        /// 添加顶点
        /// </summary>
        /// <param name="vertex"></param>
        public bool AddPoint(PointDetail point)
        {
            if (point == null) return false;

            point.GridIndex = this.Index;
            Points.Add(point.Index, point);

            return true;
        }

        /// <summary>
        /// 获取点信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PointDetail GetPoint(int index)
        {
            PointDetail detail;
            if (Points.TryGetValue(index, out detail))
            {
                return detail;
            }

            return null;
        }

        /// <summary>
        /// 是否在范围内 半闭半开区间
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector3 point)
        {
            return Orgin.x <= point.x && (point.x < Orgin.x + Size.x)
                && Orgin.y <= point.y && (point.y < Orgin.y + Size.y);
        }

        /// <summary>
        /// 是否与圆相交
        /// </summary>
        /// <param name="center"></param>
        /// <param name="r"></param>
        /// <param name="outPoints"></param>
        /// <returns></returns>
        public bool GetIntersectPointsCircle(Vector3 center, float r, out List<int> outPoints)
        {
            outPoints = new List<int>();

            if (!IsIntersectCircle(center, r))
            {
                return false;
            }

            foreach (var item in Points)
            {
                if (Vector3.Distance(center, item.Value.Position) <= r)
                {
                    outPoints.Add(item.Key);
                }
            }

            return outPoints.Count > 0;
        }

        /// <summary>
        /// 与圆相交
        /// </summary>
        /// <param name="center"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool IsIntersectCircle(Vector3 center, float r)
        {
            if (Vector3.Distance(center, Orgin) <= r
                || Vector3.Distance(center, Orgin + Size) <= r
                || Vector3.Distance(center, Orgin + new Vector3(Size.x, 0, 0)) <= r
                || Vector3.Distance(center, Orgin + new Vector3(0, Size.y, 0)) <= r)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取临近格子
        /// </summary>
        /// <param name="horizontalCount"></param>
        /// <param name="verticalCount"></param>
        /// <returns></returns>
        public List<int> GetNeighborGrids(int horizontalCount, int verticalCount)
        {
            if (neighborGrids != null && neighborGrids.Count > 0)
            {
                return neighborGrids;
            }

            int i = Index / verticalCount;
            int j = Index % horizontalCount;

            for (int m = -1; m <= 1; m++)
            {
                for (int n = -1; n <= 1; n++)
                {
                    int h = i + m;
                    int w = j + n;
                    if (h >= 0 && h <= verticalCount
                        && w >= 0 && w <= horizontalCount
                        && !(m == 0 && n == 0))
                    {
                        int index = h * horizontalCount + w;
                        neighborGrids.Add(index);
                    }
                }
            }

            return neighborGrids;
        }
    }

    /// <summary>
    /// 点详情
    /// </summary>
    public class PointDetail
    {
        /// <summary>
        /// 顶点索引
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// 坐标
        /// </summary>
        public Vector3 Position { get; }
        /// <summary>
        /// 所属格子编号
        /// </summary>
        public int GridIndex { get; set; }

        public PointDetail(int index, Vector3 position)
        {
            Index = index;
            Position = position;
        }
    }

    /// <summary>
    /// 宽度
    /// </summary>
    public float Width { get; set; }
    /// <summary>
    /// 高度
    /// </summary>
    public float Height { get; set; }
    /// <summary>
    /// 水平方向格子数
    /// </summary>
    public int HorizontalGridCount
    {
        get
        {
            return horizontalGridCount;
        }
        set
        {
            horizontalGridCount = value;
            if (horizontalGridCount <= 0) horizontalGridCount = 1;
        }
    }
    /// <summary>
    /// 垂直方向格子数
    /// </summary>
    public int VerticalGridCount
    {
        get
        {
            return verticalGridCount;
        }
        set
        {
            verticalGridCount = value;
            if (verticalGridCount <= 0) verticalGridCount = 1;
        }
    }
    /// <summary>
    /// 水平方向格子数
    /// </summary>
    private int horizontalGridCount = 1;
    /// <summary>
    /// 垂直方向格子数
    /// </summary>
    private int verticalGridCount = 1;
    /// <summary>
    /// 格子信息
    /// </summary>
    private Dictionary<int, GridDetail> grids = new Dictionary<int, GridDetail>();
    /// <summary>
    /// 点信息
    /// </summary>
    private Dictionary<int, PointDetail> points = new Dictionary<int, PointDetail>();
    /// <summary>
	/// 相邻点 {{索引，索引}, 距离}
	/// </summary>
	private Dictionary<Tuple<int, int>, float> distances = new Dictionary<Tuple<int, int>, float>();
    /// <summary>
	/// 三角形角度 {{索引，索引}, 距离}
	/// </summary>
	private Dictionary<Tuple<int, int, int>, float> triangleAngles = new Dictionary<Tuple<int, int, int>, float>();
    /// <summary>
    /// 生成格子
    /// </summary>
    public void GenerateGrids()
    {
        if (Width <= 0 || Height <= 0)
        {
            return;
        }

        int hCount = HorizontalGridCount;
        int vCount = VerticalGridCount;

        points.Clear();
        grids.Clear();

        float width = Width / hCount;
        float height = Height / vCount;

        for (int i = 0; i < VerticalGridCount; i++)
        {
            for (int j = 0; j < HorizontalGridCount; j++)
            {
                int index = i * HorizontalGridCount + j;
                GridDetail grid = new GridDetail(index);
                grid.Orgin = new Vector3(j * width, i * height);
                grid.Size = new Vector3(width, height);
                grids.Add(index, grid);
            }
        }
    }
    /// <summary>
    /// 添加点
    /// </summary>
    /// <param name="points"></param>
    public void AddPoints(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
        {
            return;
        }

        for (int i = 0; i < points.Count; i++)
        {
            this.AddPoint(i, points[i]);
        }
    }

    /// <summary>
    /// 添加点
    /// </summary>
    /// <param name="points"></param>
    public void AddPoints(Vector3[] points)
    {
        if (points == null || points.Length == 0)
        {
            return;
        }

        for (int i = 0; i < points.Length; i++)
        {
            this.AddPoint(i, points[i]);
        }
    }

    /// <summary>
    /// 添加点
    /// </summary>
    /// <param name="index"></param>
    /// <param name="position"></param>
    public void AddPoint(int index, Vector3 position)
    {
        var pointDetail = new PointDetail(index, position);
        if (points.ContainsKey(index))
        {
            return;
        }
        points.Add(index, pointDetail);

        foreach (var item in grids)
        {
            if (item.Value.Contains(position))
            {
                item.Value.AddPoint(pointDetail);
                break;
            }
        }
    }

    /// <summary>
    /// 获取区域信息
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GridDetail GetGrid(int index)
    {
        GridDetail detail;
        if (grids.TryGetValue(index, out detail))
        {
            return detail;
        }

        return null;
    }

    public GridDetail GetGrid(int i, int j)
    {
        int hCount = HorizontalGridCount;
        int vCount = VerticalGridCount;
        int index = i * HorizontalGridCount + j;

        return GetGrid(index);
    }

    /// <summary>
    /// 获取点信息
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public PointDetail GetPoint(int index)
    {
        PointDetail detail;
        if (points.TryGetValue(index, out detail))
        {
            return detail;
        }

        return null;
    }

    /// <summary>
	/// 获取距离
	/// </summary>
	/// <param name="srcNode"></param>
	/// <param name="destNode"></param>
	/// <returns></returns>
	public float GetDistance(int srcIndex, int destIndex)
    {
        var srcNode = GetPoint(srcIndex);
        var destNode = GetPoint(destIndex);
        if (srcNode == null || destNode == null)
        {
            return -1;
        }

        if (distances == null)
        {
            return -1;
        }

        float value = 0;
        Tuple<int, int> key = VertexHelper.GetKey(srcIndex, destIndex);
        if (distances.TryGetValue(key, out value))
        {
            return value;
        }

        value = VertexHelper.GetDistance(srcNode.Position, destNode.Position);
        distances.Add(key, value);

        return value;
    }

    /// <summary>
    /// 获取角度
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <param name="thirdIndex"></param>
    /// <returns></returns>
    public float GetAngle(int srcIndex, int destIndex, int thirdIndex)
    {
        var srcNode = GetPoint(srcIndex);
        var destNode = GetPoint(destIndex);
        var thirdNode = GetPoint(thirdIndex);
        if (srcNode == null || destNode == null || thirdNode == null)
        {
            return -1;
        }

        if (triangleAngles == null)
        {
            return -1;
        }

        float value = 0;
        Tuple<int, int, int> key = VertexHelper.GetKey(srcIndex, destIndex, thirdIndex);
        if (triangleAngles.TryGetValue(key, out value))
        {
            return value;
        }

        value = VertexHelper.GetAngle(srcNode.Position, destNode.Position, thirdNode.Position);
        triangleAngles.Add(key, value);

        return value;
    }

    /// <summary>
    /// 第三点与排除点是否在版区间同一边
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <param name="thirdIndex"></param>
    /// <param name="otherIndex"></param>
    /// <returns></returns>
    public bool IsInSameSide(int srcIndex, int destIndex, int thirdIndex, int otherIndex)
    {
        if (otherIndex == -1)
        {
            return false;    
        }
        var srcNode = GetPoint(srcIndex);
        var destNode = GetPoint(destIndex);
        var thirdNode = GetPoint(thirdIndex);
        var otherNode = GetPoint(otherIndex);
        if (srcNode == null || destNode == null || thirdNode == null || otherNode == null)
        {
            return true;
        }

        var pointA = srcNode.Position;
        var pointB = destNode.Position;

        var pointC = thirdNode.Position;
        var pointD = otherNode.Position;

        return VertexHelper.IsInSameSide(pointA, pointB, pointC, pointD);
    }

    /// <summary>
    /// 获取圆的圆心和半径
    /// </summary>
    /// <param name="idx0"></param>
    /// <param name="idx1"></param>
    /// <param name="idx2"></param>
    /// <param name="center"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public void GetCircleCenterAndRadian(int idx0, int idx1, int idx2, out Vector3 center, out float r)
    {
        center = new Vector3();
        r = 0;

        var node0 = GetPoint(idx0);
        var node1 = GetPoint(idx1);
        var node2 = GetPoint(idx2);
        if (node0 == null || node1 == null || node2 == null)
        {
            return;
        }

        var pointA = node0.Position;
        var pointB = node1.Position;
        var pointC = node2.Position;
        center = VertexHelper.GetCentreOfGravity(pointA, pointB, pointC);
        r = Vector3.Distance(pointA, center);
    }
    /// <summary>
    /// 圆是否包含点
    /// </summary>
    /// <param name="center"></param>
    /// <param name="r"></param>
    /// <param name="otherIndex"></param>
    /// <returns></returns>
    public bool IsCircleContainPoint(Vector3 center, float r, int otherIndex)
    {
        var otherNode = GetPoint(otherIndex);
        if (otherNode == null)
        {
            return false;
        }

        return Vector3.Distance(center, otherNode.Position) <= r;
    }

    /// <summary>
    /// 是否包含格子中的点
    /// </summary>
    /// <param name="center"></param>
    /// <param name="r"></param>
    /// <param name="exp0"></param>
    /// <param name="exp1"></param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public bool IsCircleContainPointInGrids(Vector3 center, float r, int exp0, int exp1, int exp2)
    {
        float left = center.x - r;
        float right = center.x + r;
        float top = center.y + r;
        float bottom = center.y - r;

        // 遍历符合条件的格子
        foreach (var item in grids)
        {
            List<int> indices;
            if (item.Value.GetIntersectPointsCircle(center, r, out indices))
            {
                indices.Remove(exp0); indices.Remove(exp1); indices.Remove(exp2);
                if (indices.Count > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 获取距离最近的点
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="srcIndex"></param>
    /// <param name="regions"></param>
    /// <returns></returns>
    public int GetClosestPointIndex(GridDetail grid, int srcIndex)
    {
        if (grid == null || grid.Points == null || grid.Points.Count == 0)
        {
            return -1;
        }
        int destIndex = -1;
        float lastDistance = 0;
        foreach (var item in grid.Points)
        {
            if (item.Key != srcIndex)
            {
                float temp = GetDistance(srcIndex, item.Key);
                if (destIndex == -1 || temp < lastDistance)
                {
                    destIndex = item.Key;
                    lastDistance = temp;
                }
            }
        }

        return destIndex;
    }

    /// <summary>
    /// 获取构成三角形的点
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <returns></returns>
    public int GetTriangleClosestPointIndex(GridDetail grid, int srcIndex, int destIndex)
    {
        if (grid == null || grid.Points == null || grid.Points.Count == 0)
        {
            return -1;
        }
        int thirdIndex = -1;
        float lastAngle = 0;
        foreach (var item in grid.Points)
        {
            if (item.Key != srcIndex && item.Key != destIndex)
            {
                float temp = GetAngle(srcIndex, destIndex, item.Key);
                if (thirdIndex == -1 || temp > lastAngle)
                {
                    thirdIndex = item.Key;
                    lastAngle = temp;
                }
            }
        }

        return thirdIndex;
    }

    /// <summary>
    /// 临近八个方位的格子
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="searchGrids"></param>
    private void GetEightDirectionGrids(GridDetail grid, ref Dictionary<int, bool> searchGrids)
    {
        List<int> indices = grid.GetNeighborGrids(HorizontalGridCount, VerticalGridCount);
        if (indices != null && indices.Count != 0)
        {
            foreach (var item in indices)
            {
                if (!searchGrids.ContainsKey(item))
                {
                    searchGrids[item] = false;
                }
            }
        }
    }


    /// <summary>
    /// 获取构成三角形第三点索引，该索引在指定边的另一边
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <param name="otherSideIndex"></param>
    /// <returns></returns>
    public int GetTriangleClosestPointIndexWithOppositeIndex(GridDetail grid, int srcIndex, int destIndex, int otherSideIndex)
    {
        if (grid == null || grid.Points == null || grid.Points.Count == 0)
        {
            return -1;
        }
        int thirdIndex = -1;
        float lastAngle = 0;
        foreach (var item in grid.Points)
        {
            // 内角最大，并且没有包含其他顶点
            if (item.Key != srcIndex && item.Key != destIndex
                && !IsInSameSide(srcIndex, destIndex, item.Key, otherSideIndex))
            {
                Vector3 center;
                float r;
                GetCircleCenterAndRadian(srcIndex, destIndex, item.Key, out center, out r);
                if (!IsCircleContainPointInGrids(center, r, srcIndex, destIndex, item.Key))
                {
                    float temp = GetAngle(srcIndex, destIndex, item.Key);
                    if (thirdIndex == -1 || temp > lastAngle)
                    {
                        thirdIndex = item.Key;
                        lastAngle = temp;
                    }
                }
            }
        }
        return thirdIndex;
    }

    /// <summary>
    /// 查找相邻格子
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="searchGrids"></param>
    /// <param name="nearIndices"></param>
    private void SearchClosestPointInNeighborGrids(int srcIndex, ref Dictionary<int, bool> searchGrids, ref HashSet<int> nearIndices)
    {
        List<int> keys = new List<int>();
        foreach (var item in searchGrids)
        {
            bool hasSearched = searchGrids[item.Key];
            if (!hasSearched)
            {
                keys.Add(item.Key);
            }
        }
        if (keys.Count == 0)
        {
            return;
        }
        foreach (var key in keys)
        {
            searchGrids[key] = true;
            var grid = GetGrid(key);
            if (grid != null)
            {
                int destIndex = GetClosestPointIndex(grid, srcIndex);
                if (destIndex >= 0)
                {
                    nearIndices.Add(destIndex);
                    continue;
                }
                GetEightDirectionGrids(grid, ref searchGrids);
            }
        }
    }



    /// <summary>
    /// 获取周边三角形顶点
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <param name="searchGrids"></param>
    /// <param name="nearIndices"></param>
    private void GetNearTrianglePointInNeighborGrids(int srcIndex, int destIndex, ref Dictionary<int, bool> searchGrids, ref HashSet<int> nearIndices)
    {
        List<int> keys = new List<int>();
        foreach (var item in searchGrids)
        {
            bool hasSearched = searchGrids[item.Key];
            if (!hasSearched)
            {
                keys.Add(item.Key);
            }
        }
        if (keys.Count == 0)
        {
            return;
        }
        foreach (var key in keys)
        {
            searchGrids[key] = true;

            var grid = GetGrid(key);
            if (grid != null)
            {
                int thirdIndex = GetTriangleClosestPointIndex(grid, srcIndex, destIndex);
                if (thirdIndex >= 0)
                {
                    nearIndices.Add(thirdIndex);
                    continue;
                }
                GetEightDirectionGrids(grid, ref searchGrids);
            }
        }
    }


    /// <summary>
    /// 查找对立面的三角形第三方点
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <param name="otherSideIndex"></param>
    /// <param name="searchGrids"></param>
    /// <param name="nearIndices"></param>
    private void GetNearTrianglePointInNeighborGridsWithOppositeIndex(int srcIndex, int destIndex, int otherSideIndex, ref Dictionary<int, bool> searchGrids, ref HashSet<int> nearIndices)
    {
        List<int> keys = new List<int>();
        foreach (var item in searchGrids)
        {
            bool hasSearched = searchGrids[item.Key];
            if (!hasSearched)
            {
                keys.Add(item.Key);
            }
        }
        if (keys.Count == 0)
        {
            return;
        }

        foreach (var key in keys)
        {
            searchGrids[key] = true;
            var grid = GetGrid(key);
            if (grid != null)
            {
                int thirdIndex = GetTriangleClosestPointIndexWithOppositeIndex(grid, srcIndex, destIndex, otherSideIndex);
                if (thirdIndex >= 0)
                {
                    nearIndices.Add(thirdIndex);
                    continue;
                }
                GetEightDirectionGrids(grid, ref searchGrids);
            }
        }        
    }

    /// <summary>
    /// 获取距离最近的索引
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <returns></returns>
    public int GetNearIndex(int srcIndex)
    {
        var srcNode = GetPoint(srcIndex);
        if (srcNode == null)
        {
            return -1;
        }

        HashSet<int> nearIndices = new HashSet<int>();
        Dictionary<int, bool> searchGrids = new Dictionary<int, bool>();
        searchGrids[srcNode.GridIndex] = false;

        int i = srcNode.GridIndex / HorizontalGridCount;
        int j = srcNode.GridIndex % HorizontalGridCount;

        // 包含所在格子的搜索
        int vSearchCount = Mathf.Max(i + 1, VerticalGridCount - i);
        int hSearchCount = Mathf.Max(j + 1, HorizontalGridCount - j);
        int searchCount = Mathf.Max(vSearchCount, hSearchCount);

        for (int k = 0; k < searchCount; k++)
        {
            SearchClosestPointInNeighborGrids(srcIndex, ref searchGrids, ref nearIndices);
            if (nearIndices.Count > 0)
            {// 存在临近点
                break;
            }
        }

        if (nearIndices.Count == 0)
        {
            return -1;
        }

        int nearIndex = -1;
        float lastDistance = 0;
        foreach (var item in nearIndices)
        {
            var distance = GetDistance(srcIndex, item);
            if (nearIndex == -1 || (distance > 0 && distance < lastDistance))
            {
                nearIndex = item;
                lastDistance = distance;
            }
        }

        return nearIndex;
    }

    /// <summary>
    /// 获取构成三角行最近的第三点索引
    /// 和p1 p2构成的三角形的外接圆没有其他顶点，并且该三角形中点p3所在的三角形内角最大
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <returns></returns>
    public int GetCreateTriangleIndex(int srcIndex, int destIndex)
    {
        var srcNode = GetPoint(srcIndex);
        var destNode = GetPoint(destIndex);
        if (srcNode == null || destNode == null)
        {
            return -1;
        }

        HashSet<int> nearIndices = new HashSet<int>();
        Dictionary<int, bool> searchGrids = new Dictionary<int, bool>();
        searchGrids[srcNode.GridIndex] = false;

        int i = srcNode.GridIndex / HorizontalGridCount;
        int j = srcNode.GridIndex % HorizontalGridCount;

        // 包含所在格子的搜索
        int vSearchCount = Mathf.Max(i + 1, VerticalGridCount - i);
        int hSearchCount = Mathf.Max(j + 1, HorizontalGridCount - j);
        int searchCount = Mathf.Max(vSearchCount, hSearchCount);

        for (int k = 0; k < searchCount; k++)
        {
            GetNearTrianglePointInNeighborGrids(srcIndex, destIndex, ref searchGrids, ref nearIndices);
            if (nearIndices.Count > 0)
            {// 存在临近点
                break;
            }
        }

        if (nearIndices.Count == 0)
        {
            return -1;
        }

        int nearIndex = -1;
        float lastDistance = 0;
        foreach (var item in nearIndices)
        {
            var distance = GetAngle(srcIndex, destIndex, item);
            if (nearIndex == -1 || (distance > 0 && distance < lastDistance))
            {
                nearIndex = item;
                lastDistance = distance;
            }
        }

        return nearIndex;
    }

    /// <summary>
    /// 获取对立面的顶点
    /// </summary>
    /// <param name="srcIndex"></param>
    /// <param name="destIndex"></param>
    /// <param name="otherIndex"></param>
    /// <returns></returns>
    public int GetCreateTriangleIndexWithOtherSide(int srcIndex, int destIndex, int otherIndex)
    {
        var srcNode = GetPoint(srcIndex);
        var destNode = GetPoint(destIndex);
        if (srcNode == null || destNode == null)
        {
            return -1;
        }

        HashSet<int> nearIndices = new HashSet<int>();
        Dictionary<int, bool> searchGrids = new Dictionary<int, bool>();
        searchGrids[srcNode.GridIndex] = false;

        int i = srcNode.GridIndex / HorizontalGridCount;
        int j = srcNode.GridIndex % HorizontalGridCount;

        // 包含所在格子的搜索
        int vSearchCount = Mathf.Max(i + 1, VerticalGridCount - i);
        int hSearchCount = Mathf.Max(j + 1, HorizontalGridCount - j);
        int searchCount = Mathf.Max(vSearchCount, hSearchCount);

        for (int k = 0; k < searchCount; k++)
        {
            GetNearTrianglePointInNeighborGridsWithOppositeIndex(srcIndex, destIndex, otherIndex, ref searchGrids, ref nearIndices);
            if (nearIndices.Count > 0)
            {// 存在临近点
                break;
            }
        }

        if (nearIndices.Count == 0)
        {
            return -1;
        }

        int nearIndex = -1;
        float lastDistance = 0;
        foreach (var item in nearIndices)
        {
            var distance = GetAngle(srcIndex, destIndex, item);
            if (nearIndex == -1 || (distance > 0 && distance < lastDistance))
            {
                nearIndex = item;
                lastDistance = distance;
            }
        }

        return nearIndex;
    }
    /// <summary>
    /// 随机获取一点
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <returns></returns>
    public int GetRandomPoint(int p0, int p1)
    {
        if (points == null || points.Count < 3)
        {
            return -1;
        }

        int[] keys = new int[points.Keys.Count];
        points.Keys.CopyTo(keys, 0);
        List<int> temp = new List<int>(keys);
        temp.Remove(p0);
        temp.Remove(p1);

        int random = UnityEngine.Random.Range(0, temp.Count);
        return temp[random];
    }
}
