using UnityEngine;
using System.Collections;

public class ObjectComparer : MonoBehaviour {

	public GameObject objectA, objectB;

	void Update () {

		objectA.SetActive (Tester.test);
		objectB.SetActive (!objectA.activeSelf);
	}
}
