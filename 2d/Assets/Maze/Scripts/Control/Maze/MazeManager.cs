using UnityEngine;
using System.Collections.Generic;
using Geometry;

public enum GameState
{
	/// <summary>
	/// 开始
	/// </summary>
	Start,
	/// <summary>
	/// 暂停
	/// </summary>
	Pause,
	/// <summary>
	/// 播放
	/// </summary>
	Play,
	/// <summary>
	/// 结束
	/// </summary>
	Finish
};



public class MazeManager
{
	public delegate void NewMaze (Maze maze);
	public delegate void RoleAction (MazeRoleBehaviour role);
	public delegate void GameListener(GameState state);

	public const string ROLE_NPC = "npc";
	public const  string ROLE_PLAYER = "player";
	public const  string ROLE_WALL = "wall";

	private MazeUI _ui;
	private MazeSmallMapUI _smallMap;
	private MazeMap _map;
	/// <summary>
	/// 角色
	/// </summary>
	private List<MazeRoleBehaviour> _roles;

	private Dictionary<int, Line> _rolePath;

	/// <summary>
	/// 迷宫
	/// </summary>
	private Maze _maze;
	/// <summary>
	/// 宽度
	/// </summary>
	private int _width = 8;
	/// <summary>
	/// 高度
	/// </summary>
	private int _height = 8;
	/// <summary>
	/// 游戏状态
	/// </summary>
	private GameState _state = GameState.Finish;
	/// <summary>
	/// 可以走的路
	/// </summary>
	private HashSet<Vector2> _walkablePath;
	/// <summary>
	/// 单例
	/// </summary>
	private static MazeManager _instance;

	/// <summary>
	/// 新的迷宫
	/// </summary>
	public event NewMaze NewMazeEvent;
	/// <summary>
	/// 新的角色
	/// </summary>
	public event RoleAction NewRoleEvent;
	/// <summary>
	/// 角色信息发生变更
	/// </summary>
	public event RoleAction RoleChangedEvent;
	/// <summary>
	/// 新的角色
	/// </summary>
	public event RoleAction DestoryRoleEvent;

	public event GameListener GameEvent;

	/// <summary>
	/// The loop count.
	/// </summary>
	public int loopCount = 1000;

	public static MazeManager Instance {
		get { 
			if (_instance == null) {
				_instance = new MazeManager ();
			}
			return _instance;
		}
	}

	public int Width {
		get { 
			return _width;
		}
	}

	public int Height {
		get {
			return _height;
		}
	}

	public Maze Maze {
		get { 
			return _maze;
		}
	}

	public MazeUI UI {
		get {
			return _ui;
		}
		set { 
			_ui = value;
		}
	}

	public MazeSmallMapUI SmallMap {
		get {
			return _smallMap;
		}
		set { 
			_smallMap = value;
		}
	}

	public MazeMap Map {
		get { 
			return _map;
		}
		set { 
			_map = value;
		}
	}

	public GameState State {
		get { 
			return _state;
		}
	}

	public Dictionary<int, Line>  RolePath {
		get { 
			return _rolePath;
		}
	}

	public MazeManager ()
	{
		_maze = new RecursiveDivisionMaze();
		_walkablePath = new HashSet<Vector2> ();
		_roles = new List<MazeRoleBehaviour> ();
		_rolePath = new Dictionary<int, Line> ();
	}

	public void Init()
	{
		//LoopListener.MaxLoopCount = Width * Height / 2;
		LoopListener.MaxLoopCount = Width * Height;

		_maze.GenerateMaze (Width, Height, new Vector2(1,1));
		_rolePath.Clear ();

		_walkablePath.Clear ();
		for (int i = 0; i < Height; i++) {
			for (int j = 0; j < Width; j++) {
				if (_maze.map [i, j] == PathMark.Walkable) {
					_walkablePath.Add (new Vector2 (j, i));
				}
			}
		}

		if (NewMazeEvent != null) {
			NewMazeEvent (_maze);
		}
	}

	public void Init(int width, int height)
	{
		_width = width;
		_height = height;

		Init ();
	}

	/// <summary>
	/// 广播新的角色
	/// </summary>
	/// <param name="role">Role.</param>
	public void BroadcastNewRole(MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}
		_roles.Add (role);

		if (NewRoleEvent != null) {
			NewRoleEvent (role);
		}
	}

	/// <summary>
	/// 角色信息发生改变
	/// </summary>
	/// <param name="role">Role.</param>
	public void BroadcastRoleChanged(MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}

		if (RoleChangedEvent != null) {
			RoleChangedEvent (role);
		}
	}

	/// <summary>
	/// 摧毁角色
	/// </summary>
	/// <param name="role">Role.</param>
	public void BroadcastDestoryRole(MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}
		_roles.Remove (role);
		_rolePath.Remove (role.mark);

		if (DestoryRoleEvent != null) {
			DestoryRoleEvent (role);
		}
	}

	/// <summary>
	/// 游戏事件
	/// </summary>
	/// <param name="state">State.</param>
	public void BroadcastGameState(GameState state)
	{
		_state = state;
		if (GameEvent != null) {
			GameEvent (state);
		}
	}
	/// <summary>
	/// 查找可行走的点
	/// </summary>
	/// <returns><c>true</c>, if walkable position was gotten, <c>false</c> otherwise.</returns>
	/// <param name="point">Point.</param>
	/// <param name="excludeNodes">Exclude nodes.</param>
	public bool GetWalkablePosition(ref Vector2 point, List<Vector2> excludeNodes = null)
	{
		point = new Vector2 (1, 1);
		if (_walkablePath.Count == 0) {
			return false;
		}

		List<Vector2> nodes = new List<Vector2> ();
		foreach (var item in _walkablePath) {
			nodes.Add (item);
		}

		if (excludeNodes != null) {
			foreach (var item in excludeNodes) {
				nodes.Remove (item);
			}
		}

		if (nodes.Count == 0) {
			return false;
		}

		point = nodes [Random.Range (0, nodes.Count)];

		return true;
	}

	/// <summary>
	/// 暂停游戏
	/// </summary>
	public void Pause()
	{
		if (Map != null) {
			Map.Pause ();
		}
	}

	/// <summary>
	///  开始游戏
	/// </summary>
	public void Resume()
	{
		if (Map != null) {
			Map.Resume ();
		}
	}

	/// <summary>
	/// 更新英雄路径
	/// </summary>
	/// <param name="role">Role.</param>
	/// <param name="src">Source.</param>
	/// <param name="dest">Destination.</param>
	public void UpdateRolePath(MazeRoleBehaviour role, Vector2 src, Vector2 dest)
	{
		if (role == null) {
			return;
		}

		if (!_rolePath.ContainsKey (role.mark)) {
			_rolePath.Add (role.mark, new Line (src, dest));
		} else {
			_rolePath [role.mark].src = src;
			_rolePath [role.mark].dest = dest;
		}

		Debug.LogFormat ("Update Position {0}, Cur Point {1}, Next Point {2}", role.mark, _rolePath[role.mark].src, _rolePath[role.mark].dest);
	}
}

