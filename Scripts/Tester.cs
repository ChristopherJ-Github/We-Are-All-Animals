﻿using UnityEngine;
using System.Collections;

public class Tester : Singleton <Tester> {

	void Update () {

		CheckForInput ();
	}

	public static bool test = true;
	void CheckForInput () {

		if (Input.GetKeyDown(KeyCode.F)) 
			test = !test;
	}
}