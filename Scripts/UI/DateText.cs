using UnityEngine;
using System.Collections;

public class DateText : MonoBehaviour
{
		GUIText _guiText;


		void Start ()
		{
				_guiText = GetComponent<GUIText> ();
		}
	
		// Update is called once per frame
		void Update ()
		{		if (GUIManager.instance.toggleStats)
						_guiText.text = SceneManager.currentDate.ToString ();
				else
						_guiText.text = "";
		}

}
