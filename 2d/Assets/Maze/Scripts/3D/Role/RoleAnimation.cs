using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class RoleAnimation : MonoBehaviour {

	private List<KeyCode> _keys;

	public ActionType actionType;
	private bool playing;

	public RoleAnimation()
	{
		_keys = new List<KeyCode> ();
	}
	// Use this for initialization
	void Start () {
		actionType = ActionType.Attack1;
	}

	public void PlayAnimation(ActionType type, bool loop = false)
	{
		var action = this.GetComponent<Animation> ();
		if (action == null) {
			return;
		}
		string actionName = RoleAction.GetActionName (type);
		action.PlayAction (actionName, (Animation target, string name) => {
			if (loop) {
				this.PlayAnimation (type, loop);
			}
		});
	}
	
	// Update is called once per frame
	void Update () {
		AutoPlay ();

		if (!Input.anyKey) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			_keys.Add (KeyCode.Q);
		} else if (Input.GetKeyDown (KeyCode.W)) {
			_keys.Add (KeyCode.W);
		} else if (Input.GetKeyDown (KeyCode.E)) {
			_keys.Add (KeyCode.E);
		} else if (Input.GetKeyDown (KeyCode.R)) {
			_keys.Add (KeyCode.R);
		}
	}

	void AutoPlay()
	{
		if (_keys.Count == 0) {
			return;
		}

		if (playing) {
			return;
		}

		KeyCode code = _keys [0];
		if (code == KeyCode.Q) {
			actionType = ActionType.Attack1;
		} else if (code == KeyCode.W) {
			actionType = ActionType.Attack2;
		} else if (code == KeyCode.E) {
			actionType = ActionType.Attack3;
		} else if (code == KeyCode.R) {
			actionType = ActionType.Attack4;
		}

		string actionName = RoleAction.GetActionName (actionType);
		if (string.IsNullOrEmpty (actionName)) {
			return;
		}

		var action = this.GetComponent<Animation> ();
		if (!action.IsPlaying (actionName) 
			|| (action.IsPlaying (actionName)  && action.isPlaying)) {

			playing = true;
			action.PlayAction (actionName, (Animation target, string name) => {
				_keys.RemoveAt (0);
				playing = false;
			});
			Media.PlayEffect3D ("Audio/Effect/blade_4", this.gameObject);
		}
	}
}
