using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;

/// <summary>
/// 迷宫npc模型
/// </summary>
public class MazeNPC : MazeRoleBehaviour
{
	/// <summary>
	/// 智能ai
	/// </summary>
	private CollidePathFinder ai;
	/// <summary>
	/// 下一个位置
	/// </summary>
	private Vector3 nextPosition;
	/// <summary>
	/// 下一个旋转角度
	/// </summary>
	private Quaternion nextRotation;

	/// <summary>
	/// 异常处理
	/// </summary>
	private int errorCount;

	public MazeNPC()
	{
		//ai = new DeepPathFinder ();
		ai = new CollidePathFinder();
	}

	protected override void InitRole()
	{
		if (roleSprite == null) {
			roleSprite = Instantiate(Resources.Load<Sprite>("Images/Icon/last-fm"));
		}

		this.name = MazeManager.ROLE_NPC;
		this.Resume ();

		orginalPosition = new Vector3 (Orignal2d.x + Offset3d.x, Offset3d.y,  Orignal2d.y + Offset3d.x);
		nextPosition = orginalPosition;

		MazeManager.Instance.BroadcastNewRole (this);
		MazeManager.Instance.BroadcastRoleChanged (this);

		// ai
		MazeManager.Instance.NewMazeEvent += NewMaze;
		NewMaze (MazeManager.Instance.Maze);
	}

	protected override void DestroyRole()
	{
		MazeManager.Instance.NewMazeEvent -= NewMaze;

		MazeManager.Instance.BroadcastDestoryRole (this);
	}

	protected override void UpdateRole()
	{
		if (!Playing) {
			return;
		}

		ChooseNextPosition ();
	}

	void ChooseNextPosition()
	{
		if (ai.IsPathEmpty ()) {
			return;
		}

		if (errorCount >= 3) {
			return;
		}

		Vector3 position =  this.transform.position;
		Quaternion rotation = this.transform.rotation;
		if (position == nextPosition) {
			position = nextPosition;
			this.transform.position = position;

			Point2d.x = position.x - Offset3d.x;
			Point2d.y = position.z - Offset3d.z;
			// 位置问题
			MazeManager.Instance.UpdateRolePath (this, Point2d, Point2d);

			Dictionary<int, Line> rolePath = MazeManager.Instance.RolePath;
			List<Line> excludeLines = new List<Line> ();
			foreach (var item in rolePath) {
				if (item.Key != this.mark) {
					Debug.LogFormat ("{0}, Src {1}, Dest{2}", item.Key, item.Value.src, item.Value.dest);
					excludeLines.Add (item.Value);
				}
			}
			Debug.LogFormat ("{0}, Cur Point {1}", mark, Point2d);
			Vector2 np = ai.GetNextPoint (Point2d, excludeLines);
			Debug.LogFormat ("{0}, Next Point {1}", mark, np);
			if (np == Point2d) {
				this.Pause ();
				Invoke ("Resume", 1f);
				return;
			}

			MazeManager.Instance.UpdateRolePath (this, Point2d, np);

			nextPosition.x = np.x + Offset3d.x;
			nextPosition.z = np.y + Offset3d.z;

			nextRotation = Quaternion.LookRotation(nextPosition - position);
			return;
		}

		if (rotation != nextRotation) { // 方向
			float angle = Quaternion.Angle(rotation,	nextRotation);
			if (angle <= 1) {
				this.transform.rotation = nextRotation;
				return;
			}
			float percent = Time.deltaTime / 2 * 360 / angle;
			percent *= rotateSpeed;
			rotation= Quaternion.Lerp(rotation, nextRotation, percent);
			this.transform.rotation = rotation;
			return;
		}

		if (position != nextPosition) { // 位置
			float percent = Time.deltaTime * 1 / Vector3.Distance(position, nextPosition);
			percent *= moveSpeed;
			position = Vector3.Lerp(position, nextPosition, percent);
			this.transform.position = position;

			MazeManager.Instance.BroadcastRoleChanged (this);
		}
	}

	void NewMaze(Maze maze)
	{
		errorCount = 0;

		ai.Clear ();
		ai.Flush(maze);
		ai.SetFirst (Orignal2d);

		this.transform.position = orginalPosition;
		Point2d.x = orginalPosition.x - Offset3d.x;
		Point2d.y = orginalPosition.z - Offset3d.z;

		MazeManager.Instance.UpdateRolePath (this, Point2d, Point2d);

		Invoke("ResetPosition", 0.5f);
	}

	void ResetPosition()
	{
		MazeManager.Instance.BroadcastRoleChanged (this);
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.name == MazeManager.ROLE_PLAYER) {
			var audioSrc = this.GetComponent<AudioSource> ();
			if (audioSrc != null && audioSrc.isPlaying) {
				audioSrc.Stop ();
			}

			Debug.Log ("OnTriggerEnter : Catch Role");

			MazeManager.Instance.BroadcastGameState (GameState.Finish);
		} else {
			Debug.Log ("OnTriggerEnter : Error");
		}
	}
}
