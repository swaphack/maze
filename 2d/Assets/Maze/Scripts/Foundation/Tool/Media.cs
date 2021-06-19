using UnityEngine;
using System.Collections;

/// <summary>
/// 媒体
/// </summary>
public class Media
{
	/// <summary>
	/// 播放2d音效
	/// </summary>
	/// <param name="url">URL.</param>
	public static void PlayEffect(string url)
	{
		var clip = Resources.Load<AudioClip> (url);
		if (clip == null) {
			return;
		}

		var go = new GameObject ();
		var src = go.AddComponent<AudioSource> ();
		src.clip = clip;
		src.loop = false;
		src.PlayAudio ((AudioSource audio) => {
			Object.Destroy(src.gameObject);
		});
	}

	/// <summary>
	/// 播放背景音乐
	/// </summary>
	/// <param name="url">URL.</param>
	public static void PlayBackgroundMusic(string url) {
		var clip = Resources.Load<AudioClip> (url);
		if (clip == null) {
			return;
		}

		var go = GameObject.Find("BackgroundMusic");
		if (go == null) {
			go = new GameObject ();
			go.name = "BackgroundMusic";
		}
		var src = go.GetComponent<AudioSource> ();
		if (src == null) {
			src = go.AddComponent<AudioSource> ();
		}
		src.clip = clip;
		src.loop = false;
		src.PlayAudio ((AudioSource audio) => {
			Object.Destroy(src.gameObject);
		});
	}

	/// <summary>
	/// 播放3d音效
	/// </summary>
	/// <param name="url">URL.</param>
	/// <param name="gameObject">Game object.</param>
	public static void PlayEffect3D(string url, GameObject gameObject)
	{
		if (gameObject == null) {
			return;
		}

		var clip = Resources.Load<AudioClip> (url);
		if (clip == null) {
			return;
		}

		var src = gameObject.GetComponent<AudioSource>();
		if (src == null) {
			src = gameObject.AddComponent<AudioSource> ();
		}
		src.clip = clip;
		src.loop = false;
		src.spatialBlend = 1.0f;
		src.Play ();
	}
}

