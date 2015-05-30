using UnityEngine;
using System;
using System.Collections;

public class Fader : MonoBehaviour
{
		static Fader _instance = null;

		public static Fader instance {
				get {
						if (!_instance) {
								// check if an FIX_ME is already available in the scene graph
								_instance = FindObjectOfType (typeof(Fader)) as Fader;

								// nope, create a new one
								if (!_instance) {
										var obj = new GameObject ("Fader");
										_instance = obj.AddComponent<Fader> ();
								}
						}

						return _instance;
				}
		}

		void OnApplicationQuit ()
		{
				// release reference on exit
				_instance = null;
		}

		public void FadeInOut (Material fadeObj)
		{
				StartCoroutine (fadeObj.Fade (1.0f, 0.2f, 0.6f, 
						() => {
				
						StartCoroutine (fadeObj.Fade (0.1f, 1.0f, 0.6f, 
										() => {
								Debug.Log ("fade is finished");
								//Do things when fade is done
						}));
				}));
		}

		public void FadeColorIn (Material fadeObj, float duration, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeColor (0, 1f, duration, onComplete));
		}
	
		public void FadeColorOut (Material fadeObj, float duration, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeColor (1, 0f, duration, onComplete));
		}

		public void FadeSpriteIn (SpriteRenderer fadeObj, float duration, float delay, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeSprite (0, 1f, duration, delay, onComplete));
		}
	
		public void FadeSpriteOut (SpriteRenderer fadeObj, float duration, float delay, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeSprite (1, 0f, duration, delay, onComplete));
		}


		public void FadeSpriteColIn (SpriteRenderer fadeObj, float duration, float delay, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeSpriteColor (fadeObj.color.r, 1f, duration, delay, onComplete));
		}
	
		public void FadeSpriteColOut (SpriteRenderer fadeObj, float duration, float delay, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeSpriteColor (fadeObj.color.r, 0.5f, duration, delay, onComplete));
		}



		public void FadeAlphaIn (Material fadeObj, float duration, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeAlpha (0, 1f, duration, onComplete));
		}

		public void FadeAlphaIn (Material fadeObj, float target, float duration, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeAlpha (fadeObj.color.a, target, duration, onComplete));
		}

		public void FadeAlphaOut (Material fadeObj, float duration, Action onComplete)
		{
				StartCoroutine (fadeObj.FadeAlpha (1, 0f, duration, onComplete));
		}
}

public static class AlphaExtensions
{
		public  static IEnumerator Fade (
				this Material mat,
				float start,
				float target,
				float duration,
				System.Action onComplete)
		{
				float elapsed = 0;
				float temp = start;
				float range = target - temp;
				while (elapsed < duration) {
						elapsed = Mathf.MoveTowards (elapsed, duration, Time.deltaTime);
						temp = start + range * (elapsed / duration);
						mat.SetFloat ("_Cutoff", temp);	
						yield return 0;
				}
				
				if (onComplete != null)
						onComplete ();
				
		}

		public  static IEnumerator FadeColor (
		this Material mat,
		float start,
		float target,
		float duration,
		System.Action onComplete)
		{
				float elapsed = 0;
				float temp = start;
				float range = target - temp;
				while (elapsed < duration) {
						elapsed = Mathf.MoveTowards (elapsed, duration, Time.deltaTime);
						temp = start + range * (elapsed / duration);
						mat.SetFloat ("_GrayScale", temp);	
						yield return 0;
				}
		
				if (onComplete != null)
						onComplete ();
		
		}

		public  static IEnumerator FadeSprite (
		this SpriteRenderer spr,
		float start,
		float target,
		float duration,
		float delay,
		System.Action onComplete)
		{
				Color col = spr.color;
				float elapsed = 0;
				float temp = start;
				float range = target - temp;
				yield return new WaitForSeconds (delay);
				while (elapsed < duration) {
						elapsed = Mathf.MoveTowards (elapsed, duration, Time.deltaTime);
						temp = start + range * (elapsed / duration);
						col.a = temp;
						spr.color = col;
						yield return 0;
				}
		
				if (onComplete != null)
						onComplete ();
		
		}

		
	
		public  static IEnumerator FadeSpriteColor (
		this SpriteRenderer spr,
		float start,
		float target,
		float duration,
		float delay,
		System.Action onComplete)
		{
				Color col = spr.color;
				float elapsed = 0;
				float temp = start;
				float range = target - temp;
				yield return new WaitForSeconds (delay);
				while (elapsed < duration) {
						elapsed = Mathf.MoveTowards (elapsed, duration, Time.deltaTime);
						temp = start + range * (elapsed / duration);
						col.r = temp;
						col.g = temp;
						col.b = temp;
						spr.color = col;
						yield return 0;
				}
		
				if (onComplete != null)
						onComplete ();
		
		}



		public  static IEnumerator FadeAlpha (
				this Material mat,
				float start,
				float target,
				float duration,
				System.Action onComplete)
		{
				Color col = mat.color;
				float elapsed = 0;
				float temp = start;
				float range = target - temp;
				while (elapsed < duration) {
						elapsed = Mathf.MoveTowards (elapsed, duration, Time.deltaTime);
						temp = start + range * (elapsed / duration);
						col.a = temp;
						mat.color = col;
						yield return 0;
				}
				
				if (onComplete != null)
						onComplete ();
				
		}
}
