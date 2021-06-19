using UnityEngine;
using System.Collections;

public class MazeRoleBehaviour : MonoBehaviour
{
	/// <summary>
	/// 标记
	/// </summary>
	internal static int sMark = 0;

	/// <summary>
	/// 2d地图坐标偏移
	/// </summary>
	public static Vector3 Offset3d = new Vector3 (0.5f, 0.5f, 0.5f);
	/// <summary>
	/// 初始位置
	/// </summary>
	public Vector2 Orignal2d = new Vector2();
	/// <summary>
	/// 当前2d位置
	/// </summary>
	/// <value>The point2d.</value>
	public Vector2 Point2d = new Vector2();
	/// <summary>
	/// 标记
	/// </summary>
	public int mark;
	/// <summary>
	/// 角色图片
	/// </summary>
	public Sprite roleSprite;
	/// <summary>
	/// 初始位置
	/// </summary>
	internal Vector3 orginalPosition;
	/// <summary>
	/// 移动速度
	/// </summary>
	[Range(0.1f, 10)]
	public float moveSpeed = 1;
	/// <summary>
	/// 旋转速度
	/// </summary>
	[Range(0.1f, 10)]
	public float rotateSpeed = 1;

	private bool _play = false;

	public bool Playing {
		get { 
			return _play;
		}
	}

	public MazeRoleBehaviour()
	{
		mark = ++sMark;
	}

	// Use this for initialization
	void Start ()
	{
		InitRole ();
	}

	void OnDestroy()
	{
		DestroyRole();
	}
	
	public virtual void UpdateSelf()
	{
		UpdateRole ();
	}

	protected virtual void InitRole()
	{

	}

	protected virtual void UpdateRole()
	{
		if (!_play) {
			return;
		}
	}

	protected virtual void DestroyRole()
	{

	}

	public void Pause()
	{
		_play = false;
	}

	public void Resume()
	{
		_play = true;
	}
}

