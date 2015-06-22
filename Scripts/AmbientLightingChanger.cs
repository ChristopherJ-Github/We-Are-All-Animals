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
		SetTodaysMiddayColor ();//debug
	}

	public Color _night;
	[HideInInspector] public Color night;
	public Color _dusk;
	[HideInInspector] public Color dusk;
	public Gradient middayOverYear;
	[HideInInspector] public Color midday;
	public Gradient _midday;
	void UpdateAmbientLight () {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
		float darkness = Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		night = SetDarkness (_night, darkness);
		dusk = SetDarkness (_dusk, darkness);
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
