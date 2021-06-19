using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeMap))]
public class MazeGenerationEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (GUILayout.Button ("Auto Create"))
		{
			//var maze = serializedObject.targetObject as MazeMap;
			//maze.AutoCreateMaze ();
		}
	}
}
