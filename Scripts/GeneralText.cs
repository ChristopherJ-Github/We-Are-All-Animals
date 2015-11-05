using UnityEngine;
using System.Collections;

public class GeneralText : MonoBehaviour {
	
	GUIText _guiText;
	
	void Start () {
		
		_guiText = GetComponent<GUIText> ();
	}
	
	void Update () {
		
		if (GUIManager.instance.toggleStats)
			_guiText.text = "Overcast: " + CloudControl.instance.overcast.ToString() + 
				", windiness: " + WindControl.instance.windiness.ToString() + 
				", middayValue: " + CloudControl.instance.middayValue.ToString() + 
				", snowLevel: " + SnowManager.instance.snowLevel + 
				", linearSnowLevel: " + SnowManager.instance.linearSnowLevel;
		else
			_guiText.text = "";
	}
}
