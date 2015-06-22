using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {
	
	public float minLightningDelay, maxLightningDelay;
	public AnimationCurve severityToDelay;

	public AnimationCurve severityToSpawnChance;
	private bool activated;
	void OnEnable () {

		activated = false;
		float spawnChance = severityToSpawnChance.Evaluate (WeatherControl.currentWeather.weather.severity);
		if (spawnChance > Random.value) {
			activated = true;
			CallLightning ();
		}
	}

	public void CallLightning () {

		float lerp = severityToDelay.Evaluate (WeatherControl.currentWeather.weather.severity);
		float currentMaxDelay = Mathf.Lerp (minLightningDelay, maxLightningDelay, lerp);
		float delay = Random.Range (0, currentMaxDelay);
		StartCoroutine (StartTimer (delay, currentMaxDelay));
	}

	public int maxFlashes;
	IEnumerator StartTimer (float initDelay, float initMaxDelay) {

		float timePassed = 0;
		float initDelayNorm = initDelay / initMaxDelay; 
		float currentDelay = initDelay;
		while (timePassed < currentDelay) {

			float lerp = severityToDelay.Evaluate (WeatherControl.currentWeather.weather.severity * WeatherControl.instance.totalTransition);
			float currentMaxDelay = Mathf.Lerp (minLightningDelay, maxLightningDelay, lerp);
			currentDelay = Mathf.Lerp(0, currentMaxDelay, initDelayNorm);
			timePassed += Time.deltaTime;
			yield return null;
		}
		StartCoroutine(SpawnLightning(Random.Range(1, maxFlashes + 1))); 
	}

	public LightningStrike lightningStrike;
	public float flashDelay;
	IEnumerator SpawnLightning (int flashes) {

		for (int i = 0; i < flashes; i ++) {
			Instantiate(lightningStrike, Vector3.zero, Quaternion.Euler(90,0,0));
			yield return new WaitForSeconds(flashDelay);
		}
		CallLightning ();
	}

	void OnDisable () {
	
		StopAllCoroutines ();
	}

	public void SwitchState (bool on) {

		bool aboutToActivate = on;
		if (aboutToActivate && !activated)
			CallLightning();
		if (!aboutToActivate && activated)
			StopAllCoroutines();
		activated = aboutToActivate;
	}
}
