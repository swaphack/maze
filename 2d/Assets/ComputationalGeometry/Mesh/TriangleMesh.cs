using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 三角格子生成算法
/// 算法一
/// 1.任取一个点，连接跟他相邻的所有点
/// 2.取与该点最近的一个点，执行第一个操作，如果有连线与之前的相交，取最短，删除相交的另一条线段
/// 3.重复执行第2步操作
/// </summary>
public class TriangleMesh
{
	public class MeshNode
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
		public HashSet<int> Neighbors = new HashSet<int>();

		public MeshNode(int index, Vector3 position)
        {
			this.Index = index;
			this.Position = position;
        }
	}
	
	private List<MeshNode> _meshNodes;

	/// <summary>
	/// 相邻点 {{索引，索引}, 距离}
	/// </summary>
	private Dictionary<Tuple<int, int>, float> _distances = new Dictionary<Tuple<int, int>, float>();

	public TriangleMesh()
	{
		_meshNodes = new List<MeshNode> ();
	}

	public TriangleMesh(List<Vector3> vertices)
	{
		if (vertices == null) {
			_meshNodes = new List<MeshNode>();
			return;
		}

		for (int i = 0; i < vertices.Count; i++)
        {
			_meshNodes.Add(new MeshNode(i, vertices[i]));
        }
	}

	/// <summary>
	/// 获取距离
	/// </summary>
	/// <param name="srcNode"></param>
	/// <param name="destNode"></param>
	/// <returns></returns>
	public float GetDistance(MeshNode srcNode, MeshNode destNode)
	{
		if (srcNode == null || destNode == null)
		{
			return float.MaxValue;
		}

		int minIndex = Mathf.Min(srcNode.Index, destNode.Index);
		int maxIndex = Mathf.Max(srcNode.Index, destNode.Index);

		Tuple<int, int> key = new Tuple<int, int>(minIndex, maxIndex);

		float value = float.MaxValue;
		if (_distances.TryGetValue(key, out value))
		{
			return value;
		}

		float distance = Vector3.Distance(srcNode.Position, destNode.Position);
		_distances.Add(key, value);

		return value;
	}

	/// <summary>
	/// 自动生成相邻节点
	/// </summary>
	public void AutoCreateNeightbor()
	{
		if(_meshNodes == null || _meshNodes.Count < 3)
        {
			return;
        }
	}
}

