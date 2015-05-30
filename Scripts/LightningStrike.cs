using UnityEngine;
using System.Collections;

public class LightningStrike : MonoBehaviour {

	public float intensity, duration;
	public AnimationCurve randomizationToIntensity;

	void Start () {

		float _intensity = Mathf.Lerp(0, intensity, CloudControl.instance.overcast);
		light.intensity = _intensity * WeatherControl.instance.totalTransition;
		StartCoroutine (FadeOut (duration));
	}

	IEnumerator FadeOut (float _duration) {
		
		float _intensity = light.intensity * randomizationToIntensity.Evaluate(Random.value);
		while (_duration != 0) {
			
			_duration = Mathf.MoveTowards(_duration, 0, Time.deltaTime);
			float lerp = _duration/duration;
			light.intensity = Mathf.Lerp(0, _intensity, lerp);
			yield return null;
		}
		Destroy (gameObject);
	}
}
