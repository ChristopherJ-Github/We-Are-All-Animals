using UnityEngine;
using System.Collections;

public class MoonProperties : MonoBehaviour {

	public float minIntensity;
	public float maxIntensity;
	[HideInInspector]
	public float currentIntesity;

	void Start () {

		currentIntesity = 0.74f;
	}
}
