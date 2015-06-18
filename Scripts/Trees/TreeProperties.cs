using UnityEngine;
using System.Collections;
using System;

public class monthInfo {
	public int monthStart;
	public int day = 1;
}

[System.Serializable]
public class prefabInfo: monthInfo {
	public GameObject prefab;
}

[System.Serializable]
public class textureInfo: monthInfo {
	public Texture texture;
}

public class TreeProperties : MonoBehaviour {

	public enum type {Generated, Model};
	public type treeType;
	ModeledTreeManager MTMinstance;
	GeneratedTreeManager GTMinstance;
	public prefabInfo[] trees;
	public int barkMatNumber = 1;
	public int leafMatNumber = 0;
	public textureInfo[] bark;
	public textureInfo[] leaves;
	
	void Start () {

		SceneManager.instance.OnNewDay += dayUpdate;
		if (treeType == type.Model) { 
			MTMinstance = gameObject.AddComponent<ModeledTreeManager>() as ModeledTreeManager;
			MTMinstance.barkMatNumber = barkMatNumber;
			MTMinstance.leafMatNumber = leafMatNumber;
			MTMinstance.bark = bark;
			MTMinstance.leaves = leaves;
			MTMinstance.Init ();
		} else { 
			GTMinstance = gameObject.AddComponent<GeneratedTreeManager>() as GeneratedTreeManager;
			GTMinstance.trees = trees;
			GTMinstance.Init ();
		}	

		dayUpdate ();
	}

	void dayUpdate () {

		changeMatColor ();
	}

	void Update () {

		changeMatColor ();//debug
	}

	public Gradient colorOverYear;
	public bool useOwnColors;
	void changeMatColor () {

		GameObject tree = treeType == type.Model ? gameObject : GTMinstance.currentTree;
		if (tree == null || tree.renderer.materials.Length < leafMatNumber + 1)
			return;
		Color col = useOwnColors ? colorOverYear.Evaluate (SceneManager.curvePos) : TreeColorManager.instance.currentColor;
		tree.renderer.materials [leafMatNumber].SetColor ("_Color", col);
	}

}
