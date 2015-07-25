using UnityEngine;
using System.Collections;

public class MaterialSetter : MonoBehaviour {

	public Material[] materials;

	void Start () {

		SetMaterials ();
		SetShaderProperties ();
	}

	private int leafIndex, barkIndex;
	[HideInInspector] public TreeProperties treeProperties;
	public void SetTreePropertyValues (TreeProperties treeProperties) {

		this.treeProperties = treeProperties;
		leafIndex = treeProperties.leafMatNumber;
		barkIndex = treeProperties.barkMatNumber;
	}
	
	public void SetMaterials () {

		Material[] newMaterials = renderer.materials;
		for (int i = 0; i < newMaterials.Length; i ++) 
			newMaterials[i] = materials[i] ?? newMaterials[i];
		renderer.materials = newMaterials;
	}
	
	void SetShaderProperties () {

		DistanceFeeder distanceFeeder = GetComponent<DistanceFeeder> ();
		if (distanceFeeder != null)
			distanceFeeder.InputDistance ();
		Material[] materials = renderer.materials;
		if (treeProperties.currentMonthStart == 4) {
			SetMinLeafAmount (materials);
			SetFallTintMultiplier (materials);
		}
	}
	
	void SetMinLeafAmount (Material[] materials) {

		float cutoff = materials [leafIndex].GetFloat ("_Cutoff");
		float leafReduction = Mathf.Lerp (1 - treeProperties.minLeafAmount, 0, LeafFallManager.instance.leafAmount);
		cutoff += leafReduction;
		materials [leafIndex].SetFloat ("_Cutoff", cutoff);
	}

	void SetFallTintMultiplier (Material[] materials) {

		materials [leafIndex].SetFloat ("_TintMultiplier", treeProperties.fallTintMultiplier);
	}
}
