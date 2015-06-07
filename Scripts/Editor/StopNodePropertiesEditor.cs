using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StopNodeProperties))]
public class StopNodePropertiesEditor : Editor {
	
	public SerializedProperty landDuration;
	public SerializedProperty takeOffDuration;
	public SerializedProperty duration; 
	public SerializedProperty idleAnimation;
	public SerializedProperty randomAnimation;
	public SerializedProperty landAnimation;
	public SerializedProperty takeOffAnimation;
	public SerializedProperty afterMovementAnimation;
	public SerializedProperty afterMovementSpeed;
	public SerializedProperty showBirdFields;
	
	public void OnEnable() {

		landDuration = serializedObject.FindProperty ("landDuration");
		takeOffDuration = serializedObject.FindProperty ("takeOffDuration");
		duration = serializedObject.FindProperty ("duration");
		idleAnimation = serializedObject.FindProperty ("idleAnimation");
		randomAnimation = serializedObject.FindProperty ("randomAnimation");
		landAnimation = serializedObject.FindProperty ("landAnimation");
		takeOffAnimation = serializedObject.FindProperty ("takeOffAnimation");
		afterMovementAnimation = serializedObject.FindProperty ("afterMovementAnimation");
		afterMovementSpeed = serializedObject.FindProperty ("afterMovementSpeed");
		showBirdFields = serializedObject.FindProperty ("showBirdFields");
	}

	public override void OnInspectorGUI() {

		serializedObject.Update ();

		EditorGUILayout.Separator ();
		EditorGUILayout.PropertyField (idleAnimation,  new GUIContent("Idle Animation"));
		EditorGUILayout.PropertyField (randomAnimation,  new GUIContent("Random Animation"));
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
