using UnityEngine;
using System.Collections;

public class FogControl : Singleton<FogControl> {

	void Start () {
		
		RenderSettings.fog = true;
		SceneManager.instance.OnNewDay += RandomizeFog;
		RandomizeFog ();
	}

	public AnimationCurve minFogOverYear, maxFogOverYear;
	public float minDesnity, maxDensity;
	void RandomizeFog () {
		
		float minFog = minFogOverYear.Evaluate (SceneManager.curvePos);
		float maxFog = maxFogOverYear.Evaluate (SceneManager.curvePos);
		float fogDensity = Random.Range (minFog, maxFog);
		SetFogDesnity(Mathf.Lerp (minDesnity, maxDensity, fogDensity));
		SetGlobalFog (false);
	}
	
	public void SetFogDesnity (float density) {
		
		RenderSettings.fogDensity = density;
	}

	void Update () {
		
		SetMidayColor ();
	}

	public Gradient _midday;
	public AnimationCurve overcastInfluence;
	public Color middayCloudy;
	public Color middayStorm;
	public Color middaySnow;
	[HideInInspector] public Color midday;
	public AnimationCurve daytimeToCloudDarkening;
	void SetMidayColor () {

		Color initMidday = _midday.Evaluate (CloudControl.instance.middayValue);
		float _overcastInfluence = overcastInfluence.Evaluate(CloudControl.instance.overcast); 
		Color middayAfterCloud = Color.Lerp (initMidday, middayCloudy, _overcastInfluence);
		Color middayAfterSnow = Color.Lerp (middayAfterCloud, middaySnow, SnowManager.instance.snowLevel * _overcastInfluence);
		float stormTinting = CloudControl.instance.grayAmount * (1 - SnowManager.instance.snowLevel);
		Color middayAfterStorm = Color.Lerp (middayAfterSnow, middayStorm, stormTinting);
		Color middayAfterCloudDarkening = AddCloudDarkening (middayAfterStorm);
		midday = middayAfterCloudDarkening;
	}

	Color AddCloudDarkening (Color midday) {

		float darkening = 0;
		if (SkyManager.instance.sunsetProgress > 0) {
			darkening = 1 - daytimeToCloudDarkening.Evaluate(SkyManager.instance.nightDayLerp);
		}
		Color middayAfterCloudDarkening = Color.Lerp (midday, Color.black, darkening * CloudControl.instance.grayAmount);
		return middayAfterCloudDarkening;
	}

	public Gradient nightToDusk;
	public Color NightToDusk (float lerp) {
		
		Color initColor = nightToDusk.Evaluate (lerp);
		Color grayscale = new Color (initColor.grayscale, initColor.grayscale, initColor.grayscale);
		Color afterStorm = Color.Lerp (initColor, grayscale, CloudControl.instance.grayAmount);
		Color darkened = Color.Lerp (Color.black, afterStorm, SkyManager.instance.nightDayLerp);
		return darkened;
	}

	public GameObject globalFogActivator;
	public void SetGlobalFog (bool active) {
		
		globalFogActivator.SetActive (active);
	}
}
