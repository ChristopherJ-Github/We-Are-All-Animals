using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class BackgroundChanger : MonoBehaviour {
	
	public TextureInfo[] backgrounds;

	void OnEnable () {
		
		SceneManager.instance.OnNewMonth += monthUpdate;
		backgrounds = backgrounds.OrderBy(_textureInfo => _textureInfo.monthStart).ToArray();
		monthUpdate ();
		
	}
	
	void monthUpdate () {
		
		ObjectChanger.SetCurrentTexture (backgrounds, 0, renderer);
	}
	
}
