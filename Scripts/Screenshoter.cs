using UnityEngine;
using System.Collections;

public class Screenshoter : MonoBehaviour { 
	public int multiplier = 1;
	public int frames;
	int currentFrame;

	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.S)) 
			screenShot();
		if (Input.GetKey (KeyCode.V))
			StartCoroutine (recordFrames());
	}

	IEnumerator recordFrames () {

		Debug.Log ("Started recording");
		while (currentFrame < frames) {
			screenShot(currentFrame.ToString());
			currentFrame ++;
			yield return null;
		}
		currentFrame = 0;
		Debug.Log ("Done recording");
	}

	void screenShot (string frameNumber = "") {

		Application.CaptureScreenshot("Screenshots/Screenshot_" +  System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss" + " " + frameNumber) + ".png", multiplier);
	}


}
