using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 迷宫地图模型
/// </summary>
public class MazeMap : MonoBehaviour 
{
	private const float itemWidth= 1.0f;
	private const float itemHeight = 1.0f;

	private bool _play = false;

	public bool Playing {
		get { 
			return _play;
		}
	}

	// Use this for initialization
	void Start () 
	{
		MazeManager.Instance.Map = this;

		MazeManager.Instance.NewMazeEvent += PaintMaze;

		MazeManager.Instance.DestoryRoleEvent += DestoryRole;

		MazeManager.Instance.GameEvent += OnGameStateChange;
	}

	void Update()
	{
		if (!_play) {
			return;
		}
		for (int i = 0; i < this.transform.childCount; i++) {  
			var item = transform.GetChild (i).GetComponent<MazeRoleBehaviour>();
			if (item != null) {
				item.UpdateSelf ();
			}
		}  
	}

	void OnDestroy()
	{
		MazeManager.Instance.NewMazeEvent -= PaintMaze;
		MazeManager.Instance.DestoryRoleEvent -= DestoryRole;

		MazeManager.Instance.GameEvent -= OnGameStateChange;
	}

	void OnGameStateChange(GameState state)
	{
		switch (state) {
		case GameState.Start: {
				Camera.main.transform.SetParent (null);
				Pause ();
				ClearMaze ();
			}
			break;
		case GameState.Pause:
			Pause ();
			break;
		case GameState.Play:
			Resume ();
			break;
		case GameState.Finish:
			Pause ();
			break;
		}
	}

	public void Pause()
	{
		_play = false;

		var children = this.GetComponentsInChildren<AudioSource> ();
		foreach (var item in children) {
			if (item.isPlaying) {
				item.Pause ();
			}
		}
	}

	public void Resume()
	{
		_play = true;

		var children = this.GetComponentsInChildren<AudioSource> ();
		foreach (var item in children) {
			if (!item.isPlaying) {
				item.Play ();
			}
		}
	}

	public void ClearMaze()
	{
		Camera.main.transform.SetParent (null);

		for (int i = 0; i < this.transform.childCount; i++) {  
			Destroy (transform.GetChild (i).gameObject);  
		}  
	}

	void PaintWall(float x, float y, float w, float h)
	{
		var go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.position = new Vector3 (x, 0.5f, y);
		go.name = MazeManager.ROLE_WALL;
		go.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture> ("Textures/wall_2");
		go.transform.SetParent (this.transform);
	}

	void PaintMaze (Maze maze)
	{
		int wNum = maze.width;
		int hNum = maze.height;

		Terrain terrain = this.GetComponent<Terrain> ();
		terrain.terrainData.size = new Vector3 (wNum, 10, hNum);

		for (int j = 0; j < hNum; j++) {
			for (int i = 0; i < wNum; i++) {
				bool bVisible = maze.map [j, i]  == PathMark.NotWalkable;
				if (bVisible) {
					float x = (2 * i + 1) * itemWidth * 0.5f;
					float y = (2 * j + 1) * itemHeight * 0.5f;
					PaintWall (x, y, itemWidth, itemHeight);
				}
			}
		}
	}

	void DestoryRole(MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}

		if (role.name == MazeManager.ROLE_NPC) {
			role.transform.SetParent (null);
		} else if (role.name == MazeManager.ROLE_PLAYER) {
			role.transform.SetParent (null);
		}
	}

	public void AddNPC()
	{
		var children = MazeManager.Instance.Map.GetComponentsInChildren<MazeRoleBehaviour> ();
		List<Vector2> excludeNodes = new List<Vector2> ();
		foreach (var item in children) {
			if (item != this) {
				excludeNodes.Add (item.Point2d);
			}
		}

		Vector2 point = new Vector2 ();
		if (!MazeManager.Instance.GetWalkablePosition (ref point, excludeNodes)) {
			return;
		}

		var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		var collider = go.GetComponent<SphereCollider> ();
		collider.isTrigger = true;

		var role = go.AddComponent<MazeNPC> ();
		role.Orignal2d = point;

		var audioSrc = go.AddComponent<AudioSource> ();
		audioSrc.clip = Resources.Load<AudioClip> ("Audio/Effect/robot_walk");
		audioSrc.loop = true;
		audioSrc.spatialBlend = 1.0f;
		audioSrc.minDistance = 0.5f;
		audioSrc.maxDistance = 1.0f;
		audioSrc.playOnAwake = false;

		go.transform.position = new Vector3 (
			role.Orignal2d.x + MazeRoleBehaviour.Offset3d.x, 
			MazeRoleBehaviour.Offset3d.y, 
			role.Orignal2d.y + MazeRoleBehaviour.Offset3d.z);
		go.transform.SetParent (this.transform);
	}

	public void AddRole()
	{
		var children = MazeManager.Instance.Map.GetComponentsInChildren<MazeRoleBehaviour> ();
		List<Vector2> excludeNodes = new List<Vector2> ();
		foreach (var item in children) {
			if (item != this) {
				excludeNodes.Add (item.Point2d);
			}
		}

		Vector2 point = new Vector2 ();
		if (!MazeManager.Instance.GetWalkablePosition (ref point, excludeNodes)) {
			return;
		}

		var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		var body = go.AddComponent<Rigidbody> ();
		body.constraints = RigidbodyConstraints.FreezeAll;
		body.constraints -= RigidbodyConstraints.FreezePositionX;
		body.constraints -= RigidbodyConstraints.FreezePositionZ;
		body.useGravity = false;
		body.drag = 10000;

		var role = go.AddComponent<MazePlayer> ();
		role.Orignal2d = point;
		role.moveSpeed = Random.Range (1.2f, 1.8f);

		go.transform.position = new Vector3 (
			role.Orignal2d.x + MazeRoleBehaviour.Offset3d.x, 
			MazeRoleBehaviour.Offset3d.y, 
			role.Orignal2d.y + MazeRoleBehaviour.Offset3d.z);
		go.transform.SetParent (this.transform);

		var audioSrc = go.AddComponent<AudioSource> ();
		audioSrc.clip = Resources.Load<AudioClip> ("Audio/Effect/role_run");
		audioSrc.playOnAwake = false;

		Camera.main.transform.SetParent (go.transform);
		Camera.main.transform.localPosition = new Vector3 (0, 0.15f, 0f);
		Camera.main.transform.localRotation = Quaternion.Euler(new Vector3 (0, 0, 0));
	}
}
