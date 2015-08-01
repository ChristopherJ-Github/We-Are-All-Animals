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
		Dust.instance.createDust = true;
	}

	void Update () {

		float maxWindiness = WeatherControl.instance.severity < initWindiness ? initWindiness : WeatherControl.instance.severity;
		float maxPresetWindiness = WindControl.instance.maxWeatherWindOverYear.Evaluate (SceneManager.curvePos);
		maxPresetWindiness = Mathf.Lerp (maxPresetWindiness, maxWindiness, SnowManager.instance.snowLevel);
		maxWindiness = maxWindiness > maxPresetWindiness ? maxPresetWindiness : maxWindiness;
		float transWindiness = Mathf.Lerp (initWindiness, maxWindiness, WeatherControl.instance.transition);
		float transSeverity = WeatherControl.instance.transition * WeatherControl.instance.severity;
		WindControl.instance.SetValues(transWindiness, transSeverity); 
	}

	void OnDisable () {

		if (applicationIsQuitting) return;
		Dust.instance.createDust = false;
		WindControl.instance.SetValues (initWindiness);
	}

	private bool applicationIsQuitting;
	void OnApplicationQuit () {

		applicationIsQuitting = true;
	}
}
