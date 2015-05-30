using UnityEngine;
using System.Collections;

public class FadeToGame : MonoBehaviour
{
		public float delay = 2;
		public float duration;

		int timesFaded = 0;
		IEnumerator Start ()
		{
				yield return StartCoroutine (Auto.Wait (delay));
				StartCoroutine (BeginFade ());
				
		}

		void OnEnable ()
		{
				SceneManager.instance.StartFadeEvent += StartFade;
		}
		void OnDisable ()
		{
				if (SceneManager.instance != null)
						SceneManager.instance.StartFadeEvent -= StartFade;
		}

		void StartFade (int fade)
		{
				StartCoroutine (BeginFade ());
		}

		IEnumerator BeginFade ()
		{
				
				yield return StartCoroutine (Auto.Wait (delay));
				Fader.instance.FadeAlphaIn (renderer.material, 1.0f, duration, FadeOut);
		}
	
		void FadeOut ()
		{
				SceneManager.instance.TriggerAfterFade (timesFaded);
				timesFaded++;
				Fader.instance.FadeAlphaOut (renderer.material, duration, null);

		}
}