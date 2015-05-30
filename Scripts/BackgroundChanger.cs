using UnityEngine;
using System.Collections;
using System;

public class BackgroundChanger : MonoBehaviour {
	
	public textureInfo[] backgrounds;

	void OnEnable () {
		
		SceneManager.instance.OnNewMonth += monthUpdate;
		ObjectChanger.sortArray (backgrounds);
		monthUpdate ();
		
	}
	
	void monthUpdate () {
		
		ObjectChanger.setTextureForMonth (backgrounds, 0, renderer);
		
	}
	
}
