using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TreeProperties))]
public class TreePropertiesEditor : Editor {
	public SerializedProperty treeType;

	//=====Generated tree variables======
	public SerializedProperty trees;
	public SerializedProperty minLeafAmount;
	public SerializedProperty fallTintMultiplier;

	//======Modeled tree variables======
	
	public SerializedProperty barkMatNumber;
	public SerializedProperty leafMatNumber;
	public SerializedProperty bark;
	public SerializedProperty leaves;

	//=====Leaf particle variables====
	public SerializedProperty hasLeafFall;
	public SerializedProperty leafParticals;
	public SerializedProperty leafFallPercentage;

	public void OnEnable() {
		
		treeType = serializedObject.FindProperty ("treeType");
		trees = serializedObject.FindProperty ("trees");
		barkMatNumber = serializedObject.FindProperty ("barkMatNumber");
		leafMatNumber = serializedObject.FindProperty ("leafMatNumber");
		bark = serializedObject.FindProperty ("bark");
		leaves = serializedObject.FindProperty ("leaves");
		leafFallPercentage = serializedObject.FindProperty ("leafFallPercentage");
		leafParticals= serializedObject.FindProperty ("leafParticals");
		hasLeafFall = serializedObject.FindProperty ("hasLeafFall");
		minLeafAmount = serializedObject.FindProperty ("minLeafAmount");
		fallTintMultiplier = serializedObject.FindProperty ("fallTintMultiplier");
	}
	
	// Update is called once per frame
	public override void OnInspectorGUI() {
		
		serializedObject.Update ();

		EditorGUILayout.PropertyField(treeType, true);
		EditorGUILayout.PropertyField(barkMatNumber, true);
		EditorGUILayout.PropertyField(leafMatNumber, true);
		EditorGUILayout.PropertyField (minLeafAmount, true);
		EditorGUILayout.PropertyField (fallTintMultiplier, true);
		if (treeType.enumValueIndex == 0) {
			EditorGUILayout.PropertyField(trees, true);
		} else {
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.PropertyField(bark, true);
			EditorGUILayout.PropertyField(leaves, true);

		}
		serializedObject.ApplyModifiedProperties ();
		
	}
}
