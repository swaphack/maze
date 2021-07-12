using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Delaunay 三角化
/// 
/// 1、空圆特性：Delaunay三角网是唯一的（任意四点不能共圆），在Delaunay三角形网中任一三角形的外接圆范围内不会有其它点存在。
/// 2、最大化最小角特性：在散点集可能形成的三角剖分中，Delaunay三角剖分所形成的三角形的最小角最大。
/// 从这个意义上讲，Delaunay三角网是“最接近于规则化的“的三角网。
/// 具体的说是指在两个相邻的三角形构成凸四边形的对角线，在相互交换后，六个内角的最小角不再增大
/// 
/// 建立第一个三角形：
/// 选择第一个顶点，第一个顶点作为p1；
/// 选择第二个顶点，距离p1最近的顶点，记为p2；
/// 选择第三个顶点，和p1 p2构成的三角形的外接圆没有其他顶点，并且该三角形中点p3所在的三角形内角最大
/// 生成三个边，加入边表；
/// 生成第一个三角形，组建三角形表
/// 
/// 扩展三角形网格：
/// 从边表头选择一个边，标志位为假（表示目前仅仅存在于一个三角形中）；
/// 从点链表中搜索一个符合下述条件的点：
/// 在边所在三角形中第三个点的对侧；
/// 该点和该边构成的三角形的外接圆中没有其他点；
/// 满足上述条件的点在新三角形中的内角最大的点作为p3；
/// 如果边表中没有新生的边，将其加入边表尾，并设置标志位维false；如果已经存在，则设置其标志位为true；
/// 将生成的三角形加入三角形表；
/// 将从边表头选择的那条边标志位设为true；
/// 转至该过程的第一步，直至所有的边的标志位均为true；
/// 
/// </summary>
public class DelaunayTriangle
{
	/// <summary>
	/// 三角形点
	/// </summary>
	public class DTPoint
	{
		/// <summary>
		/// 索引
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		/// 坐标
		/// </summary>
		public Vector3 Position { get; set; }
		/// <summary>
		/// 相邻节点
		/// </summary>
		public HashSet<int> Neighbors { get; }

		public DTPoint(int index, Vector3 position)
		{
			this.Index = index;
			this.Position = position;
			this.Neighbors = new HashSet<int>();
		}
	}

	/// <summary>
	/// 边
	/// </summary>
	public class DTEdge
	{
		/// <summary>
		/// 索引
		/// </summary>
		public Tuple<int, int> Index { get; }
		/// <summary>
		/// 状态
		/// </summary>
		public bool State { get; set; }
		/// <summary>
		/// 所在三角形
		/// </summary>
		public List<int> Triangles { get; }

		public DTEdge(int p0, int p1)
		{
			Index = VertexHelper.GetKey(p0, p1);
			State = false;
			Triangles = new List<int>();
		}

		/// <summary>
		/// 添加三角形
		/// </summary>
		/// <param name="index"></param>
		public void AddTriangle(int index)
        {
			if (Triangles.Contains(index))
            {
				return;
            }

			Triangles.Add(index);
        }
	}

	/// <summary>
	/// 三角形
	/// </summary>
	public class DTTriangle
	{
		public int Index { get; }
		/// <summary>
		/// 顶点索引 从到大排序
		/// </summary>
		public Tuple<int, int, int> VertexPoint { get; private set; }
		/// <summary>
		/// 边
		/// </summary>
		public List<Tuple<int, int>> Edges { get; } = new List<Tuple<int, int>>();

		public DTTriangle(int index)
		{
			Index = index;
		}

		/// <summary>
		/// 获取第三个顶点索引
		/// </summary>
		/// <param name="p0"></param>
		/// <param name="p1"></param>
		/// <param name="check"></param>
		/// <returns></returns>
		public int GetThirdPointIndex(int p0, int p1, bool check = false)
        {
			int p2 = -1;
			if (!check)
            {
				p2 = VertexPoint.Item1 + VertexPoint.Item2 + VertexPoint.Item3 - p0 - p1;
			}
			else
            {
				int min = Mathf.Min(p0, p1);
				int max = Mathf.Max(p0, p1);

				if (VertexPoint.Item1 == min)
				{
					if (VertexPoint.Item2 == max) p2 = VertexPoint.Item3;
					else if (VertexPoint.Item3 == max) p2 = VertexPoint.Item2;
				}
				else if (VertexPoint.Item2 == min)
				{
					if (VertexPoint.Item3 == max) p2 = VertexPoint.Item1;
				}
			}

			return p2;
        }

		/// <summary>
		/// 设置顶点
		/// </summary>
		/// <param name="p0"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		public void SetVertexPoint(int p0, int p1, int p2)
        {
			VertexPoint = VertexHelper.GetKey(p0, p1, p2);
			Edges.Add(VertexHelper.GetKey(p0, p1));
			Edges.Add(VertexHelper.GetKey(p1, p2));
			Edges.Add(VertexHelper.GetKey(p2, p0));
		}
	}

	/// <summary>
	/// 划分点区域
	/// </summary>
	private PointRegion pointRegion = new PointRegion();

	/// <summary>
	/// 所有顶点
	/// </summary>
	private List<DTPoint> points = new List<DTPoint>();
	/// <summary>
	/// 边
	/// </summary>
	private Dictionary<Tuple<int, int>, DTEdge> pointEdges = new Dictionary<Tuple<int, int>, DTEdge>();

	/// <summary>
	/// 三角形索引
	/// </summary>
	private int triangleIndex = 0;
	/// <summary>
	/// 三角形 索引
	/// </summary>
	private Dictionary<int, DTTriangle> indexTriangles = new Dictionary<int, DTTriangle>();
	/// <summary>
	/// 三角形 顶点索引
	/// </summary>
	private Dictionary<Tuple<int, int, int>, DTTriangle> pointTriangles = new Dictionary<Tuple<int, int, int>, DTTriangle>();

	/// <summary>
	/// 三角形顶点索引
	/// </summary>
	public Tuple<int, int, int>[] Triangles
    {
		get
        {
			if (pointTriangles == null || pointTriangles.Count == 0)
            {
				return null;
            }

			Tuple<int, int, int>[] indices = new Tuple<int, int, int>[pointTriangles.Count];
			pointTriangles.Keys.CopyTo(indices, 0);
			return indices;
		}
    }


	public DelaunayTriangle()
    {
		pointRegion.HorizontalGridCount = 10;
		pointRegion.VerticalGridCount = 10;
		pointRegion.Width = 100;
		pointRegion.Height = 100;
	}

	/// <summary>
	/// 设置划分区域参数
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="horizontalCount"></param>
	/// <param name="verticalCount"></param>
	public void SetPointRegion(float width, float height, int horizontalCount, int verticalCount)
	{
		pointRegion.HorizontalGridCount = horizontalCount <= 0 ? 1 : horizontalCount;
		pointRegion.VerticalGridCount = verticalCount <= 0 ? 1 : verticalCount;
		pointRegion.Width = width <= 0 ? 1 : width;
		pointRegion.Height = height <= 0 ? 1 : height;
		pointRegion.GenerateGrids();
	}

	/// <summary>
	/// 设置点集合
	/// </summary>
	/// <param name="vertices"></param>
	public void SetPoints(List<Vector3> vertices)
	{
		if (vertices == null)
		{
			return;
		}
		points.Clear();
		for (int i = 0; i < vertices.Count; i++)
		{
			points.Add(new DTPoint(i, vertices[i]));
		}
		pointRegion.AddPoints(vertices);
	}

	/// <summary>
	/// 添加边
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="triangleIndex">三角形索引,-1 表示无所属三角形</param>
	/// <returns></returns>
	private DTEdge CreateEdge(int p0, int p1, int triangleIndex)
    {
		var key = VertexHelper.GetKey(p0, p1);

		DTEdge value;
		if (this.pointEdges.TryGetValue(key, out value))
        {
			if (triangleIndex >= 0)
            {
				value.AddTriangle(triangleIndex);
			}			
        }
		else
        {
			value = new DTEdge(p0, p1);
			if (triangleIndex >= 0)
			{
				value.AddTriangle(triangleIndex);
			}
			this.pointEdges.Add(key, value);
		}

		return value;
	}
	/// <summary>
	/// 是否包含边
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <returns></returns>
	private bool ContainEdge(int p0, int p1)
    {
		var key = VertexHelper.GetKey(p0, p1);
		return this.pointEdges.ContainsKey(key);
	}

	/// <summary>
	/// 查找边
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	private DTEdge FindEdge(Tuple<int, int> key)
    {
		DTEdge edge = null;
		if (this.pointEdges.TryGetValue(key, out edge))
        {
			return edge;
        }

		return null;
    }

	/// <summary>
	/// 添加三角形
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	private DTTriangle AddTriangle(int p0, int p1, int p2)
	{
		var key = VertexHelper.GetKey(p0, p1, p2);
		DTTriangle value;
		if (!this.pointTriangles.TryGetValue(key, out value))
        {
			int index = triangleIndex;
			value = new DTTriangle(index);
			value.SetVertexPoint(p0, p1, p2);
			this.pointTriangles.Add(key, value);
			this.indexTriangles.Add(index, value);
			triangleIndex++;
		}

		return value;
	}
	/// <summary>
	/// 移除三角形
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	private void RemoveTriangle(int p0, int p1, int p2)
    {
		var key = VertexHelper.GetKey(p0, p1, p2);
		this.RemoveTriangle(key);
	}

	/// <summary>
	/// 移除三角形
	/// </summary>
	/// <param name="key"></param>
	private void RemoveTriangle(Tuple<int, int, int> key)
	{
		DTTriangle value;
		if (this.pointTriangles.TryGetValue(key, out value))
		{
			this.pointTriangles.Remove(key);
			this.indexTriangles.Remove(value.Index);
		}
	}

	/// <summary>
	/// 是否包含三角形
	/// </summary>
	/// <param name="p0"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <returns></returns>
	private bool ContainTriangle(int p0, int p1, int p2)
    {
		var key = VertexHelper.GetKey(p0, p1, p2);
		return this.pointTriangles.ContainsKey(key);
	}

	/// <summary>
	/// 是否包含三角形
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	private bool ContainTriangle(int index)
	{
		return this.indexTriangles.ContainsKey(index);
	}

	private DTTriangle FindTriangle(Tuple<int, int, int> key)
    {
		DTTriangle triangle = null;
		if (this.pointTriangles.TryGetValue(key, out triangle))
        {
			return triangle;
        }

		return null;
    }

	/// 建立第一个三角形：
	/// 选择第一个顶点，第一个顶点作为p1；
	/// 选择第二个顶点，距离p1最近的顶点，记为p2；
	/// 选择第三个顶点，和p1 p2构成的三角形的外接圆没有其他顶点，并且该三角形中点p3所在的三角形内角最大
	/// 生成三个边，加入边表；
	/// 生成第一个三角形，组建三角形表
	private void CreateFirstTriangle()
    {
		if (points == null || points.Count < 3)
        {
			return;
        }

		var point1 = points[0];

		var p0 = point1.Index;
		var p1 = pointRegion.GetNearIndex(p0);
		if (p1 == -1) return;

		var p2 = pointRegion.GetCreateTriangleIndex(p0, p1);
		if (p2 == -1) return;

		var triangle = this.AddTriangle(p0, p1, p2);
		if (triangle != null)
        {
			this.CreateEdge(p0, p1, triangle.Index);
			this.CreateEdge(p1, p2, triangle.Index);
			this.CreateEdge(p2, p0, triangle.Index);
		}
	}

	/// <summary>
	/// 从边表头选择一个边，标志位为假（表示目前仅仅存在于一个三角形中）；
	/// 从点链表中搜索一个符合下述条件的点：
	///		在边所在三角形中第三个点的[对侧]；
	///		该点和该边构成的三角形的外接圆中没有其他点；
	///		满足上述条件的点在新三角形中的内角最大的点作为p3；
	///     如果边表中没有新生的边，将其加入边表尾，并设置标志位维false；如果已经存在，则设置其标志位为true；
	///     
	///		将生成的三角形加入三角形表；
	///		将从边表头选择的那条边标志位设为true；
	/// 转至该过程的第一步，直至所有的边的标志位均为true；
	/// </summary>
	private void CreateOtherTraingles()
    {
		if (pointEdges.Count < 3)
        {
			return;
        }

		List<Tuple<int, int>> waitSearchEdgeList = new List<Tuple<int, int>>();
		foreach (var item in pointEdges)
        {
			waitSearchEdgeList.Add(item.Value.Index);
        }
		List<Tuple<int, int>> searchedEdgeList = new List<Tuple<int, int>>();
		do
		{
			var front = waitSearchEdgeList[0];
			var p0 = front.Item1;
			var p1 = front.Item2;
			int otherPointIndex = -1;
			DTEdge edge = null;
			DTTriangle triangle = null;

			if (this.pointEdges.TryGetValue(front, out edge))
            {
				if (edge.Triangles.Count == 1)
                {
					if (this.indexTriangles.TryGetValue(edge.Triangles[0], out triangle))
                    {
						otherPointIndex = triangle.GetThirdPointIndex(p0, p1);
					}
                }
				else if (edge.Triangles.Count == 2)
                {
					waitSearchEdgeList.Remove(front);
					searchedEdgeList.Add(front);
					continue;
				}

            }

			var p2 = pointRegion.GetCreateTriangleIndexWithOtherSide(p0, p1, otherPointIndex);
			if (p2 != -1)
            {
				var line20 = VertexHelper.GetKey(p2, p0);
				var line21 = VertexHelper.GetKey(p2, p1);
				if (searchedEdgeList.Contains(line20) || searchedEdgeList.Contains(line21))
                {
					waitSearchEdgeList.Remove(front);
				}
				else
                {
					if (!waitSearchEdgeList.Contains(line20))
						waitSearchEdgeList.Add(line20);
					else
						searchedEdgeList.Add(line20);

					if (!waitSearchEdgeList.Contains(line21))
						waitSearchEdgeList.Add(line21);
					else
						searchedEdgeList.Add(line21);

					var newTriangle = this.AddTriangle(p0, p1, p2);
					if (newTriangle != null)
					{
						this.CreateEdge(p0, p1, newTriangle.Index);
						this.CreateEdge(p1, p2, newTriangle.Index);
						this.CreateEdge(p2, p0, newTriangle.Index);
					}

					waitSearchEdgeList.Remove(front);

					searchedEdgeList.Add(front);
				}
			}
			else
            {
				/*
				if (triangle != null)
                {
					foreach (var item in triangle.Edges)
                    {
						var oldEdge = this.FindEdge(item);
						if (oldEdge != null)
                        {
							oldEdge.Triangles.Clear();
						}

						searchedEdgeList.Remove(item);
						if (!waitSearchEdgeList.Contains(item))
                        {
							waitSearchEdgeList.Add(item);
						}
					}

					this.RemoveTriangle(triangle.VertexPoint);
				}
				*/
				waitSearchEdgeList.Remove(front);
			}
		} while (waitSearchEdgeList.Count > 0);
    }

	/// <summary>
	/// 创建三角形
	/// </summary>
	public void AutoCreateTriangles()
    {
		CreateFirstTriangle();
		CreateOtherTraingles();
    }

}
