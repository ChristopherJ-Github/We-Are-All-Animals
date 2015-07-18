using UnityEngine;
using System.Collections;

public class Fog : MonoBehaviour {
	
	public LightShafts lightShafts;
	private float initBrightnessAmount;

	public float maxSnowSeverity;
	public float GetMaxSeverity (float currentMaxSeverity) {

		float snowInfluence = (1 - CloudControl.instance.overcast) * SnowManager.instance.snowLevel;
		float maxAfterSnow = Mathf.Lerp (currentMaxSeverity, maxSnowSeverity, snowInfluence);
		return maxAfterSnow;
	}

	void OnEnable () {

		initBrightnessAmount = FogControl.instance.brightnessAmount;
		GUIManager.instance.OnGuiEvent += UpdateSeverity;
		UpdateFog ();
	}

	void UpdateSeverity (float severity) {

		WeatherControl.instance.severity = severity;
	}

	public float minBrightnessAmount, maxBrightnessAmount;
	void UpdateFog () {

		float brightnessAmount = Mathf.Lerp (minBrightnessAmount, maxBrightnessAmount, WeatherControl.instance.severity);
		float maxBrightness = brightnessAmount < initBrightnessAmount ? initBrightnessAmount : brightnessAmount;
		float currentBrightness = Mathf.Lerp (initBrightnessAmount, maxBrightness, WeatherControl.instance.transition);
		FogControl.instance.SetLightShaftBrightness (currentBrightness);
	}
	
	void Update () {
		
		UpdateFog ();
	}

	private bool applicationIsQuitting;
	void OnDisable () {

		if (applicationIsQuitting) return;
		FogControl.instance.SetLightShaftBrightness (initBrightnessAmount);
	}

	void OnApplicationQuit () {
		
		applicationIsQuitting = true;
	}
}

