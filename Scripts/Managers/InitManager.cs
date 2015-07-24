using UnityEngine;
using System.Collections;

public class InitManager : MonoBehaviour {

	//this class is used to properly instantiate and assign member references 
	//to all singletons via a call to the instance property
	
	private SceneManager sceneManager;
	public GUISkin gSkin;
	
	void Awake () {

		sceneManager = SceneManager.instance;
		GUIManager.instance.gSkin = gSkin;
	}
}
