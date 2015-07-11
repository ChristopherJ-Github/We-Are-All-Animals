using UnityEngine;
using System.Collections;

public class Fog : MonoBehaviour {
	
	public LightShafts lightShafts;
	private float initBrightness;

	void OnEnable () {

		initBrightness = FogControl.instance.brightness;
		brightness = Mathf.Lerp (minBrightness, maxBrightness, WeatherControl.instance.severity);
		GUIManager.instance.OnGuiEvent += UpdateSeverity;
		UpdateFog ();
	}

	void UpdateSeverity (float severity) {

		WeatherControl.instance.severity = severity;
		brightness = Mathf.Lerp (minBrightness, maxBrightness, WeatherControl.instance.severity);
	}

	private float brightness;
	public float minBrightness, maxBrightness;
	void UpdateFog () {

		float maxBrightness = brightness < initBrightness ? initBrightness : brightness;
		float currentBrightness = Mathf.Lerp (initBrightness, maxBrightness, WeatherControl.instance.transition);
		FogControl.instance.SetLightShaftBrightness (currentBrightness);
	}
	
	void Update () {
		
		UpdateFog ();
	}

	private bool applicationIsQuitting;
	void OnDisable () {

		if (applicationIsQuitting) return;
		FogControl.instance.SetLightShaftBrightness (initBrightness);
	}

	void OnApplicationQuit () {
		
		applicationIsQuitting = true;
	}
}

