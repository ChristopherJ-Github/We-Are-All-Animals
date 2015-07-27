using UnityEngine;
using System.Collections;

public class FogControl : Singleton<FogControl> {

	void Start () {

		RenderSettings.fog = true;
		SceneManager.instance.OnNewDay += RandomizeFog;
		RandomizeFog ();
	}

	void RandomizeFog () {

		SetFog (Random.value);
	}

	public AnimationCurve minFogOverYear, maxFogOverYear;
	public float minDesnity, maxDensity;
	void SetFog (float fogAmount) {

		float minFog = minFogOverYear.Evaluate (SceneManager.curvePos);
		float maxFog = maxFogOverYear.Evaluate (SceneManager.curvePos);
		float fogDensity = Mathf.Lerp(minFog, maxFog, fogAmount);
		SetFogDesnity(Mathf.Lerp (minDesnity, maxDensity, fogDensity));
		SetGlobalFog (false);
	}
	
	public void SetFogDesnity (float density) {
		
		RenderSettings.fogDensity = density;
	}

	void Update () {

		SetMidayColor ();
		UpdateLightShaftBrightness ();
	}

	public Gradient _midday;
	public AnimationCurve overcastInfluence;
	public Color middayCloudy;
	public Color middayStorm;
	public Color middaySnow;
	public Color middayLightning;
	[HideInInspector] public Color midday;
	void SetMidayColor () {

		Color initMidday = _midday.Evaluate (CloudControl.instance.middayValue);
		float _overcastInfluence = overcastInfluence.Evaluate(CloudControl.instance.overcast); 
		Color middayAfterCloud = Color.Lerp (initMidday, middayCloudy, _overcastInfluence);
		Color middayAfterSnow = Color.Lerp (middayAfterCloud, middaySnow, SnowManager.instance.snowLevel * _overcastInfluence);
		float stormTinting = CloudControl.instance.grayAmount * (1 - SnowManager.instance.snowLevel);
		Color middayAfterStorm = Color.Lerp (middayAfterSnow, middayStorm, stormTinting);
		Color middayAfterLightning = Color.Lerp (middayAfterStorm, middayLightning, CloudControl.instance.lightningDarkness);
		Color middayAfterCloudDarkening = AddCloudDarkening (middayAfterLightning);
		midday = middayAfterCloudDarkening;
	}
	
	public AnimationCurve brightnessOverYear;
	public float maxDailyBrightnessAmount;
	[HideInInspector] public float dailyBrightnessNorm;
	void UpdateLightShaftBrightness () {

		dailyBrightnessNorm = brightnessOverYear.Evaluate (SceneManager.curvePos);
		brightnessAmount = Mathf.Lerp (0, maxDailyBrightnessAmount, dailyBrightnessNorm); 
		SetLightShaftBrightness (brightnessAmount);
	}

	public LightShafts lightShafts;
	[HideInInspector] public float brightnessAmount;
	public float maxBrightness;
	public void SetLightShaftBrightness (float brightnessAmount) {

		this.brightnessAmount = brightnessAmount;
		lightShafts.enabled = brightnessAmount > 0;
		float brightness = Mathf.Lerp (0, maxBrightness, brightnessAmount);
		float brightnessDarkened = Mathf.Lerp (0, brightness, SkyManager.instance.nightDayLerp);
		lightShafts.m_Brightness = brightnessDarkened;
	}

	public AnimationCurve daytimeToDarkeningRain, daytimeToDarkeningSnow;
	Color AddCloudDarkening (Color midday) {

		float darkening = 0;
		if (SkyManager.instance.sunsetProgress > 0 && WeatherControl.currentWeather != null) {
			if (WeatherControl.currentWeather.weather.name == "Rain") {
				darkening = 1 - daytimeToDarkeningRain.Evaluate(SkyManager.instance.nightDayLerp);
			}
			if (WeatherControl.currentWeather.weather.name == "Snow") {
				darkening = 1 - daytimeToDarkeningSnow.Evaluate(SkyManager.instance.nightDayLerp);
			}
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
