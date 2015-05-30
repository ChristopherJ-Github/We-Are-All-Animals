using UnityEngine;
using System.Collections;

public class TimeScaleText : MonoBehaviour
{

		GUIText _guiText;


		void Start ()
		{
				_guiText = GetComponent<GUIText> ();
		}
	
		
		void Update ()
		{
				if (GUIManager.instance.toggleStats)
						_guiText.text = "Playback speed: " + Time.timeScale + "x.  A slow, D fast, S reset";
				else
						_guiText.text = "";
		}
}
