using UnityEngine;
using System.Collections;

public class MazeSun : MonoBehaviour
{
	public const float DayTime = 60;
	/// <summary>
	/// 旋转速度
	/// </summary>
	[Range(0.1f, 10)]
	public float rotateSpeed = 1;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.transform.Rotate (new Vector3(360 / DayTime * Time.deltaTime * rotateSpeed, 0, 0));
	}
}

