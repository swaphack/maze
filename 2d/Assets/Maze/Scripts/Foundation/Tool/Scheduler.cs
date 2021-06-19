using UnityEngine;
using System.Collections.Generic;

public delegate void SchedulerCallback(Object obj);

/// <summary>
/// 调度器
/// </summary>
public class Scheduler : MonoBehaviour
{
	internal class Task
	{
		public Object target;
		public float deltaTime;
		public SchedulerCallback callback;

		public Task(Object target, float delta, SchedulerCallback handler)
		{
			this.target = target;
			this.deltaTime = delta;
			this.callback = handler;
		}
	}

	/// <summary>
	/// 静态实例
	/// </summary>
	public static Scheduler _instance;

	private List<Task> _tasks;

	public Scheduler()
	{
		_tasks = new List<Task> ();
	}
	/// <summary>
	/// 静态实例
	/// </summary>
	/// <value>The instance.</value>
	public static Scheduler Instance {
		get { 
			if (_instance == null) {
				var go = GameObject.Find ("Scheduler");
				if (go == null) {
					go = new GameObject ();
					go.name = "Scheduler";
					_instance = go.AddComponent<Scheduler> ();
				}
			}
			return _instance; 
		}
	}

	public void AddTask(Object target, float delta, SchedulerCallback callback)
	{
		if (target == null || callback == null) {
			return;
		}

		_tasks.Add (new Task (target, delta, callback));
	}

	void Update()
	{
		int count = _tasks.Count;
		for (int i = count - 1; i >= 0; i--) {
			var task = _tasks [i];
			task.deltaTime -= Time.deltaTime;
			if (task.deltaTime <= 0) {
				if (task.callback != null) {
					task.callback (task.target);
				}
				_tasks.RemoveAt (i);
			}
		}
	}
}

