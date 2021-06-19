using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 迷宫小地图ui
/// </summary>
public class MazeSmallMapUI : MonoBehaviour 
{
	public Sprite wallSprite;


	private Dictionary<int, Image> roleImages;

	private Vector2 screenSize;
	private Vector2 offset;
	private Vector2 itemSize;

	public MazeSmallMapUI()
	{
		roleImages = new Dictionary<int, Image> ();
	}

	// Use this for initialization
	void Start () {

		if (wallSprite == null) {
			wallSprite = Instantiate(Resources.Load<Sprite>("Images/Icon/mail"));
		}

		MazeManager.Instance.SmallMap = this;
		MazeManager.Instance.NewMazeEvent += PaintMaze;

		RectTransform rect = this.GetComponent<RectTransform> ();
		screenSize.x = rect.sizeDelta.x;
		screenSize.y = rect.sizeDelta.y;

		MazeManager.Instance.NewRoleEvent += NewRole;
		MazeManager.Instance.RoleChangedEvent += UpdateRole;
		MazeManager.Instance.DestoryRoleEvent += DestoryRole;

		MazeManager.Instance.GameEvent += OnGameStateChange;
	}

	void OnDestroy()
	{
		MazeManager.Instance.NewRoleEvent -= NewRole;
		MazeManager.Instance.RoleChangedEvent -= UpdateRole;
		MazeManager.Instance.DestoryRoleEvent -= DestoryRole;

		MazeManager.Instance.GameEvent -= OnGameStateChange;
	}

	void OnGameStateChange(GameState state)
	{
		switch (state) {
		case GameState.Start:
			ClearMaze ();
			break;
		case GameState.Pause:
			break;
		case GameState.Play:
			break;
		case GameState.Finish:
			break;
		}
	}

	public void ClearMaze()
	{
		roleImages.Clear ();
		for (int i = 0; i < this.transform.childCount; i++) {  
			var child = this.transform.GetChild (i);
			Destroy (child.gameObject);  
		}  
	}

	void PaintWall(float x, float y)
	{
		if (wallSprite == null) {
			return;
		}

		var go = new GameObject ();

		Image image = go.AddComponent<Image> ();
		image.sprite = wallSprite;

		RectTransform rect = image.GetComponent<RectTransform> ();
		go.transform.SetParent (this.transform);

		x += offset.x;
		y += offset.y;

		rect.localPosition = new Vector3 (x , y , 0) ;

		rect.sizeDelta = new Vector2 (itemSize.x, itemSize.y);
		rect.localScale = Vector3.one;
		rect.name = string.Format ("{0}, {1}", x, y);
	}

	void InitRole(MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}

		if (roleImages.ContainsKey (role.mark)) {
			return;
		}

		var go = new GameObject ();
		var image = go.AddComponent<Image> ();
		image.sprite = role.roleSprite;

		roleImages.Add (role.mark, image);


		RectTransform rect = image.GetComponent<RectTransform> ();
		go.transform.SetParent (this.transform);

		float w = itemSize.x * 0.8f;
		float h = itemSize.y * 0.8f;

		rect.sizeDelta = new Vector2 (w, h);
		rect.localScale = Vector3.one;
		rect.name = "role";
	}

	void PaintMaze (Maze maze)
	{
		int wNum = maze.width;
		int hNum = maze.height;

		float wItem = screenSize.x / wNum;
		float hItem = screenSize.y / hNum;

		wItem = wItem < hItem? wItem : hItem;

		itemSize.x = wItem;
		itemSize.y = wItem;

		float realWidth = wItem * wNum;
		float realHeight = wItem * hNum;

		offset.x = screenSize.x - realWidth;
		offset.y = screenSize.y - realHeight;

		for (int j = 0; j < hNum; j++) {
			for (int i = 0; i < wNum; i++) {
				bool bVisible = maze.map [j, i]  == PathMark.NotWalkable;
				if (bVisible) {
					float x = (2 * i + 1) * itemSize.x * 0.5f;
					float y = (2 * j + 1) * itemSize.y * 0.5f;
					PaintWall (x, y);
				}
			}
		}
	}

	void NewRole(MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}

		if (!roleImages.ContainsKey (role.mark)) {
			InitRole (role);
		}
	}

	// 更新角色位置
	void UpdateRole (MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}

		if (!roleImages.ContainsKey (role.mark)) {
			return;
		}

		Vector3 position = role.transform.position;
		Vector3 forward = role.transform.forward;

		var image = roleImages [role.mark];

		float x = position.x * itemSize.x;
		float y = position.z * itemSize.y;

		x += offset.x;
		y += offset.y;

		RectTransform rect = image.GetComponent<RectTransform> ();
		rect.localPosition = new Vector3 (x, y, 0);
	}

	void DestoryRole(MazeRoleBehaviour role)
	{
		if (role == null) {
			return;
		}

		if (!roleImages.ContainsKey (role.mark)) {
			return;
		}

		var image = roleImages [role.mark];
		image.transform.SetParent (null);
	}
}
