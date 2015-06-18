using UnityEngine;
using System.Collections;

public class AmbientLightingChanger : Singleton <AmbientLightingChanger> {
	
	public Color _night;
	[HideInInspector] 
	public Color night;
	public Color _dusk;
	[HideInInspector] 
	public Color dusk;
	public Gradient middayOverYear;
	[HideInInspector]
	public Color midday;
	private Color midayColorOfDay;
	public float minDarkness, maxDarkness;


	void Start () {

		SceneManager.instance.OnNewDay += dayUpdate;
		dayUpdate ();
	}
	
	void dayUpdate () {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
	}

	void Update () {

		midayColorOfDay = middayOverYear.Evaluate (SceneManager.curvePos);
		float darkness = Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		night = SetDarkness (_night, darkness);
		dusk = SetDarkness (_dusk, darkness);
		midday = SetDarkness (midayColorOfDay, darkness);
	}

	public Color SetDarkness(Color color, float? darkness = null) {
		
		float _darkness = darkness ?? Mathf.Lerp (maxDarkness, minDarkness, SkyManager.instance.intensityLerp);
		color.r = Mathf.Clamp01(color.r - _darkness);
		color.b = Mathf.Clamp01(color.b - _darkness);
		color.g = Mathf.Clamp01(color.g - _darkness);
		return color;
	}
}
