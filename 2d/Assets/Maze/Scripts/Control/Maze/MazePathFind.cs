using UnityEngine;
using System.Collections.Generic;
using Geometry;

public class MazeNode
{
	/// <summary>
	/// 相邻节点数
	/// </summary>
	public const int NeighborCount = 4;

	public Point point;
	/// <summary>
	/// 相邻节点，0-左，1-下，2-右，3-上
	/// </summary>
	public MazeNode[] neighbors;

	public MazeNode()
		:this(new Point())
	{
	}

	public MazeNode(Point position)
	{
		point = position;
		neighbors = new MazeNode[NeighborCount];
	}

	public void SetLeft(MazeNode node)
	{
		if (node == null) {
			return;
		}

		this.neighbors [0] = node;
		node.neighbors [2] = this;
	}

	public void SetRight(MazeNode node)
	{
		if (node == null) {
			return;
		}
		this.neighbors [2] = node;
		node.neighbors [0] = this;
	}

	public void SetUp(MazeNode node)
	{
		if (node == null) {
			return;
		}

		this.neighbors [3] = node;
		node.neighbors [1] = this;
	}

	public void SetDown(MazeNode node)
	{
		if (node == null) {
			return;
		}
		this.neighbors [1] = node;
		node.neighbors [3] = this;
	}

	/// <summary>
	/// 是否是临近节点
	/// </summary>
	/// <returns><c>true</c> if this instance is neighbor the specified node; otherwise, <c>false</c>.</returns>
	/// <param name="node">Node.</param>
	public bool IsNeighbor(MazeNode node)
	{
		if (node == null) {
			return false;
		}

		for (int i = 0; i < NeighborCount; i++) {
			if (neighbors [i].point == node.point) {
				return true;
			}
		}

		return false;
	}
}

/// <summary>
/// 寻路算法
/// </summary>
public abstract class MazePathFind
{
	/// <summary>
	/// 所有节点
	/// </summary>
	internal Dictionary<Point, MazeNode> _allNodes;
	/// <summary>
	/// 遍历路径
	/// </summary>
	private List<Point> _path;

	internal Dictionary<Point, MazeNode> AllNodes
	{
		get { 
			return _allNodes;
		}
	}

	internal List<Point> Path
	{
		get { 
			return _path;
		}
	}

	public MazePathFind()
	{
		_allNodes = new Dictionary<Point, MazeNode> ();
		_path = new List<Point> ();
	}

	protected virtual MazeNode CreateNode(Point point)
	{
		return new MazeNode (point);
	}


	public void Flush(Maze maze)
	{
		if (maze == null) {
			return;
		}

		this.Clear ();

		for (int i = 0; i < maze.height; i++) {
			for (int j = 0; j < maze.width; j++) {
				if (maze.map [i, j] == PathMark.Walkable) {
					Point point = new Point (j, i);
					MazeNode node = null;
					if (!_allNodes.ContainsKey (point)) {
						node = CreateNode (point);
						_allNodes.Add (point, node);
					} else {
						node = _allNodes [point];
					}

					// left
					if (j - 1 >= 0) {
						var left = new Point (j - 1, i);
						if (_allNodes.ContainsKey (left)) {
							var leftNode = _allNodes [left];
							node.SetLeft (leftNode);
						}
					}

					// up
					if (i - 1 >= 0) {
						var up = new Point (j, i -1);
						if (_allNodes.ContainsKey (up)) {
							var upNode = _allNodes [up];
							node.SetUp (upNode);
						}
					}
				}
			}
		}
	}

	public void SetFirst(Point position)
	{
		//_path.Add (position);
	}

	public void Clear()
	{
		_allNodes.Clear ();	
		_path.Clear ();
	}

	public bool IsPathEmpty()
	{
		return _allNodes.Count == 0;  
	}

	/// <summary>
	/// 获取下一个节点
	/// </summary>
	/// <returns>The next point.</returns>
	/// <param name="position">Position.</param>
	/// <param name="excludeLines">Exclude nodes.</param>
	public abstract Point GetNextPoint (Point position, List<Line> excludeLines = null);

	public static bool IntersectLine(List<Line> excludeLines, Line line)
	{
		if (excludeLines == null || excludeLines.Count == 0) {
			return false;
		}

		foreach (var item in excludeLines) {
			if (item.Contains (line.src) || item.Contains (line.dest)) {
				return true;
			}
			if (item.Intersect(line)) {
				return true;
			}
		}

		return false;
	}

	public static bool ContainPoint(List<Line> excludeLines, Point point)
	{
		if (excludeLines == null || excludeLines.Count == 0) {
			return false;
		}

		foreach (var item in excludeLines) {
			if (item.Contains (point)) {
				return true;
			}
		}

		return false;
	}
}

/// <summary>
/// 简单搜索
/// </summary>
public class SimplePathFinder : MazePathFind
{
	public override Point GetNextPoint (Point position, List<Line> excludeLines = null)
	{
		if (!AllNodes.ContainsKey (position)) {
			return position;
		}

		if (AllNodes.Count == 1) {
			return position;
		}

		var node = AllNodes [position];

		List<Point> lstNextPoints = new List<Point> ();
		for (var i = 0; i < MazeNode.NeighborCount; i++) {
			if (node.neighbors [i] != null) {
				var point = node.neighbors [i].point;
				if (!Path.Contains (point)) {
					lstNextPoints.Add (point);
				}
			}
		}

		if (lstNextPoints.Count == 0) {
			if (Path.Count == 0) {
				return position;
			} else {
				Path.Clear ();
				return GetNextPoint (position, excludeLines);
			}
		}

		if (!Path.Contains (position)) {
			Path.Add (position);
		}

		var nextPosition = lstNextPoints[Random.Range(0, lstNextPoints.Count)];

		Path.Add (nextPosition);

		return nextPosition;
	}
}

public class DeepMazeNode : MazeNode
{
	public bool visited = false;

	public DeepMazeNode()
		:this(new Point())
	{
	}

	public DeepMazeNode(Point position)
		:base(position)
	{
		visited = false;
	}
}

/// <summary>
/// 深度优先搜索
/// </summary>
public class DeepPathFinder : MazePathFind
{
	protected override MazeNode CreateNode(Point point)
	{
		return new DeepMazeNode (point);
	}

	public void Reset()
	{
		foreach (var item in AllNodes.Keys) {
			var node = AllNodes[item] as DeepMazeNode;
			node.visited = false;
		}
	}

	public override Point GetNextPoint (Point position, List<Line> excludeLines = null)
	{
		if (!AllNodes.ContainsKey (position)) {
			return position;
		}

		if (AllNodes.Count == 1) {
			return position;
		}

		var node = AllNodes [position];

		List<MazeNode> lstNextPoints = new List<MazeNode> ();
		for (var i = 0; i < MazeNode.NeighborCount; i++) {
			var neighbor = node.neighbors [i] as DeepMazeNode;
			if (neighbor != null) {
				if (!Path.Contains (neighbor.point) && !neighbor.visited) {
					lstNextPoints.Add (neighbor);
				}
			}
		}

		if (lstNextPoints.Count == 0) {
			if (Path.Count == 0) { // 重置
				Path.Clear ();
				this.Reset ();
				return GetNextPoint (position, excludeLines);
			} else { // 回溯
				var backPosition = Path [Path.Count - 1];

				var backNode = AllNodes [backPosition]  as DeepMazeNode;
				backNode.visited = true;
				Path.Remove (backPosition);
				return backPosition;
			}
		}

		// 正常流程
		if (!Path.Contains (position)) {
			Path.Add (position);
		}

		var nextPosition = lstNextPoints[Random.Range(0, lstNextPoints.Count)];

		Path.Add (nextPosition.point);

		return nextPosition.point;
	}
}

/// <summary>
/// 带碰撞的搜索
/// </summary>
public class CollidePathFinder : MazePathFind
{
	public override Point GetNextPoint (Point position, List<Line> excludeLines = null)
	{
		if (!AllNodes.ContainsKey (position)) {
			return position;
		}

		if (AllNodes.Count == 1) {
			return position;
		}

		var node = AllNodes [position];

		List<MazeNode> lstNextPoints = new List<MazeNode> ();
		for (var i = 0; i < MazeNode.NeighborCount; i++) {
			var neighbor = node.neighbors [i];
			if (neighbor != null) {
				if (!Path.Contains (neighbor.point)) {
					if (!ContainPoint (excludeLines, neighbor.point)) {
						lstNextPoints.Add (neighbor);
					}
				}
			}
		}

		if (lstNextPoints.Count == 0) {
			Path.Clear ();
			return position;
		}

		// 正常流程
		if (!Path.Contains (position)) {
			Path.Add (position);
		}

		var nextPosition = lstNextPoints[Random.Range(0, lstNextPoints.Count)];

		Path.Add (nextPosition.point);

		return nextPosition.point;
	}
}