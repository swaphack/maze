using UnityEngine;
using System.Collections;

/// <summary>
/// 媒体扩展
/// </summary>
public static class MediaExtensions
{
	/// <summary>
	/// 音效播放结束后处理
	/// </summary>
	/// <param name="src">Source.</param>
	/// <param name="callback">Callback.</param>
	public static void PlayAudio(this AudioSource src, AudioPlayEndCallback callback) {
		if (src == null || callback == null) {
			return;
		}

		var ext = src.GetComponent<AudioSourceExt> ();
		if (ext == null) {
			ext = src.AddComponent<AudioSourceExt> ();
		}

		ext.Play (callback);
	}

	/// <summary>
	/// 动画播放结束后处理
	/// </summary>
	/// <param name="src">Source.</param>
	/// <param name="callback">Callback.</param>
	public static void PlayAction(this Animation src, string name, AnimationPlayEndCallback callback) {
		if (src == null || callback == null) {
			return;
		}

		var ext = src.GetComponent<AnimationExt> ();
		if (ext == null) {
			ext = src.AddComponent<AnimationExt> ();
		}

		ext.Play (name, callback);
	}
}

