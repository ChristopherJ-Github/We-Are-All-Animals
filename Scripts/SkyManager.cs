using UnityEngine;
using Tools;
using System.Collections;

public class SkyManager : Singleton<SkyManager> {

	void OnEnable () { 
		
		SceneManager.instance.OnNewDay += UpdatePhaseTimes;
	}
	
	void Start () {

		UpdatePhaseTimes ();
	}

	private float sunriseAstroTime, sunriseTime, sunsetTime, sunsetAstroTime;
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

	[HideInInspector] public float nightDayLerp, sunsetProgress, sunriseProgress;
	void SetNightSettings (float time) {

		nightDayLerp = 0;
		sunriseProgress = time > 12 && time < 24 ? 1 : 0;
		sunsetProgress = time > 12 && time < 24 ? 1 : 0;
		SetSkyBox (1, 0);
		SetColors (0, 0);
		SetIntensity (1, 0);
		DarkenSky (1);
	}
	
	void SetDuskSettings (float time) {

		float lerp = Mathf.InverseLerp (sunriseAstroTime, sunriseTime, time);
		nightDayLerp = Mathf.InverseLerp(sunriseAstroTime, sunriseTime + 2, time);
		sunriseProgress = nightDayLerp;
		sunsetProgress = 0;
		SetSkyBox (1, lerp);
		SetColors (lerp, 0);
		SetIntensity (1 - lerp);
		DarkenSky (1);
	}
	
	void SetDuskToMidaySettings (float time) {

		float lerp = Mathf.InverseLerp (sunriseTime, sunriseTime + 2, time);
		nightDayLerp = Mathf.InverseLerp(sunriseAstroTime, sunriseTime + 2, time);
		sunriseProgress = nightDayLerp;
		sunsetProgress = 0;
		SetSkyBox (2, lerp);
		SetColors (1, lerp);
		SetIntensity (0);
		DarkenSky (1 - lerp);
	}

	void SetMiddaySettings (float time) {

		nightDayLerp = 1;
		sunriseProgress = 1;
		sunsetProgress = 0;
		SetSkyBox (2, 1);
		SetColors (0, 1);
		SetIntensity (0, 1);
		DarkenSky (0);
		Debug.Log ("midday");
	}

	void SetMiddayToDawnSettings (float time) {

		float lerp = Mathf.InverseLerp (sunsetTime, sunsetTime - 2, time);
		nightDayLerp = Mathf.InverseLerp(sunsetAstroTime, sunsetTime - 2, time);
		sunriseProgress = 1;
		sunsetProgress = 1 - nightDayLerp;
		SetSkyBox (2, lerp);
		SetColors (1, lerp);
		SetIntensity (0, lerp);
		DarkenSky (1 - lerp);
		Debug.Log ("midday to dawn");
	}

	void SetDawnSettings (float time) {

		float lerp = Mathf.InverseLerp (sunsetAstroTime, sunsetTime, time);
		nightDayLerp = Mathf.InverseLerp(sunsetAstroTime, sunsetTime - 2, time);
		sunriseProgress = 1;
		sunsetProgress = 1 - nightDayLerp;
		SetSkyBox (1, lerp);
		SetColors (lerp, 0);
		SetIntensity (1 - lerp, 0);
		DarkenSky (1);
		Debug.Log ("dawn");
	}

	public SunProperties sun;
	public MoonProperties moon;
	[HideInInspector] public float intensityLerp;
	[HideInInspector] public float sunrisePosInDay, sunsetPosInDay;
	public float sunriseAngle, sunsetAngle;
	void AdjustSunAndMoon () {

		float currentIntensity = sun.light.intensity + moon.light.intensity;
		intensityLerp = Mathf.InverseLerp(0, sun.maxIntensity, currentIntensity);
		float posInDay = Mathf.Clamp01 (SunControl.instance.posInDay);
		float sunAngle = Math.Convert (posInDay, sunrisePosInDay, sunsetPosInDay, sunriseAngle, sunsetAngle);
		sun.transform.localEulerAngles = new Vector3(sunAngle, 0, 0);
		float posInNight = Mathf.Clamp01 (SunControl.instance.posInNight);
		float moonAngle = Math.Convert (posInNight, 0, 1, sunriseAngle, sunsetAngle);
		moon.transform.localEulerAngles = new Vector3 (moonAngle, 0, 0);
	}

	public AnimationCurve daytimeToIntensity;
	void SetIntensity (float moonLerp, float? sunLerp = null) {
		
		moon.light.intensity = Mathf.Lerp(0, moon.currentIntesity, moonLerp);
		if (sunLerp == null) {
			float daytimeInfluence = daytimeToIntensity.Evaluate (nightDayLerp);
			sun.light.intensity = Tools.Math.Convert (daytimeInfluence,0, 1, 0, sun.currentIntesity);
		} else {
			sun.light.intensity = Mathf.Lerp(0, sun.currentIntesity, (float)sunLerp);
		}
	}

	public Material SkyBoxMaterial1;
	public Material SkyBoxMaterial2;
	void SetSkyBox (int index, float blend) {
		
		RenderSettings.skybox = index == 1 ? SkyBoxMaterial1 : SkyBoxMaterial2;
		RenderSettings.skybox.SetFloat("_Blend", blend);
	}

	void DarkenSky (float skyDarkness) {
		
		Color skyColor = new Color (0, 0, 0, skyDarkness * CloudControl.instance.overcast);
		RenderSettings.skybox.SetColor ("_SnowColor", skyColor);
	}

	void SetColors (float nightToDuskValue, float middayValue) {

		float nightToDuskSaturation = 1 - CloudControl.instance.overcast * WeatherControl.instance.severity;
		SetSkyboxTint (nightToDuskValue, middayValue, nightToDuskSaturation);
		SetAmbientLightColor (nightToDuskValue, middayValue, nightToDuskSaturation);
		SetFogColor (nightToDuskValue, middayValue, nightToDuskSaturation);
		SetSunColor (nightToDuskValue, middayValue, nightToDuskSaturation);
	}

	void SetSkyboxTint (float nightToDuskValue, float middayValue, float nightToDuskSaturation = 1, float middaySaturation = 1) {
		
		Color colorAroundNight = CloudControl.instance.NightToDusk(nightToDuskValue);
		Color colorAroundNightDesat = Desaturate (colorAroundNight, nightToDuskSaturation);
		Color colorAtMidday = CloudControl.instance.midday;
		Color colorAtMiddayDesat = Desaturate (colorAtMidday, middaySaturation);
		Color currentColor = Color.Lerp (colorAroundNightDesat, colorAtMiddayDesat, middayValue);
		RenderSettings.skybox.SetColor ("_Tint", currentColor);
	}

	public Gradient sunNightToDusk;
	public Gradient sunMiddayTint;
	void SetSunColor (float nightToDuskValue, float middayValue, float nightToDuskSaturation = 1, float middaySaturation = 1) {

		Color colorAroundNight = sunNightToDusk.Evaluate(nightToDuskValue);
		Color colorAroundNightDesat = Desaturate (colorAroundNight, nightToDuskSaturation);
		Color colorAtMidday = sun.GetMiddayColor ();
		Color colorAtMiddayDesat = Desaturate (colorAtMidday, middaySaturation);
		Color currentColor = Color.Lerp (colorAroundNightDesat, colorAtMiddayDesat, middayValue);
		sun.light.color = currentColor;
	}

	void SetAmbientLightColor (float nightToDuskValue, float middayValue, float nightToDuskSaturation = 1, float middaySaturation = 1) {
		
		Color colorAroundNight = AmbientLightingChanger.instance.NightToDusk (nightToDuskValue);
		Color colorAroundNightDesat = Desaturate (colorAroundNight, nightToDuskSaturation);
		Color colorAtMidday = AmbientLightingChanger.instance.midday;
		Color colorAtMiddayDesat = Desaturate (colorAtMidday, middaySaturation);
		Color currentColor = Color.Lerp (colorAroundNightDesat, colorAtMiddayDesat, middayValue);
		RenderSettings.ambientLight = currentColor;
	}

	void SetFogColor (float nightToDuskValue, float middayValue, float nightToDuskSaturation = 1, float middaySaturation = 1) {
		
		Color colorAroundNight = FogControl.instance.NightToDusk (nightToDuskValue);
		Color colorAroundNightDesat = Desaturate (colorAroundNight, nightToDuskSaturation);
		Color colorAtMidday = FogControl.instance.midday;
		Color colorAtMiddayDesat = Desaturate (colorAtMidday, middaySaturation);
		Color currentColor = Color.Lerp (colorAroundNightDesat, colorAtMiddayDesat, middayValue);
		RenderSettings.fogColor = currentColor;
	}

	Color Desaturate (Color color, float saturation) {

		Color desaturatedColor = new Color (color.grayscale, color.grayscale, color.grayscale);
		Color newColor = Color.Lerp(desaturatedColor, color, saturation); 
		return newColor;
	}
}
