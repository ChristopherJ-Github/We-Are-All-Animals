using UnityEngine;
using System.Collections;

public class Escape : MonoBehaviour {

	void Update () {
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();
	}
}
