using UnityEngine;
using System.Collections;

public class CloudControl : Singleton<CloudControl> {
	
	public AnimationCurve maxCloudinessOverYear;
	public AnimationCurve minCloudinessOverYear;
	public AnimationCurve likelyCloudinessOverYear;
	public AnimationCurve likelyInfluence;
	public float maxOvercast;
	[HideInInspector] public float overcast; 
	private float initOvercast; 
	public float minHeight, maxHeight;
	public float minSpeed, maxSpeed;
	private float speed;

	public float intensity = 4;
	public float shadowScale = 0.75f;
	public float distScale = 10.0f;
	
	public float minScattering, maxScattering;
	public float minSharpness, maxSharpness;
	public float minThickness, maxThickness;
	public Gradient nightToDuskTint;
	public Gradient _middayTint;
	public AnimationCurve overcastToRandomization;
	[HideInInspector]
	public Color middayTint;
	[HideInInspector]
	public float middayLerp, grayAmount;
	private float initMiddayLerp;
	
	[HideInInspector]
	public float extraOvercast;
	public float minExtraOvercast, maxExtraOvercast;
	public float minDelaySpeed, maxDelaySpeed;
	public float changeSpeed;
	
	void OnEnable () {
		
		SceneManager.instance.OnNewDay += UpdateClouds;
		UpdateClouds ();
		StartCoroutine(ChangeExtraOvercast());
		
		Shader.SetGlobalFloat("ls_cloudintensity", intensity);
		Shader.SetGlobalFloat("ls_shadowscale", shadowScale);
		Shader.SetGlobalVector("ls_cloudcolor", (new Vector3(1,0.9f,0.95f)));
		Shader.SetGlobalFloat("ls_distScale", distScale);
	}
	
	void UpdateClouds () {
		
		float minCloudiness = minCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float maxCloudiness = maxCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float randomCloudiness = Random.Range(minCloudiness, maxCloudiness);
		float likelyCloudiness = likelyCloudinessOverYear.Evaluate (SceneManager.curvePos);
		float influence = likelyInfluence.Evaluate (Random.value);
		initOvercast = Mathf.Lerp (randomCloudiness, likelyCloudiness, influence);
		setOvercast (initOvercast);
		
		float height = Random.Range (minHeight, maxHeight);
		Shader.SetGlobalFloat ("ls_cloudscale", height);
		float maxRandomValue = overcastToRandomization.Evaluate (overcast);
		middayLerp = Random.Range (0, maxRandomValue);
		initMiddayLerp = middayLerp;
		middayTint = _middayTint.Evaluate (middayLerp);
		
		speed = Mathf.Lerp (minSpeed, maxSpeed, WindControl.instance.windiness);
		Shader.SetGlobalFloat("ls_time", Time.time * speed * 0.25f);
	}
	
	void Update () {
		
		speed = Mathf.Lerp (minSpeed, maxSpeed, WindControl.instance.windiness);
		Shader.SetGlobalFloat("ls_time", Time.time * speed * 0.25f);
		/*
		lerp = Mathf.Clamp01 (lerp);
		lerp2 = Mathf.Clamp01 (lerp2);
		setOvercast (lerp);
		middayTint = _middayTint.Evaluate (lerp2);

		Shader.SetGlobalFloat("ls_cloudintensity", intensity);
		Shader.SetGlobalFloat("ls_shadowscale", shadowScale);

		Shader.SetGlobalFloat("ls_distScale", distScale);
		*/
	}
	
	public void setOvercast(float _cloudiness) {

		overcast = _cloudiness;
		float scattering = Mathf.Lerp (minScattering, maxScattering, overcast);
		float sharpness = Mathf.Lerp (minSharpness, maxSharpness, overcast);
		float thickness = Mathf.Lerp (minThickness, maxThickness, overcast);
		Shader.SetGlobalFloat("ls_cloudscattering", scattering);
		Shader.SetGlobalFloat("ls_cloudsharpness", sharpness);
		Shader.SetGlobalFloat("ls_cloudthickness", thickness);
	}

	public void SetStormTint (float _grayAmount, float _darkness) {

		grayAmount = _grayAmount;
		Color initMiddayTint = _middayTint.Evaluate (initMiddayLerp);
		Color middayGrayscale = new Color (initMiddayTint.grayscale, initMiddayTint.grayscale, initMiddayTint.grayscale);
		Color middayAfterGray = Color.Lerp(initMiddayTint, middayGrayscale, _grayAmount);
		Color middayDarkened = Color.Lerp (middayAfterGray, Color.black, _darkness);
		middayTint = middayDarkened;
	}
	
	IEnumerator ChangeExtraOvercast () {
		
		float timer = 1;
		while (timer > 0) {
			
			float speed = Mathf.Lerp(minDelaySpeed, maxDelaySpeed, WindControl.instance.windiness);
			timer -= Time.deltaTime * speed;
			yield return null;
		}
		float _extraOvercast = Mathf.Lerp (minExtraOvercast, maxExtraOvercast, 1 - overcast);
		float extraOvercastGoal = Random.Range (-_extraOvercast, _extraOvercast);
		StartCoroutine (SetExtraOvercast (extraOvercastGoal));
	}
	
	IEnumerator SetExtraOvercast (float extraOvercastGoal) {
		
		while (extraOvercast != extraOvercastGoal) {
			
			extraOvercast = Mathf.MoveTowards(extraOvercast, extraOvercastGoal, Time.deltaTime * changeSpeed);
			yield return null;
		}
		StartCoroutine(ChangeExtraOvercast());
	}
}
