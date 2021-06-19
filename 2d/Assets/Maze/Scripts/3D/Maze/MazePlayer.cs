using UnityEngine;
using System.Collections;

public class MazePlayer : MazeRoleBehaviour
{
	// Use this for initialization
	protected override void InitRole ()
	{
		if (roleSprite == null) {
			roleSprite = Instantiate(Resources.Load<Sprite>("Images/Icon/aim"));
		}

		this.name = MazeManager.ROLE_PLAYER;
		this.Resume ();

		orginalPosition = new Vector3 (Orignal2d.x + Offset3d.x, Offset3d.y,  Orignal2d.y + Offset3d.x);

		MazeManager.Instance.BroadcastNewRole (this);
		MazeManager.Instance.BroadcastRoleChanged (this);
	}

	void ListenKeyEvent()
	{
		var audioSrc = this.GetComponent<AudioSource> ();

		if (!Input.anyKey) {
			if (audioSrc != null && audioSrc.isPlaying) {
				audioSrc.Stop ();
			}
			MazeManager.Instance.BroadcastRoleChanged (this);
			return;
		}
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			this.transform.Rotate (Vector3.up, -Time.deltaTime * 360 * rotateSpeed);
		} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			this.transform.Rotate (Vector3.up, Time.deltaTime * 360 * rotateSpeed);
		} else if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
			if (audioSrc != null && !audioSrc.isPlaying) {
				audioSrc.Play ();
			}

			this.transform.position += this.transform.forward * Time.deltaTime * moveSpeed;
		} else if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
			if (audioSrc != null && !audioSrc.isPlaying) {
				audioSrc.Play ();
			}

			this.transform.position -= this.transform.forward * Time.deltaTime * moveSpeed;
		}

		MazeManager.Instance.BroadcastRoleChanged (this);
	}
	
	// Update is called once per frame
	protected override void UpdateRole ()
	{
		ListenKeyEvent ();
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.name == MazeManager.ROLE_NPC) 
		{
			var audioSrc = this.GetComponent<AudioSource> ();
			if (audioSrc != null && audioSrc.isPlaying) {
				audioSrc.Stop ();
			}
				
			Debug.Log ("OnCollisionEnter : Your Are Dead");

			MazeManager.Instance.BroadcastGameState (GameState.Finish);
		}
	}

	protected override void DestroyRole()
	{
		MazeManager.Instance.BroadcastDestoryRole (this);
	}
}

