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
	ModeledTreeManager modeledTreeManager;
	GeneratedTreeManager generatedTreeManager;
	public prefabInfo[] trees;
	public int barkMatNumber = 1;
	public int leafMatNumber = 0;
	public textureInfo[] bark;
	public textureInfo[] leaves;
	
	void Start () {

		SceneManager.instance.OnNewDay += dayUpdate;
		if (treeType == type.Model) 
			InitModeledTree();
		else 
			InitGeneratedTree();
		dayUpdate ();
	}

	void InitModeledTree () {

		modeledTreeManager = gameObject.AddComponent<ModeledTreeManager>() as ModeledTreeManager;
		modeledTreeManager.barkMatNumber = barkMatNumber;
		modeledTreeManager.leafMatNumber = leafMatNumber;
		modeledTreeManager.bark = bark;
		modeledTreeManager.leaves = leaves;
		modeledTreeManager.Init ();
	}

	public float minLeafAmount; //in TreeProperties to allow for unique values for instances
	void InitGeneratedTree () {

		generatedTreeManager = gameObject.AddComponent<GeneratedTreeManager>() as GeneratedTreeManager;
		generatedTreeManager.treeProperties = this;
		generatedTreeManager.trees = trees;
		generatedTreeManager.Init ();
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

		GameObject tree = treeType == type.Model ? gameObject : generatedTreeManager.currentTree;
		if (tree == null || tree.renderer.materials.Length < leafMatNumber + 1)
			return;
		Color col = useOwnColors ? colorOverYear.Evaluate (SceneManager.curvePos) : TreeColorManager.instance.currentColor;
		tree.renderer.materials [leafMatNumber].SetColor ("_Color", col);
	}

}
