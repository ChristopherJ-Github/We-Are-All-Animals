using UnityEngine;
using System.Collections;

public class DateText : MonoBehaviour {

	public GameObject background;
	private GUIText _guiText;

	void Start () {

		_guiText = GetComponent<GUIText> ();
	}

	void Update () {

		if (GUIManager.instance.toggleStats) {
			_guiText.text = SceneManager.currentDate.ToString ();
			background.SetActive(true);
		} else { 
			_guiText.text = "";
			background.SetActive(false);
		}
	}
}
