using UnityEngine;

public delegate void AudioPlayEndCallback(AudioSource audio);

[RequireComponent(typeof(AudioSource))]
public class AudioSourceExt : MonoBehaviour
{
	internal class Task
	{
		public bool init;
		public float deltaTime;
		public AudioPlayEndCallback callback;

		public Task(float delta, AudioPlayEndCallback handler)
		{
			this.deltaTime = delta;
			this.callback = handler;
			this.init = true;
		}

		public Task()
		{
			this.init = false;
		}
	}

	private Task _task;

	public AudioSourceExt()
	{
		_task = new Task ();
	}

	public void Play(AudioPlayEndCallback callback = null)
	{
		var audio = this.GetComponent<AudioSource> ();
		audio.Play ();

		if (callback != null) {
			var clip = audio.clip;
			_task = new Task (clip.length, callback);
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
				var audio = this.GetComponent<AudioSource> ();
				_task.callback (audio);
			}
		}
	}
}

