using UnityEngine;
using System.Collections;
using System;

public class GeneratedTreeManager: MonoBehaviour {

	public prefabInfo[] trees;
	public GameObject currentTree;

	// Use this for initialization
	public void init () { 

		SceneManager.instance.OnNewMonth += monthUpdate;
		renderer.enabled = false; //replace the tree in the editor with one's spawned from this script
		ObjectChanger.sortArray (trees); 
		monthUpdate ();
	}

	void monthUpdate () {

		currentTree = ObjectChanger.setPrefabForMonth(trees, currentTree, transform);
	}
}
