using UnityEngine;
using System.Collections;

public class DistanceFeeder : MonoBehaviour {

	public int materialIndex = 0;
	private float distance;
	void Start () {

		distance = Vector3.Distance (transform.position, Camera.main.transform.position);
	}

	void Update () {

		Material[] materials = renderer.materials;
		materials[materialIndex].SetFloat ("_Distance", distance);
		renderer.materials = materials;
	}
}
