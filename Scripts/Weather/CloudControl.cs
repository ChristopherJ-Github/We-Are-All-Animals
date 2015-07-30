using UnityEngine;
using System.Collections;

public class CloudControl : Singleton<CloudControl> {
	
	void OnEnable () {
		
		SceneManager.instance.OnNewDay += UpdateClouds;
		lightning = false;
		UpdateClouds ();
		SetCloudProperties ();
		StartCoroutine(ChangeExtraOvercast());
	}
	
	void UpdateClouds () {
		
		RandomizeOvercast ();
		RandomizeCloudHeight ();
		RandomizeSkyboxTint ();
	}

	public AnimationCurve maxCloudinessOverYear;
	public AnimationCurve minCloudinessOverYear;
	public AnimationCurve likelyCloudinessOverYear;
	public AnimationCurve likelyInfluence;
	private float initOvercast; 
	void RandomizeOvercast () {

		float likelyCloudiness = likelyCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float influence = likelyInfluence.Evaluate (Random.value);
		influence = Mathf.Lerp (influence, 0, SnowManager.instance.snowLevel);
		float minCloudiness = minCloudinessOverYear.Evaluate (SceneManager.curvePos);
		minCloudiness = Mathf.Lerp (minCloudiness, 0, SnowManager.instance.snowLevel);
		float maxCloudiness = maxCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float randomCloudiness = Mathf.Lerp(minCloudiness, maxCloudiness, Random.value);
		float overcast = Mathf.Lerp (randomCloudiness, likelyCloudiness, influence);
		initOvercast = Mathf.Clamp (overcast, minCloudiness, maxCloudiness);
		SetOvercast (initOvercast);
	}

	private float _overcast;
	public float overcast {
		get { return _overcast; }
		set { _overcast = Mathf.Clamp01(value);}
	}
	public float minScattering, maxScattering;
	public float minSharpness, maxSharpness;
	public float minThickness, maxThickness;
	private bool setOvercastCalled;
	public void SetOvercast(float overcast, bool overrideInit = false) {

		setOvercastCalled = true;
		this.overcast = SkyManager.instance.sunsetProgress > 0 ? GetNightOvercast (overcast) : overcast;
		initOvercast = overrideInit ? _overcast : initOvercast;
		float scattering = Mathf.Lerp (minScattering, maxScattering, _overcast);
		float sharpness = Mathf.Lerp (minSharpness, maxSharpness, _overcast);
		float thickness = Mathf.Lerp (minThickness, maxThickness, _overcast);
		Shader.SetGlobalFloat("ls_cloudscattering", scattering);
		Shader.SetGlobalFloat("ls_cloudsharpness", sharpness);
		Shader.SetGlobalFloat("ls_cloudthickness", thickness);
		this.overcast = overcast;
	}

	public float nightOvercast;
	float GetNightOvercast (float overcast) {

		float currentOvercast = Mathf.Lerp (nightOvercast, overcast, SkyManager.instance.nightDayLerp);
		return currentOvercast;
	}
	
	public float minHeight, maxHeight;
	void RandomizeCloudHeight () {

		float height = Random.Range (minHeight, maxHeight);
		Shader.SetGlobalFloat ("ls_cloudscale", height);
	}

	public AnimationCurve overcastToRandomization;
	void RandomizeSkyboxTint () {

		float maxRandomValue = overcastToRandomization.Evaluate (_overcast);
		float tintValue = Mathf.Lerp(0, maxRandomValue, Random.value);
		SetSkyboxTint (tintValue);
	}

	[HideInInspector] public float middayValue;
	[HideInInspector] public Color midday;
	public Gradient _midday;
	public Gradient nightToDusk;
	void SetSkyboxTint (float tintValue) {

		middayValue = tintValue;
		midday = _midday.Evaluate (middayValue);
	}

	public float intensity = 4;
	public float shadowScale = 0.75f;
	public float distScale = 10.0f;
	public Color cloudColor;
	void SetCloudProperties () {
		
		Shader.SetGlobalFloat("ls_cloudintensity", intensity);
		Shader.SetGlobalFloat("ls_shadowscale", shadowScale);
		Shader.SetGlobalFloat("ls_distScale", distScale);
		SetCloudColor (cloudColor);
	}
	
	public void SetCloudColor (Color color) {

		Shader.SetGlobalColor("ls_cloudcolor", color);
	}

	public Color col;
	void Update () {
	
		SetCloudSpeed ();
		if (setOvercastCalled) {
			setOvercastCalled = false;
		} else {
			SetOvercast (initOvercast);
		}
		SetCurrnetCloudColor ();
	}

	public float minSpeed, maxSpeed;
	void SetCloudSpeed () {
		
		float speed = Mathf.Lerp (minSpeed, maxSpeed, WindControl.instance.windiness);
		Shader.SetGlobalFloat("ls_time", Time.time * speed * 0.25f);
	}

	private bool lightning;
	public float lightningDarkness { get { return lightning ? darkness : 0; } }

	public Color lightningCloudColor;
	public float nightDarkening;
	void SetCurrnetCloudColor () {

		Color lightningColor = Color.Lerp(cloudColor, lightningCloudColor, lightningDarkness);
		float nightInfluence = SkyManager.instance.nightTransition * (1 - nightDarkening);
		Color nightColor = Color.Lerp(lightningColor, Color.black, nightInfluence);
		SetCloudColor(nightColor);
	}
	
	[HideInInspector] public float grayAmount, darkness;
	public AnimationCurve overcastToDarkening;
	public void SetStormTint (float grayAmount, float darkness, bool lightning) {

		this.grayAmount = grayAmount;
		this.darkness = darkness;
		this.lightning = lightning;
		Color initMidday = _midday.Evaluate (middayValue);
		Color middayGrayscale = new Color (initMidday.grayscale, initMidday.grayscale, initMidday.grayscale);
		Color middayAfterGray = Color.Lerp(initMidday, middayGrayscale, grayAmount);
		float overcastInfluence = overcastToDarkening.Evaluate (_overcast);
		Color middayDarkened = Color.Lerp (middayAfterGray, Color.black, darkness * overcastInfluence);
		midday = middayDarkened;
	}

	public Color NightToDusk (float lerp) {

		Color initColor = nightToDusk.Evaluate (lerp);
		Color grayscale = new Color (initColor.grayscale, initColor.grayscale, initColor.grayscale);
		Color afterStorm = Color.Lerp (initColor, grayscale, grayAmount);
		return afterStorm;
	}

	public float minDelaySpeed, maxDelaySpeed;
	public float minExtraOvercast, maxExtraOvercast;
	public float snowInfluence;
	IEnumerator ChangeExtraOvercast () {
		
		float timer = 1;
		while (timer > 0) {	
			float speed = Mathf.Lerp(minDelaySpeed, maxDelaySpeed, WindControl.instance.windiness);
			timer -= Time.deltaTime * speed;
			yield return null;
		}
		float _extraOvercast = Mathf.Lerp (minExtraOvercast, maxExtraOvercast, 1 - _overcast);
		float extraOvercastGoal = Random.Range (-_extraOvercast, _extraOvercast) * (1 - SnowManager.instance.snowLevel * snowInfluence);
		StartCoroutine (SetExtraOvercast (extraOvercastGoal));
	}

	[HideInInspector] public float extraOvercast;
	public float changeSpeed;
	IEnumerator SetExtraOvercast (float extraOvercastGoal) {

		while (extraOvercast != extraOvercastGoal) {
			extraOvercast = Mathf.MoveTowards(extraOvercast, extraOvercastGoal, Time.deltaTime * changeSpeed);
			yield return null;
		}
		StartCoroutine(ChangeExtraOvercast());
	}
}
