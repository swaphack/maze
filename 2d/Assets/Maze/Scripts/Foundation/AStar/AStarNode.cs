using System;
using UnityEngine;


/// <summary>
/// A*节点
/// </summary>
public class AStarNode
{
	/// <summary>
	/// 能否通过
	/// </summary>
	public bool CanPass;
	/// <summary>
	/// 与起始位置的距离
	/// </summary>
	public float SrcDistance;
	/// <summary>
	/// 与目标位置的距离
	/// </summary>
	public float DestDistance;

	/// <summary>
	/// 上一个节点
	/// </summary>
	public AStarNode Previous;

	/// <summary>
	/// 总距离
	/// </summary>
	/// <value>The total distance.</value>
	public float TotalDistance {
		get { 
			return SrcDistance + DestDistance;
		}	
	}

	public AStarNode ()
	{
		this.Reset ();
	}

	/// <summary>
	/// 重置
	/// </summary>
	public void Reset ()
	{
		CanPass = true;
		SrcDistance = 0;
		DestDistance = 0;
		Previous = null;
	}
}


