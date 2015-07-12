﻿using UnityEngine;
using System.Collections;

public class CloudControl : Singleton<CloudControl> {
	
	void OnEnable () {
		
		SceneManager.instance.OnNewDay += UpdateClouds;
		UpdateClouds ();
		SetCloudProperties ();
		StartCoroutine(ChangeExtraOvercast());
	}
	
	void UpdateClouds () {
		
		RandomizeOvercast ();
		RandomizeCloudHeight ();
		RandomizeSkyboxTint (Random.value);
	}

	public AnimationCurve maxCloudinessOverYear;
	public AnimationCurve minCloudinessOverYear;
	public AnimationCurve likelyCloudinessOverYear;
	public AnimationCurve likelyInfluence;
	private float initOvercast; 
	void RandomizeOvercast () {

		float minCloudiness = minCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float maxCloudiness = maxCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float randomCloudiness = Random.Range(minCloudiness, maxCloudiness);
		float likelyCloudiness = likelyCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float influence = likelyInfluence.Evaluate (Random.value);
		initOvercast = Mathf.Lerp (randomCloudiness, likelyCloudiness, influence);
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
	public void SetOvercast(float overcast) {

		Debug.Log ("overcast: " + overcast);
		setOvercastCalled = true;
		this.overcast = SkyManager.instance.sunsetProgress > 0 ? GetNightOvercast (overcast) : overcast;
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
	private float initMiddayValue;
	[HideInInspector] public float middayValue;
	[HideInInspector] public Color midday;
	public Gradient _midday;
	public Gradient nightToDusk;
	void RandomizeSkyboxTint (float tintValue) {

		float maxRandomValue = overcastToRandomization.Evaluate (_overcast);
		middayValue = Mathf.Lerp(0, maxRandomValue, tintValue);
		initMiddayValue = middayValue;
		midday = _midday.Evaluate (middayValue);
	}

	public float intensity = 4;
	public float shadowScale = 0.75f;
	public float distScale = 10.0f;
	void SetCloudProperties () {
		
		Shader.SetGlobalFloat("ls_cloudintensity", intensity);
		Shader.SetGlobalFloat("ls_shadowscale", shadowScale);
		Shader.SetGlobalVector("ls_cloudcolor", (new Vector3(1,0.9f,0.95f)));
		Shader.SetGlobalFloat("ls_distScale", distScale);
	}
	
	void Update () {

		SetCloudSpeed ();
		if (setOvercastCalled) {
			setOvercastCalled = false;
		} else {
			SetOvercast (initOvercast);
		}
	}

	public float minSpeed, maxSpeed;
	void SetCloudSpeed () {
		
		float speed = Mathf.Lerp (minSpeed, maxSpeed, WindControl.instance.windiness);
		Shader.SetGlobalFloat("ls_time", Time.time * speed * 0.25f);
	}

	[HideInInspector] public float grayAmount;
	public AnimationCurve overcastToDarkening;
	public float testValue;
	public void SetStormTint (float grayAmount, float darkness) {

		grayAmount *= testValue;
		this.grayAmount = grayAmount;
		Color initMidday = _midday.Evaluate (initMiddayValue);
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
