using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子路线
/// 寻路地图(二维数组)
/// </summary>
public class ASGridPath : AStarPath
{
	/// <summary>
	/// 宽度
	/// </summary>
	private int _Width;
	/// <summary>
	/// 高度
	/// </summary>
	private int _Height;

	/// <summary>
	/// 格子
	/// </summary>
	private ASGridNode[,] _Grids;

	public ASGridPath ()
	{
	}

	/// <summary>
	/// 设置总面积
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public void SetSize (int width, int height)
	{
		if (width <= 0 || height <= 0) {
			return;
		}

		_Width = width;
		_Height = height;
	}

	/// <summary>
	/// 设置格子
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="item">Item.</param>
	public void SetGrid (Vector2 position, ASGridNode item)
	{
		if ((position.x < 0 || position.x >= _Width)
		     || (position.y < 0 || position.y >= _Height)) {
			return;
		}

		_Grids [(int)position.x, (int)position.y] = item;
	}

	/// <summary>
	/// 获取项
	/// </summary>
	/// <returns>The item.</returns>
	/// <param name="position">Position.</param>
	public ASGridNode GetGrid (Vector2 position)
	{
		if ((position.x < 0 || position.x >= _Width)
		     || (position.y < 0 || position.y >= _Height)) {
			return null;
		}

		if (_Grids == null) {
			return null;
		}

		int i = (int)position.x;
		int j = (int)position.y;

		return _Grids [i, j];
	}

	/// <summary>
	/// 查找一条从开始到结尾的路径
	/// </summary>
	/// <returns>The way.</returns>
	/// <param name="src">Source.</param>
	/// <param name="dest">Destination.</param>
	public List<AStarNode> FindWay (Vector2 src, Vector2 dest)
	{
		if (src == dest) {
			return null;
		}

		ASGridNode startNode = GetGrid (src);
		ASGridNode endNode = GetGrid (dest);

		if (startNode == null || endNode == null) {
			return null;
		}

		return FindWay (startNode, endNode);
	}


	/// <summary>
	/// 是否已加载
	/// </summary>
	/// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
	public bool Loaded {
		get { 
			return _Grids != null;
		}
	}

	/// <summary>
	/// 计算距离
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	protected override float CalDistance (AStarNode startNode, AStarNode endNode)
	{
		if (startNode == null || endNode == null) {
			return float.MaxValue;
		}

		ASGridNode node0 = startNode as ASGridNode;
		ASGridNode node1 = endNode as ASGridNode;

		if (node0 == null || node1 == null) {
			return float.MaxValue;
		}

		return CalDistance (node0.Position, node1.Position);
	}

	/// <summary>
	/// 获取距离
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	private static float CalDistance (Vector2 startNode, Vector2 endNode)
	{
		// StraightCost DiagCost
		return Vector2.Distance (startNode, endNode);
		//return GetDiagonalDistance (startNode, endNode, straightCost, diagCost);
	}

	/// <summary>
	/// 初始化
	/// </summary>
	public override bool Init ()
	{
		if (_Width <= 0 || _Height <= 0) {
			return false;
		}

		if (_Grids != null) {
			return true;
		}

		_Grids = new ASGridNode[_Width, _Height];

		int i, j, m, n, x, y;
		float distance;
		for (i = 0; i < _Height; i++) {
			for (j = 0; j < _Width; j++) {
				if (_Grids [i, j] == null) {
					_Grids [i, j] = new ASGridNode ();
					_Grids [i, j].Position.x = i;
					_Grids [i, j].Position.y = j;
				} else {
					_Grids [i, j].Reset ();
				}
			}
		}

		for (i = 0; i < _Height; i++) {
			for (j = 0; j < _Width; j++) {
				Vector2 position = _Grids [i, j].Position;
				for (m = -1; m <= 1; m++) {
					for (n = -1; n <= 1; n++) {
						// 如果是自己，则跳过
						if (m == 0 && n == 0)
							continue;
						x = (int)position.x + m;
						y = (int)position.y + n;
						// 判断是否越界，如果没有，加到列表中
						if (x < _Width && x >= 0 && y < _Height && y >= 0) {
							distance = CalDistance (_Grids [i, j].Position, _Grids [x, y].Position);
							this.AddNeighborNode (_Grids [i, j], _Grids [x, y], distance);
						}
					}
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
		int i, j;
		for (i = 0; i < _Height; i++) {
			for (j = 0; j < _Width; j++) {
				_Grids [i, j].Reset ();
			}
		}
	}

	/// <summary>
	/// 清除
	/// </summary>
	public override void Dispose ()
	{
		base.Dispose ();

		_Width = 0;
		_Height = 0;
		_Grids = null;
	}
}

