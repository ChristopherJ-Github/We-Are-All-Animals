#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class FullscreenFocuser : MonoBehaviour {

	void Start () {

		StartCoroutine (FocusScreen ());
	}

	public float duration;
	public float clickDelay;
	IEnumerator FocusScreen () {

		float timePassed = 0;
		while (timePassed < duration) {
			Click();
			yield return new WaitForSeconds (clickDelay);
			timePassed += clickDelay;
		}
	}

	void Click () {

		MouseOperations.SetCursorPosition (Screen.width / 2, Screen.height / 2);
		MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown);
	}
}
#endif
