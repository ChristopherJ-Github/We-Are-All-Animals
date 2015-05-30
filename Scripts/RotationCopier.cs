using UnityEngine;
using System.Collections;

public class RotationCopier : MonoBehaviour {

	public Transform toCopy;

	void Update () {

		transform.rotation = toCopy.rotation;
	}
}
