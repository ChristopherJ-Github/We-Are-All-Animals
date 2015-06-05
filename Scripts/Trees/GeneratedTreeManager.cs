using UnityEngine;
using System.Collections;
using System;

public class GeneratedTreeManager: MonoBehaviour {

	public prefabInfo[] trees;
	public GameObject currentTree;
	public static Transform parent;

	public void init () { 

		SceneManager.instance.OnNewMonth += monthUpdate;
		renderer.enabled = false; //replace the tree in the editor with one's spawned from this script
		if (parent == null) {
			parent = new GameObject ("Trees").transform;
		}
		ObjectChanger.sortArray (trees); 
		monthUpdate ();
	}

	void monthUpdate () {

		currentTree = ObjectChanger.setPrefabForMonth(trees, currentTree, transform);
		currentTree.transform.parent = parent;
	}
}
