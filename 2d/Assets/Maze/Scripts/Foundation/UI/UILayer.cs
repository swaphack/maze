using UnityEngine;
using System.Collections;

public class UILayer : MonoBehaviour
{
	/// <summary>
	/// 文件路径
	/// </summary>
	private static string _filePath;

	public static string FilePath {
		get { 
			return _filePath;
		}
		protected set { 
			_filePath = value;
		}
	}
	void Start()
	{
		if (!LoadUIFile ()) {
			Debug.LogErrorFormat ("Read UI Error : {0}", FilePath);
			return;
		}
		InitControl ();
	}

	void OnDestroy() {
		DestroyControl ();
	}

	protected virtual bool LoadUIFile()
	{
		return true;
	}

	protected virtual void InitControl()
	{
		
	}

	protected virtual void DestroyControl()
	{
	}


	public virtual void InitData(params object[] data)
	{
		
	}
}

