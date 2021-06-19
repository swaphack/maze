using UnityEngine;
using System.Collections.Generic;

public delegate void AnimationPlayEndCallback(Animation animation, string name);

[RequireComponent(typeof(Animation))]
public class AnimationExt : MonoBehaviour
{
	internal class Task
	{
		public bool init;
		public string name;
		public float deltaTime;
		public AnimationPlayEndCallback callback;

		public Task(string name, float delta, AnimationPlayEndCallback handler)
		{
			this.name = name;
			this.deltaTime = delta;
			this.callback = handler;
			this.init = true;
		}

		public Task(Task task)
		{
			if (task == null) {
				return;
			}
			this.name = task.name;
			this.deltaTime = task.deltaTime;
			this.callback = task.callback;
			this.init = task.init;
		}

		public Task()
		{
			this.init = false;
		}
	}

	private Task _task;

	public AnimationExt()
	{
		_task = new Task ();
	}

	public void Play(string name, AnimationPlayEndCallback callback = null)
	{
		var animation = this.GetComponent<Animation> ();
		animation.Play (name);

		if (callback != null) {
			var state = animation[name];
			if (state != null) {
				_task = new Task (name, state.clip.length, callback);
			}
		}
	}

	void Update()
	{
		if (!_task.init) {
			return;
		}

		_task.deltaTime -= Time.deltaTime;
		if (_task.deltaTime <= 0) {
			if (_task.callback != null) {
				_task.init = false;
				var animation = this.GetComponent<Animation> ();
				_task.callback (animation, _task.name);
			}
		}
	}
}

