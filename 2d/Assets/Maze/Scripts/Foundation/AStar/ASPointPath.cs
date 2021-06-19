using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点线路
/// </summary>
public class ASPointPath : AStarPath
{
	/// <summary>
	/// 关联点
	/// </summary>
	private Dictionary<Vector3, Dictionary<Vector3, float>> _NeighborPoints;
	/// <summary>
	/// 点
	/// </summary>
	private Dictionary<Vector3, ASPointNode> _Points;

	public ASPointPath ()
	{
		_NeighborPoints = new Dictionary<Vector3, Dictionary<Vector3, float>> ();
		_Points = new Dictionary<Vector3, ASPointNode> ();
	}

	/// <summary>
	/// 添加点
	/// </summary>
	/// <param name="position">Position.</param>
	public void AddPoint (Vector3 position)
	{
		if (!_Points.ContainsKey (position)) {
			_Points.Add (position, new ASPointNode ());
		}
	}

	/// <summary>
	/// 添加关联
	/// </summary>
	/// <param name="first">First.</param>
	/// <param name="second">Second.</param>
	/// <param name="distance">Distance.</param>
	public void AddLink (Vector3 first, Vector3 second, float distance)
	{
		if (!_NeighborPoints.ContainsKey (first)) {
			_NeighborPoints.Add (first, new Dictionary<Vector3, float> ());
		}

		if (!_NeighborPoints [first].ContainsKey (second)) {
			_NeighborPoints [first].Add (second, distance);
		} else {
			_NeighborPoints [first] [second] = distance;
		}
	}

	/// <summary>
	/// 获取点
	/// </summary>
	/// <returns>The node.</returns>
	/// <param name="vector">Vector.</param>
	public ASPointNode GetPoint (Vector3 vector)
	{
		if (!_Points.ContainsKey (vector)) {
			return null;
		}
		return _Points [vector];
	}

	/// <summary>
	/// 查找从起始点到终点的距离
	/// </summary>
	/// <returns>The way.</returns>
	/// <param name="src">Source.</param>
	/// <param name="dest">Destination.</param>
	public List<AStarNode> FindWay (Vector3 src, Vector3 dest)
	{
		ASPointNode startPoint = GetPoint (src);
		ASPointNode endPoint = GetPoint (dest);

		if (startPoint == null || endPoint == null) {
			return null;
		}

		return FindWay (startPoint, endPoint);
	}

	/// <summary>
	/// 初始化
	/// </summary>
	public override bool Init ()
	{
		float distance;
		foreach (KeyValuePair<Vector3, Dictionary<Vector3, float>> item in _NeighborPoints) {
			foreach (KeyValuePair<Vector3, float> item2 in item.Value) {
				ASPointNode startNode = GetPoint (item.Key);
				ASPointNode endNode = GetPoint (item2.Key);
				if (startNode != null && endNode != null) {
					if (item2.Value == 0) {
						distance = Vector3.Distance (startNode.Position, endNode.Position);
					} else {
						distance = item2.Value;
					}
					this.AddNeighborNode (startNode, endNode, distance);
				}
			}
		}

		return true;
	}

	/// <summary>
	/// 重置
	/// </summary>
	public override void Reset ()
	{
		foreach (KeyValuePair<Vector3, ASPointNode> item in _Points) {
			item.Value.Reset ();
		}
	}

	/// <summary>
	/// 清除
	/// </summary>
	public override void Dispose ()
	{
		base.Dispose ();
		_Points.Clear ();
		_NeighborPoints.Clear ();
	}
}

