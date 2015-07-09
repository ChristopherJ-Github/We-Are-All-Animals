using UnityEngine;
using System.Collections;

public class RealDateText : MonoBehaviour {
	
	GUIText _guiText;
	
	void Start () {
		
		_guiText = GetComponent<GUIText> ();
	}
	
	void Update () {
		
		if (GUIManager.instance.toggleStats)
			_guiText.text = "Real Date: " + SceneManager.realDate.ToString ();
		else
			_guiText.text = "";
	}
}
