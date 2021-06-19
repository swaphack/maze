using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 迷宫ui管理
/// </summary>
public class MazeUI : UILayer 
{
	public int width = 8;
	public int height = 8;

	Text txtWidth;
	Text txtHeight;

	Button btnCreate;
	Slider sliderWidth;
	Slider sliderHeight;
	Toggle togMap;
	MazeSmallMapUI map;

	Button btnAddNPC;
	Button btnAddRole;
	Button btnStart;

	Button btnRetry;

	RectTransform  SettingUI;
	RectTransform  FinishUI;

	protected override void InitControl()
	{
		MazeManager.Instance.UI = this;

		map = this.GetComponentInChildren<MazeSmallMapUI> ();

		btnCreate = this.GetChildWithRecursive<Button> ("btnCreate");

		txtWidth = this.GetChildWithRecursive<Text> ("txtWidth");
		txtHeight = this.GetChildWithRecursive<Text> ("txtHeight");

		sliderWidth = this.GetChildWithRecursive<Slider> ("sliderWidth");
		sliderHeight = this.GetChildWithRecursive<Slider>("sliderHeight");

		togMap = this.GetChildWithRecursive<Toggle>("togMap");

		btnAddRole = this.GetChildWithRecursive<Button>("btnAddRole");
		btnAddNPC = this.GetChildWithRecursive<Button>("btnAddNPC");

		SettingUI = this.GetChildWithRecursive<RectTransform>("SettingUI");
		FinishUI = this.GetChildWithRecursive<RectTransform>("FinishUI");

		btnRetry = this.GetChildWithRecursive<Button>("btnRetry");
		btnStart = this.GetChildWithRecursive<Button>("btnStart");

		if (btnCreate != null) {
			btnCreate.onClick.AddListener (InitMaze);
		}

		if (sliderWidth != null) {
			sliderWidth.minValue = 1;
			sliderWidth.maxValue = 50;
			sliderWidth.value = width;
			sliderWidth.onValueChanged.AddListener(WidthValueChanged);
		}

		if (sliderHeight != null) {
			sliderHeight.minValue = 1;
			sliderHeight.maxValue = 50;
			sliderHeight.value = width;
			sliderHeight.onValueChanged.AddListener(HeightValueChanged);
		}

		if (togMap != null) {
			togMap.onValueChanged.AddListener (ToggleValueChanged);
			ToggleValueChanged (togMap.isOn);
		}

		if (btnAddRole != null) {
			btnAddRole.onClick.AddListener (AddRole);
		}

		if (btnAddNPC != null) {
			btnAddNPC.onClick.AddListener (AddNPC);
		}

		if (btnRetry != null) {
			btnRetry.onClick.AddListener (RetryGame);
		}

		if (btnStart != null) {
			btnStart.onClick.AddListener (StartGame);
		}

		if (SettingUI != null) {
			SettingUI.SetVisible (false);
		}

		if (FinishUI != null) {
			FinishUI.SetVisible (false);
		}

		MazeManager.Instance.GameEvent += OnGameStateChange;

		WidthValueChanged (width);
		HeightValueChanged (height);

		MazeManager.Instance.BroadcastGameState (GameState.Start);
	}

	protected override void DestroyControl()
	{
		MazeManager.Instance.GameEvent -= OnGameStateChange;
	}

	void InitMaze()
	{
		MazeManager.Instance.SmallMap.ClearMaze ();
		MazeManager.Instance.Map.ClearMaze ();

		MazeManager.Instance.Init(width, height);
	}

	void AddRole()
	{
		MazeManager.Instance.Map.AddRole ();
	}

	void AddNPC()
	{
		MazeManager.Instance.Map.AddNPC ();
	}

	void StartGame()
	{
		MazeManager.Instance.BroadcastGameState (GameState.Play);
	}

	void RetryGame()
	{
		if (FinishUI != null) {
			FinishUI.SetVisible (false);
		}

		MazeManager.Instance.BroadcastGameState (GameState.Start);
	}

	void WidthValueChanged(float value)
	{
		width = (int)value;

		if (txtWidth == null) {
			return;
		}

		txtWidth.text = string.Format ("Width: {0}", width);
	}

	void HeightValueChanged(float value)
	{
		height = (int)value;

		if (txtHeight == null) {
			return;
		}
		txtHeight.text = string.Format ("Height: {0}", height);
	}

	void ToggleValueChanged(bool toggled)
	{
		if (togMap == null) {
			return;
		}

		if (map != null) {
			map.gameObject.SetActive (toggled);
		}
	}

	void OnGameStateChange(GameState state)
	{
		switch (state) {
		case GameState.Start:
			if (SettingUI != null) {
				SettingUI.SetVisible (true);
			}
			break;
		case GameState.Pause:
			break;
		case GameState.Play:
			if (SettingUI != null) {
				SettingUI.SetVisible (false);
			}
			break;
		case GameState.Finish:
			Media.PlayEffect ("Audio/Effect/catch");
			if (FinishUI != null) {
				FinishUI.SetVisible (true);
			}
			break;
		}
	}
}
