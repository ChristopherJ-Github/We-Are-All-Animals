using UnityEngine;
using System.Collections;
using System;

public class monthInfo {
	public int monthStart;
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
	public Gradient colorOverYear;
	public bool useOwnColors;

	//=====Generated tree variables======
	public prefabInfo[] trees;

	//======Modeled tree variables======

	public int barkMatNumber = 1;
	public int leafMatNumber = 0;
	public textureInfo[] bark;
	public textureInfo[] leaves;

	//=====Leaf particle variables====
	public bool hasLeafFall;
	public GameObject leafParticals;
	public AnimationCurve leafFallPercentage;
	
	float initFallAmount;

	void Start () {

		SceneManager.instance.OnNewDay += dayUpdate;
		if (treeType == type.Model) { 
			MTMinstance = gameObject.AddComponent<ModeledTreeManager>() as ModeledTreeManager;
			MTMinstance.barkMatNumber = barkMatNumber;
			MTMinstance.leafMatNumber = leafMatNumber;
			MTMinstance.bark = bark;
			MTMinstance.leaves = leaves;
			MTMinstance.init ();
		} else { 
			GTMinstance = gameObject.AddComponent<GeneratedTreeManager>() as GeneratedTreeManager;
			GTMinstance.trees = trees;
			GTMinstance.init ();
		}	

		if (hasLeafFall) 
			initFallAmount = leafParticals.particleSystem.emissionRate;
		dayUpdate ();
	}

	void dayUpdate () {

		setLeafParticals ();
		changeMatColor ();
	}

	void setLeafParticals () {

		if (!hasLeafFall)
			return;

		float fallPercentage = leafFallPercentage.Evaluate (SceneManager.curvePos);
		fallPercentage = Mathf.Clamp(fallPercentage,0f,1f);
		if (fallPercentage > 0) {
			leafParticals.SetActive(true);
			leafParticals.particleSystem.emissionRate = initFallAmount * fallPercentage;
		} else 
			leafParticals.SetActive(false);
	}

	void changeMatColor () {

		GameObject tree = treeType == type.Model ? gameObject : GTMinstance.currentTree;
		if (tree == null || tree.renderer.materials.Length < leafMatNumber + 1)
			return;
		
		Color col = useOwnColors ? colorOverYear.Evaluate (SceneManager.curvePos) : TreeColorManager.instance.currentColor;
		tree.renderer.materials [leafMatNumber].SetColor ("_Color", col);
	}

}
