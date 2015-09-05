using UnityEngine;
using System.Collections;

public class MoonProperties : MonoBehaviour {

	void Update () {

		UpdateIntensity ();
	}
	
	public float minIntensity, maxIntensity;
	[HideInInspector] public float currentIntensity;
	public float snowInfluence;
	void UpdateIntensity () {
		
		float snowEffect = SnowManager.instance.snowLevel * snowInfluence;
		float darknessAmount = SkyManager.instance.sun.weatherDarkness + snowEffect;
		float currentDarkness = Mathf.Lerp (0, maxIntensity, darknessAmount);
		float intensity = Mathf.Clamp (maxIntensity - currentDarkness, minIntensity, maxIntensity);
		currentIntensity = intensity;
	}
}
