using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StopNodeProperties))]
public class StopNodePropertiesEditor : Editor {

	public SerializedProperty idleAnimation;
	public SerializedProperty duration;
	public SerializedProperty randomAnimation;
	public SerializedProperty minInterval, maxInterval;
	public SerializedProperty landAnimation;
	public SerializedProperty landDuration;
	public SerializedProperty takeOffAnimation;
	public SerializedProperty takeOffDuration;
	public SerializedProperty afterMovementAnimation;
	public SerializedProperty afterMovementSpeed;
	public SerializedProperty showBirdFields;
	public SerializedProperty showRandomAnimFields;
	
	public void OnEnable() {

		idleAnimation = serializedObject.FindProperty ("idleAnimation");
		duration = serializedObject.FindProperty ("duration");
		randomAnimation = serializedObject.FindProperty ("randomAnimation");
		minInterval = serializedObject.FindProperty ("minInterval");
		maxInterval = serializedObject.FindProperty ("maxInterval");
		landAnimation = serializedObject.FindProperty ("landAnimation");
		landDuration = serializedObject.FindProperty ("landDuration");
		takeOffAnimation = serializedObject.FindProperty ("takeOffAnimation");
		takeOffDuration = serializedObject.FindProperty ("takeOffDuration");
		afterMovementAnimation = serializedObject.FindProperty ("afterMovementAnimation");
		afterMovementSpeed = serializedObject.FindProperty ("afterMovementSpeed");
		showBirdFields = serializedObject.FindProperty ("showBirdFields");
		showRandomAnimFields = serializedObject.FindProperty ("showRandomAnimFields");
	}

	public override void OnInspectorGUI() {

		serializedObject.Update ();
		EditorGUILayout.Separator ();
		EditorGUILayout.PropertyField (idleAnimation,  new GUIContent("Idle Animation"));
		EditorGUILayout.PropertyField (duration,  new GUIContent("Duration"));
		RandomAnimationFields ();
		EditorGUILayout.PropertyField (afterMovementAnimation,  new GUIContent("Animation Afterwards"));
		EditorGUILayout.PropertyField (afterMovementSpeed,  new GUIContent("Speed"));
		BirdFields ();
		serializedObject.ApplyModifiedProperties ();
	}

	void RandomAnimationFields () {

		showRandomAnimFields.boolValue = EditorGUILayout.ToggleLeft ("Random Animation", showRandomAnimFields.boolValue);
		if (showRandomAnimFields.boolValue) {
			EditorGUILayout.PropertyField (randomAnimation,  new GUIContent("Random Animation"));
			EditorGUILayout.PropertyField (minInterval,  new GUIContent("Min Interval"));
			EditorGUILayout.PropertyField (maxInterval,  new GUIContent("Max Interval"));
		}
	}

	void BirdFields () {

		showBirdFields.boolValue = EditorGUILayout.ToggleLeft ("Bird", showBirdFields.boolValue);
		if (showBirdFields.boolValue) {
			EditorGUILayout.PropertyField (landAnimation,  new GUIContent("Landing Animation"));
			EditorGUILayout.PropertyField (landDuration,  new GUIContent("Duration"));
			EditorGUILayout.PropertyField (takeOffAnimation,  new GUIContent("Takeoff Animation"));
			EditorGUILayout.PropertyField (takeOffDuration,  new GUIContent("Duration"));
		}
	}
}
