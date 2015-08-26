using UnityEngine;
using System.Collections;

public class Screenshoter : MonoBehaviour { 
	
	void Update () {

		if (Input.GetKey (KeyCode.S)) 
			TakeScreen();
	}

	public int multiplier = 1;
	void TakeScreen (string frameNumber = "") {

		Application.CaptureScreenshot("Screenshots/Screenshot_" +  System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss" + " " + frameNumber) + ".png", multiplier);
	}


}
