using UnityEngine;
using System.Collections;

public class DistanceFeeder : MonoBehaviour {

	public int materialIndex = 0;
	private float distance;

	public void InputDistance () {

		distance = Vector3.Distance (transform.position, Camera.main.transform.position);
		Material[] materials = renderer.materials;
		materials[materialIndex].SetFloat ("_Distance", distance);
		renderer.materials = materials;
	}
}
