using UnityEngine;
using System.Collections;

public class Fog : GeneralWeather {

	private bool applicationIsQuitting;
	public float minBrightness, maxBrightness;
	private float brightness;
	public float minExtinction, maxExtinction;
	public LightShafts lightShafts;
	
	void OnGuiEvent (float val) {
		
		severity = Mathf.Lerp (0, maxSeverity, val);
		//RenderSettings.fogDensity = Mathf.Lerp (minDesnity, maxSeverity, severity);
	}
	
	void OnEnable () {

		GUIManager.instance.OnGuiEvent += OnGuiEvent;
		brightness = Mathf.Lerp (minBrightness, maxBrightness, severity);
		GUIManager.instance.OnGuiEvent += OnGuiEvent;
		lightShafts.enabled = true;
		UpdateFog ();
	}
	
	void UpdateFog () {

		float _brightness = Mathf.Lerp (0, brightness, SkyManager.instance.nightDayLerp);
		lightShafts.m_Brightness = Mathf.Lerp (0, _brightness, WeatherControl.instance.transition);
		//lightShafts.m_Extinction = Mathf.Lerp (minExtinction, maxExtinction, WeatherControl.instance.transition);
	}
	
	void Update () {
		
		UpdateFog ();
	}
	
	void OnDisable () {

		if (applicationIsQuitting) return;

		GUIManager.instance.OnGuiEvent -= OnGuiEvent;
		lightShafts.enabled = false;
	}

	void OnApplicationQuit () {
		
		applicationIsQuitting = true;
	}
}

