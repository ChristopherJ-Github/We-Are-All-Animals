using UnityEngine;
using System.Collections;

public class UnveilSettings : MonoBehaviour {
	
	void Start () {

		SkyManager.instance.SetPhaseTimes (12, 15, 19.5f, 20.25f);
	}

	void Update () {
	
	}
}
