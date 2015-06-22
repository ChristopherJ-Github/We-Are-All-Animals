using UnityEngine;
using System;
using System.Linq;
using System.Collections;


public class GeneratedTreeManager: MonoBehaviour {

	public prefabInfo[] trees;
	public GameObject currentTree;
	public static Transform parent;

	public void Init () { 

		SceneManager.instance.OnNewDay += SetCurrentPrefab;
		renderer.enabled = false; //replace the tree in the editor with one's spawned from this script
		if (parent == null) {
			parent = new GameObject ("Trees").transform;
		}
		trees = trees.OrderBy (_prefabInfo => _prefabInfo.monthStart).ThenBy (_prefabInfo => _prefabInfo.day).ToArray ();
		SetCurrentPrefab ();
	}

	void SetCurrentPrefab () {

		currentTree = ObjectChanger.SetCurrentPrefab(trees, currentTree, transform);
		currentTree.transform.parent = parent;
	}
}
