using UnityEngine;
using System.Collections;

public class Tester : Singleton <Tester> {

	void Update () {

		CheckForInput ();
	}

	public static bool test = true;
	public static bool buttonPressed;
	void CheckForInput () {

		if (Input.GetKeyDown(KeyCode.F)) 
			test = !test;
		buttonPressed = false;
		if (Input.GetKeyDown(KeyCode.F)) 
			buttonPressed = true;
	}
}
