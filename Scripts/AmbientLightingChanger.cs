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

	public float minDarkness, maxDarkness;
	void UpdateAmbientLight () {

		float darkness = Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		SetNightAmbience (darkness);
		SetDuskAmbience (darkness);
		SetMiddayAmbience (darkness);
	}
	
	public Color _night;
	[HideInInspector] public Color night;
	void SetNightAmbience (float darkness) {

		night = SetDarkness (_night, darkness);
	}

	public Color _dusk;
	[HideInInspector] public Color dusk;
	void SetDuskAmbience (float darkness) {

		dusk = SetDarkness (_dusk, darkness);
	}

	public Gradient middayOverYear;
	public Gradient _midday;
	[HideInInspector] public Color midday;
	void SetMiddayAmbience (float darkness) {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
		Color middayFullSnow = _midday.Evaluate (CloudControl.instance.middayValue);
		Color middayAfterSnow = Color.Lerp (midayColorOfDay, middayFullSnow, SnowManager.instance.snowLevel);
		midday = SetDarkness (middayAfterSnow, darkness);
	}
	
	public Color SetDarkness(Color color, float darkness) {

		color.r = Mathf.Clamp01(color.r - darkness);
		color.b = Mathf.Clamp01(color.b - darkness);
		color.g = Mathf.Clamp01(color.g - darkness);
		return color;
	}
}
