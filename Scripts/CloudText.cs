using UnityEngine;
using System.Collections;

public class CloudText : MonoBehaviour {
	
	GUIText _guiText;
	
	void Start () {
		
		_guiText = GetComponent<GUIText> ();
	}
	
	void Update () {
		
		if (GUIManager.instance.toggleStats)
			_guiText.text = "timePassed: " + DynamicCloudControl.instance.timePassed.ToString() + 
				", currentDelay: " + DynamicCloudControl.instance.currentDelay.ToString() + 
				", extraOvercast: " + DynamicCloudControl.instance.extraOvercast.ToString() + 
				", extraOvercastGoal: " + DynamicCloudControl.instance.extraOvercastGoal.ToString();
		else
			_guiText.text = "";
	}
}
