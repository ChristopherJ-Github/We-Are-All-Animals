using UnityEngine;
using System.Collections;

public class Tester : Singleton <Tester> {

	void Update () {

		CheckForInput ();
		UpdateTestValue ();
		if (Application.isEditor)
			SpeedUpTime ();
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

	public float testValue01 = 1;
	public float transitionSpeed = 2.2f;
	void UpdateTestValue () {

		if (Input.GetKey(KeyCode.Semicolon))
			testValue01 -= transitionSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.Quote))
			testValue01 += transitionSpeed * Time.deltaTime;
		testValue01 = Mathf.Clamp01(testValue01);
	}

	public float timeScale;
	void SpeedUpTime () {

		Time.timeScale = timeScale;
	}
}
