using UnityEngine;
using System.Collections;

public class WindStorm : GeneralWeather {

	private float initWindiness;

	void OnEnable () {

		initWindiness = WindControl.instance.windiness; 
		WindControl.instance.createDust = true;
	}

	void Update () {

		float transWindiness = Mathf.Lerp (initWindiness, WeatherControl.instance.severity < initWindiness ?
		                                   initWindiness : WeatherControl.instance.severity, WeatherControl.instance.transition);
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
