using UnityEngine;
using System;
using System.Linq;
using System.Collections;


public class GeneratedTreeManager: MonoBehaviour {

	public TreeProperties treeProperties;
	public PrefabInfo[] trees;
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

		currentTree = ObjectChanger.SetCurrentPrefab(trees, currentTree, transform, out treeProperties.currentMonthStart);
		currentTree.transform.parent = parent;
		MaterialSetter materialSetter = currentTree.GetComponent<MaterialSetter> ();
		if (materialSetter != null)
			materialSetter.SetTreePropertyValues (treeProperties);
	}
}
