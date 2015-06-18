using UnityEngine;
using System.Collections;

public class AtsBillboardTest : MonoBehaviour {

	public Color billboardColor;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 colorVector = new Vector3 (billboardColor.r, billboardColor.b, billboardColor.g);
		Shader.SetGlobalVector("tree_color", colorVector);
	}
}
