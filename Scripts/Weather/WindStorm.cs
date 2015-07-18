using UnityEngine;
using System.Collections;

public class WindStorm : MonoBehaviour {

	private float initWindiness;

	public static float GetMaxSeverity (float currentMaxSeverity) {
		
		float maxAfterSnow = Mathf.Lerp (currentMaxSeverity, 1, SnowManager.instance.snowLevel);
		return maxAfterSnow;
	}

	void OnEnable () {

		initWindiness = WindControl.instance.windiness; 
		WindControl.instance.createDust = true;
	}

	void Update () {

		float maxWindiness = WeatherControl.instance.severity < initWindiness ? initWindiness : WeatherControl.instance.severity;
		float transWindiness = Mathf.Lerp (initWindiness, maxWindiness, WeatherControl.instance.transition);
		WindControl.instance.SetValues(transWindiness); 
	}

	void OnDisable () {

		if (applicationIsQuitting) return;
		WindControl.instance.createDust = false;
		WindControl.instance.SetValues (initWindiness);
	}

	private bool applicationIsQuitting;
	void OnApplicationQuit () {

		applicationIsQuitting = true;
	}
}
