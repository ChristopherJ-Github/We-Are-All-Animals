using UnityEngine;
using System.Collections;

public class Tester : Singleton <Tester> {

	void Update () {

		CheckForInput ();
		UpdateTestValue ();
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

	public float testValue = 1;
	void UpdateTestValue () {

		if (Input.GetKey(KeyCode.G))
			testValue -= 2.2f * Time.deltaTime;
		if (Input.GetKey(KeyCode.H))
			testValue += 2.2f * Time.deltaTime;
	}
}
