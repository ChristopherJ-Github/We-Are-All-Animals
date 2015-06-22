using UnityEngine;
using System.Collections;

public class MaterialSetter : MonoBehaviour {

	public Material[] materials;

	void Start () {

		renderer.materials = materials;
		//GameObject.Destroy (this);
	}
}
