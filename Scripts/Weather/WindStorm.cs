using UnityEngine;
using System.Collections;

public class WindStorm : GeneralWeather {

	private float initWindiness;
	private bool applicationIsQuitting;

	void OnGuiEvent (float val) {
		
		severity = Mathf.Lerp (0, maxSeverity, val);
	}

	void OnEnable () {

		GUIManager.instance.OnGuiEvent += OnGuiEvent;
		initWindiness = WindControl.instance.windiness; //comment out for webbuild
		WindControl.instance.createDust = true;
	}

	void Update () {

		float transWindiness = Mathf.Lerp (initWindiness, severity < initWindiness ? initWindiness : severity, WeatherControl.instance.transition);
		WindControl.instance.SetValues(transWindiness); //comment out for webbuild
	}

	void OnDisable () {

		if (applicationIsQuitting) return;

		WindControl.instance.createDust = false;
		GUIManager.instance.OnGuiEvent -= OnGuiEvent;
		WindControl.instance.SetValues (initWindiness);
	}

	void OnApplicationQuit () {

		applicationIsQuitting = true;
	}
}
