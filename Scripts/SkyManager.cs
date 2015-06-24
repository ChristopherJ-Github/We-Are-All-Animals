using UnityEngine;
using Tools;
using System.Collections;

public class SkyManager : Singleton<SkyManager>{
	
	public SunProperties sun;
	public MoonProperties moon;
	public float sunriseAngle, sunsetAngle;
	public Gradient sunNightToDusk;
	[HideInInspector]
	public float intensityLerp;
	
	public Material SkyBoxMaterial1;
	public Material SkyBoxMaterial2;
	
	public GameObject Water;
	public bool IncludeWater = false;
	public Color WaterNight;
	public Color WaterDay;
	
	private float sunriseAstroTime, sunriseTime, sunsetTime, sunsetAstroTime;
	[HideInInspector]
	public float sunrisePosInDay, sunsetPosInDay;
	[HideInInspector]
	public float nightDayLerp;
	public Gradient sunMiddayTint;

	void OnEnable () { 
		
		SceneManager.instance.OnNewDay += UpdatePhaseTimes;
	}
	
	void Start () {

		UpdatePhaseTimes ();
	}
	
	void UpdatePhaseTimes () {
		
		sunriseAstroTime = SunControl.instance.sunriseAstroTime;
		sunriseTime = SunControl.instance.sunriseTime;
		sunsetTime = SunControl.instance.sunsetTime;
		sunsetAstroTime = SunControl.instance.sunsetAstroTime;
		double minsAtSunise = sunriseTime * 60 + SceneManager.minsAtDayStart;
		float sunrisePos = (float)(minsAtSunise / SceneManager.minsInYear);
		sunrisePosInDay = SunControl.instance.dayCurve.Evaluate (sunrisePos);
		double minsAtSunset = sunsetTime * 60 + SceneManager.minsAtDayStart;
		float sunsetPos = (float)(minsAtSunset / SceneManager.minsInYear);
		sunsetPosInDay = SunControl.instance.dayCurve.Evaluate (sunsetPos);
	}
	
	void Update () {
		
		ApplyPhaseChanges ();
		AdjustSunAndMoon ();
	}
	
	void ApplyPhaseChanges () {
		
		float time = SceneManager.curvePosDay * 24;
		//sun.color = Color.Lerp (SunNight, SunDay, (time/24) * 2);
		if (time > sunsetAstroTime || time < sunriseAstroTime)
			SetNightSettings(time);
		if (time > sunriseAstroTime && time < sunriseTime)
			SetDuskSettings(time);
		if (time > sunriseTime && time < sunriseTime + 2) 
			SetDuskToMidaySettings(time);
		if (time > sunriseTime + 2 && time < sunsetTime - 2)
			SetMiddaySettings(time);
		if (time > sunsetTime - 2 && time < sunsetTime) 
			SetMiddayToDawnSettings(time);
		if (time > sunsetTime && time < sunsetAstroTime)
			SetDawnSettings(time);
	}

	void SetNightSettings (float time) {

		RenderSettings.skybox = SkyBoxMaterial1;
		RenderSettings.skybox.SetFloat("_Blend", 0);
		Color nightTint = CloudControl.instance.nightToDusk.Evaluate(0);
		SkyBoxMaterial1.SetColor ("_Tint", nightTint);
		RenderSettings.ambientLight = AmbientLightingChanger.instance.night;
		RenderSettings.fogColor = FogControl.instance.NightToDusk(0);	
		nightDayLerp = 0;
		sun.light.intensity = 0;
		moon.light.intensity = moon.currentIntesity;
	}

	void SetDuskSettings (float time) {

		float lerp = Mathf.InverseLerp (sunriseAstroTime, sunriseTime, time);
		RenderSettings.skybox = SkyBoxMaterial1;
		RenderSettings.skybox.SetFloat("_Blend", lerp);
		SkyBoxMaterial1.SetColor ("_Tint", CloudControl.instance.NightToDusk(lerp));
		RenderSettings.ambientLight = Color.Lerp (AmbientLightingChanger.instance.night, AmbientLightingChanger.instance.dusk, lerp);
		RenderSettings.fogColor = FogControl.instance.NightToDusk(lerp);
		nightDayLerp = Mathf.InverseLerp(sunriseAstroTime, sunriseTime + 2, time);
		sun.light.color = sunNightToDusk.Evaluate(lerp);
		sun.light.intensity = Mathf.Lerp(0, sun.currentIntesity, nightDayLerp);
		moon.light.intensity = Mathf.Lerp(moon.currentIntesity, 0, lerp);
	}

	void SetDuskToMidaySettings (float time) {

		float lerp = Mathf.InverseLerp (sunriseTime, sunriseTime + 2, time);
		RenderSettings.skybox = SkyBoxMaterial2;
		RenderSettings.skybox.SetFloat("_Blend", lerp);
		Color duskTint = CloudControl.instance.NightToDusk(1);
		SkyBoxMaterial2.SetColor ("_Tint", Color.Lerp (duskTint, CloudControl.instance.midday, lerp));
		RenderSettings.ambientLight = Color.Lerp (AmbientLightingChanger.instance.dusk, AmbientLightingChanger.instance.midday, lerp);
		Color duskFog = FogControl.instance.NightToDusk(1);
		RenderSettings.fogColor = Color.Lerp(duskFog, FogControl.instance.midday, lerp);
		nightDayLerp = Mathf.InverseLerp(sunriseAstroTime, sunriseTime + 2, time);
		Color sunMidday = sun.GetMiddayColor();
		Color sunDusk = sunNightToDusk.Evaluate(1);
		sun.light.color = Color.Lerp (sunDusk, sunMidday, lerp);
		sun.light.intensity = Mathf.Lerp(0, sun.currentIntesity, nightDayLerp);
		moon.light.intensity = 0;
	}

	void SetMiddaySettings (float time) {

		RenderSettings.skybox = SkyBoxMaterial2;
		RenderSettings.skybox.SetFloat("_Blend", 1);
		SkyBoxMaterial2.SetColor ("_Tint", CloudControl.instance.midday);
		RenderSettings.ambientLight = AmbientLightingChanger.instance.midday;
		RenderSettings.fogColor = FogControl.instance.midday;
		Color sunMidday = sun.GetMiddayColor();
		nightDayLerp = 1;
		sun.light.color = sunMidday;
		sun.light.intensity = sun.currentIntesity;
		moon.light.intensity = 0;
	}

	void SetMiddayToDawnSettings (float time) {

		float lerp = Mathf.InverseLerp (sunsetTime, sunsetTime - 2, time);
		RenderSettings.skybox = SkyBoxMaterial2;
		RenderSettings.skybox.SetFloat("_Blend", lerp);
		Color duskTint = CloudControl.instance.NightToDusk(1);
		SkyBoxMaterial2.SetColor ("_Tint", Color.Lerp (duskTint, CloudControl.instance.midday, lerp));
		RenderSettings.ambientLight = Color.Lerp (AmbientLightingChanger.instance.dusk, AmbientLightingChanger.instance.midday, lerp);
		Color duskFog = FogControl.instance.NightToDusk(1);
		RenderSettings.fogColor = Color.Lerp(duskFog, FogControl.instance.midday, lerp);
		nightDayLerp = Mathf.InverseLerp(sunsetAstroTime, sunsetTime - 2, time);
		Color sunMidday = sun.GetMiddayColor();
		Color sunDusk = sunNightToDusk.Evaluate(1);
		sun.light.color = Color.Lerp (sunDusk, sunMidday, lerp);
		sun.light.intensity = Mathf.Lerp(0, sun.currentIntesity, lerp);
		moon.light.intensity = 0;
	}

	void SetDawnSettings (float time) {

		float lerp = Mathf.InverseLerp (sunsetAstroTime, sunsetTime, time);
		RenderSettings.skybox = SkyBoxMaterial1;
		RenderSettings.skybox.SetFloat("_Blend", lerp);
		SkyBoxMaterial1.SetColor ("_Tint", CloudControl.instance.NightToDusk(lerp));
		RenderSettings.ambientLight = Color.Lerp (AmbientLightingChanger.instance.night, AmbientLightingChanger.instance.dusk, lerp);
		RenderSettings.fogColor = FogControl.instance.NightToDusk(lerp);
		nightDayLerp = Mathf.InverseLerp(sunsetAstroTime, sunsetTime - 2, time);
		sun.light.color = sunNightToDusk.Evaluate(lerp);
		sun.light.intensity = 0;
		moon.light.intensity = Mathf.Lerp(moon.currentIntesity, 0, lerp);
	}

	void AdjustSunAndMoon () {

		//sun.light.intensity = Mathf.Lerp(0, sun.currentIntesity, nightDayLerp);
		//moon.light.intensity = Mathf.Lerp(moon.currentIntesity, 0, nightDayLerp);
		float currentIntensity = sun.light.intensity + moon.light.intensity;
		intensityLerp = Mathf.InverseLerp(0, sun.maxIntensity, currentIntensity);
		float posInDay = Mathf.Clamp01 (SunControl.instance.posInDay);
		float sunAngle = Math.Convert (posInDay, sunrisePosInDay, sunsetPosInDay, sunriseAngle, sunsetAngle);
		sun.transform.localEulerAngles = new Vector3(sunAngle, 0, 0);
		float posInNight = Mathf.Clamp01 (SunControl.instance.posInNight);
		float moonAngle = Math.Convert (posInNight, 0, 1, sunriseAngle, sunsetAngle);
		moon.transform.localEulerAngles = new Vector3 (moonAngle, 0, 0);
	}
}
