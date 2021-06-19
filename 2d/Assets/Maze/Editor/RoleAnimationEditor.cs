using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoleAnimation))]
public class RoleAnimationEditor : Editor {

	RoleAnimation role;

	bool loop = true;

	public void OnEnable() {
		role = this.target as RoleAnimation;
	}
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		role.actionType = (ActionType)EditorGUILayout.EnumPopup ("ActionType",  role.actionType, GUILayout.ExpandWidth (true));
		loop = EditorGUILayout.Toggle ("Loop", loop);

		if (GUILayout.Button ("Play")) {
			role.PlayAnimation (role.actionType, loop);
		}

		serializedObject.ApplyModifiedProperties ();
	}
}




