using UnityEngine;
using System.Collections;

public class MoonProperties : MonoBehaviour {

	void Update () {

		UpdateIntensity ();
		CompareSettings ();
	}
	
	public float minIntensity, maxIntensity;
	[HideInInspector] public float currentIntensity;
	public float snowInfluence;
	void UpdateIntensity () {
		
		float snowEffect = SnowManager.instance.snowLevel * snowInfluence;
		float darknessAmount = SkyManager.instance.sun.weatherDarkness + snowEffect;
		darknessAmount = Mathf.Clamp01 (darknessAmount);
		float currentDarkness = Mathf.Lerp (0, maxIntensity, darknessAmount);
		float intensity = Mathf.Clamp (maxIntensity - currentDarkness, minIntensity, maxIntensity);
		currentIntensity = intensity;                      
	}

	void CompareSettings () {

		if (Tester.test) {
			minIntensity = 0.06f;
			maxIntensity = 0.14f;
			snowInfluence = 0.81f;
		} else {
			minIntensity = 0.192194f;
			maxIntensity = 0.3656f;
			snowInfluence = 0.81f;
		}
	}
}
