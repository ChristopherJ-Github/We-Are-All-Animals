using UnityEngine;
using System.Collections;

public class AmbientLightingChanger : Singleton <AmbientLightingChanger> {

	void Start () {

		SceneManager.instance.OnNewDay += SetTodaysMiddayColor;
		SetTodaysMiddayColor ();
	}

	private Color midayColorOfDay;
	void SetTodaysMiddayColor () {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
	}

	void Update () {

		UpdateAmbientLight ();
	}

	void UpdateAmbientLight () {

		float darkness = Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		SetNightAmbience (darkness);
		SetDuskDarkness (darkness);
		SetMiddayAmbience (darkness);
	}

	public Color _night;
	[HideInInspector] public Color night;
	Color SetNightAmbience (float darkness) {

		night = SetDarkness (_night, darkness);
	}

	public Color _dusk;
	[HideInInspector] public Color dusk;
	Color SetDuskDarkness (float darkness) {

		dusk = SetDarkness (_dusk, darkness);
	}

	public Gradient middayOverYear;
	public Gradient _midday;
	[HideInInspector] public Color midday;
	Color SetMiddayAmbience (float darkness) {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
		Color middayFullSnow = _midday.Evaluate (CloudControl.instance.middayValue);
		Color middayAfterSnow = Color.Lerp (midayColorOfDay, middayFullSnow, SnowManager.instance.snowLevel);
		midday = SetDarkness (middayAfterSnow, darkness);
	}

	public float minDarkness, maxDarkness;
	public Color SetDarkness(Color color, float? darkness = null) {
		
		float _darkness = darkness ?? Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		color.r = Mathf.Clamp01(color.r - _darkness);
		color.b = Mathf.Clamp01(color.b - _darkness);
		color.g = Mathf.Clamp01(color.g - _darkness);
		return color;
	}
}
