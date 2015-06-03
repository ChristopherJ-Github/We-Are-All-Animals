using UnityEngine;
using System.Collections;

public class LeafFaller : MonoBehaviour {
#if UNITY_EDITOR
	void Start () {
	}
	
	void Update () {

		if (Input.GetKeyDown(KeyCode.T)) {
			SwitchLeaves();
		}
	}

	void SwitchLeaves () {

		Tree treeObject = gameObject.GetComponent<Tree> ();
		TreeEditor.TreeData treeData = treeObject.data as TreeEditor.TreeData;
		TreeEditor.TreeGroupLeaf[] treeGroupLeaves = treeData.leafGroups;
		foreach (TreeEditor.TreeGroupLeaf treeGroupLeaf in treeGroupLeaves) {
			treeGroupLeaf.visible = !treeGroupLeaf.visible;
		}
		
		Material[] materials;
		treeData.UpdateMesh (treeObject.transform.worldToLocalMatrix, out materials);
	}
#endif
}
