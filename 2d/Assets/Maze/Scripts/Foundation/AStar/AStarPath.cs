using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A*静态寻路
/// </summary>
public class AStarPath
{
	/// <summary>
	/// 直线权值
	/// </summary>
	public const int StraightCost = 10;
	/// <summary>
	/// 斜线权值
	/// </summary>
	public const int DiagCost = 14;
	/// <summary>
	/// 相邻节点 { 节点， 相邻节点信息 { 节点， 距离}}
	/// </summary>
	private Dictionary<AStarNode, Dictionary<AStarNode, float>> _NeighborNodes;

	// 寻路待考虑的点
	private List<AStarNode> _OpenList;
	// 不在寻路考虑范围的点
	private HashSet<AStarNode> _ClosedList;

	/// <summary>
	/// 相邻节点
	/// </summary>
	protected Dictionary<AStarNode, Dictionary<AStarNode, float>> NeighborNodes {
		get { 
			return _NeighborNodes;
		}
	}

	public AStarPath ()
	{
		_NeighborNodes = new Dictionary<AStarNode, Dictionary<AStarNode, float>> ();

		_OpenList = new List<AStarNode> ();
		_ClosedList = new HashSet<AStarNode> ();
	}

	/// <summary>
	/// 曼哈顿估价法
	/// </summary>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	/// <param name="straightCost">Straight cost.</param>
	public static float GetManhattanDistance (Vector2 startNode, Vector2 endNode, float straightCost)
	{
		return (Math.Abs(startNode.x - endNode.x) + Math.Abs(startNode.y - endNode.y)) * straightCost;
	}

	/// <summary>
	/// 几何估价法
	/// </summary>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	/// <param name="straightCost">Straight cost.</param>
	public static float GetEuclidianDistance (Vector2 startNode, Vector2 endNode, float straightCost)
	{
		float dx = startNode.x - endNode.x;
		float dy = startNode.y - endNode.y;
		return (float)Math.Sqrt (dx * dx + dy * dy) * straightCost;
	}

	/// <summary>
	/// 对角线估价法
	/// </summary>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	/// <param name="straightCost">Straight cost.</param>
	/// <param name="diagCost">Diag cost.</param>
	public static float GetDiagonalDistance (Vector2 startNode, Vector2 endNode, float straightCost, float diagCost)
	{
		float dx = Math.Abs (startNode.x - endNode.x);
		float dy = Math.Abs (startNode.y - endNode.y);
		float diag = Math.Min (dx, dy);
		float straight = dx + dy;
		return diagCost * diag + straightCost * (straight - 2 * diag);
	}

	/// <summary>
	/// 生成路径
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	public static List<AStarNode> GeneratePath (AStarNode startNode, AStarNode endNode)
	{
		if (startNode == null || endNode == null) {
			return null;
		}

		List<AStarNode> path = new List<AStarNode> ();
		AStarNode temp = endNode;
		while (temp != startNode) {
			path.Add (temp);
			temp = temp.Previous;
		}
		path.Reverse ();

		return path;
	}


	/// <summary>
	/// 添加相邻节点
	/// </summary>
	/// <param name="first">第一节点</param>
	/// <param name="second">第二节点</param>
	/// <param name="distance">距离</param>
	public void AddNeighborNode (AStarNode first, AStarNode second, float distance)
	{
		if (first == null || second == null) {
			return;
		}

		if (!_NeighborNodes.ContainsKey (first)) {
			_NeighborNodes.Add (first, new Dictionary<AStarNode, float> ());
		}

		if (!_NeighborNodes.ContainsKey (second)) {
			_NeighborNodes.Add (second, new Dictionary<AStarNode, float> ());
		}

		if (!_NeighborNodes [first].ContainsKey (second)) {
			_NeighborNodes [first].Add (second, distance);
		} else {
			_NeighborNodes [first] [second] = distance;
		}

		if (!_NeighborNodes [second].ContainsKey (first)) {
			_NeighborNodes [second].Add (first, distance);
		} else {
			_NeighborNodes [second] [first] = distance;
		}
	}

	/// <summary>
	/// 移除相邻节点
	/// </summary>
	/// <param name="first">First.</param>
	/// <param name="second">Second.</param>
	public void RemoveNeighborNode (AStarNode first, AStarNode second)
	{
		if (first == null || second == null) {
			return;
		}

		if (_NeighborNodes.ContainsKey (first)) {
			_NeighborNodes [first].Remove (second);
		}

		if (!_NeighborNodes.ContainsKey (second)) {
			_NeighborNodes [second].Remove (first);
		}
	}

	/// <summary>
	/// 获取相邻节点
	/// </summary>
	/// <returns>The neighbor nodes.</returns>
	/// <param name="node">Node.</param>
	protected List<AStarNode> GetNeighborNodes (AStarNode node)
	{
		if (node == null) {
			return null;
		}
				
		if (!NeighborNodes.ContainsKey (node)) {
			return null;
		}

		List<AStarNode> nodes = new List<AStarNode> ();
		foreach (KeyValuePair<AStarNode, float> item in NeighborNodes[node]) {
			nodes.Add (item.Key);
		}

		return nodes;
	}

	/// <summary>
	/// 获取两节点间距离
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	protected float GetDistance (AStarNode startNode, AStarNode endNode)
	{
		if (startNode == null || endNode == null) {
			return float.MaxValue;
		}

		if (NeighborNodes.ContainsKey (startNode)) {
			if (NeighborNodes [startNode].ContainsKey (endNode)) {
				return NeighborNodes [startNode] [endNode];
			}
		}

		if (NeighborNodes.ContainsKey (endNode)) {
			if (NeighborNodes [endNode].ContainsKey (startNode)) {
				return NeighborNodes [endNode] [startNode];
			}
		}

		return float.MaxValue;
	}

	/// <summary>
	/// 计算距离
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	protected virtual float CalDistance (AStarNode startNode, AStarNode endNode)
	{
		return float.MaxValue;
	}


	/// <summary>
	/// 查找从起点到终点的路
	/// </summary>
	/// <returns>The way.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	public List<AStarNode> FindWay (AStarNode startNode, AStarNode endNode)
	{
		if (startNode == null || endNode == null) {
			return null;
		}

		_OpenList.Clear ();
		_ClosedList.Clear ();


		AStarNode currentNode;
		_OpenList.Add (startNode);

		int count = 0;
		int i = 0;
		AStarNode item = null;
		//Log.Warning ("Begin Find Way From :(" + src.x + "," + src.y + "),To :(" +dest.x + "," + dest.y + ")");
		while (_OpenList.Count > 0) {
			currentNode = _OpenList [0];

			// 查找最小权值
			count = _OpenList.Count;
			for (i = 0; i < count; i++) {
				if (_OpenList [i].TotalDistance <= currentNode.TotalDistance
				     && _OpenList [i].DestDistance < currentNode.DestDistance) {
					currentNode = _OpenList [i];
				}
			}

			_OpenList.Remove (currentNode);
			_ClosedList.Add (currentNode);

			// 找到目标，生成路径
			if (currentNode == endNode) {
				return GeneratePath (startNode, endNode);
			}

			// 计算周围到目标的路径
			List<AStarNode> neighborItems = GetNeighborNodes (currentNode);
			if (neighborItems == null) {
				return null;
			}
			count = neighborItems.Count;
			for (i = 0; i < count; i++) {
				item = neighborItems [i];
				// 不可通过、已查找过
				if (item == null || !item.CanPass || _ClosedList.Contains (item)) {
					continue;
				}

				float newCost = currentNode.SrcDistance + GetDistance (currentNode, item);

				// 距离小，或者未在考虑表中
				if (newCost < item.SrcDistance || !_OpenList.Contains (item)) {
					item.SrcDistance = newCost;
					item.DestDistance = CalDistance (item, endNode);
					item.Previous = currentNode;

					if (!_OpenList.Contains (item)) {
						_OpenList.Add (item);
					} 

					//Log.Warning ("Node : (" + currentNode.Position.x + "," + currentNode.Position.y + ")" + "Src : " + item.SrcDistance + ", Dest :" + item.DestDistance);
				}
			}
		}

		return null;
	}

	/// <summary>
	/// 初始化
	/// </summary>
	public virtual bool Init ()
	{
		return true;
	}

	/// <summary>
	/// 重置
	/// </summary>
	public virtual void Reset ()
	{
	}

	public virtual void Dispose ()
	{
		NeighborNodes.Clear ();
	}
}


