using UnityEngine;
using System.Collections;

public class MaterialSetter : MonoBehaviour {

	public Material[] materials;

	void Start () {

		SetMaterials ();
	}

	void SetMaterials () {

		Material[] newMaterials = renderer.materials;
		for (int i = 0; i < newMaterials.Length; i ++) 
			newMaterials[i] = materials[i] ?? newMaterials[i];
		renderer.materials = newMaterials;
	}
}
