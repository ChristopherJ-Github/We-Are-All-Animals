using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StopNodeProperties))]
public class StopNodePropertiesEditor : Editor {
	
	public SerializedProperty landDuration;
	public SerializedProperty takeOffDuration;
	public SerializedProperty duration; 
	public SerializedProperty landAnimation;
	public SerializedProperty idleAnimation;
	public SerializedProperty takeOffAnimation;
	public SerializedProperty afterMovementAnimation;
	public SerializedProperty afterMovementSpeed;
	public SerializedProperty showBirdFields;

	// Use this for initialization
	public void OnEnable() {

		landDuration = serializedObject.FindProperty ("landDuration");
		takeOffDuration = serializedObject.FindProperty ("takeOffDuration");
		duration = serializedObject.FindProperty ("duration");
		landAnimation = serializedObject.FindProperty ("landAnimation");
		idleAnimation = serializedObject.FindProperty ("idleAnimation");
		takeOffAnimation = serializedObject.FindProperty ("takeOffAnimation");
		afterMovementAnimation = serializedObject.FindProperty ("afterMovementAnimation");
		afterMovementSpeed = serializedObject.FindProperty ("afterMovementSpeed");
		showBirdFields = serializedObject.FindProperty ("showBirdFields");

	}
	
	// Update is called once per frame
	public override void OnInspectorGUI() {

		serializedObject.Update ();

		EditorGUILayout.Separator ();
		EditorGUILayout.PropertyField (idleAnimation,  new GUIContent("Idle Animation"));
		EditorGUILayout.PropertyField (duration,  new GUIContent("Duration"));
		EditorGUILayout.PropertyField (afterMovementAnimation,  new GUIContent("Animation Afterwards"));
		EditorGUILayout.PropertyField (afterMovementSpeed,  new GUIContent("Speed"));


		showBirdFields.boolValue = EditorGUILayout.ToggleLeft ("Bird", showBirdFields.boolValue);

		if (showBirdFields.boolValue) {
			EditorGUILayout.PropertyField (landAnimation,  new GUIContent("Landing Animation"));
			EditorGUILayout.PropertyField (landDuration,  new GUIContent("Duration"));
			EditorGUILayout.PropertyField (takeOffAnimation,  new GUIContent("Takeoff Animation"));
			EditorGUILayout.PropertyField (takeOffDuration,  new GUIContent("Duration"));
		}
		serializedObject.ApplyModifiedProperties ();

	}
}
