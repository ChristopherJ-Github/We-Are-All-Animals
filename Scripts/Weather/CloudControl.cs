using UnityEngine;
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
		SetSkyboxTint (Random.value);
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

	[HideInInspector] public float overcast;
	public float minScattering, maxScattering;
	public float minSharpness, maxSharpness;
	public float minThickness, maxThickness;
	public void SetOvercast(float overcast) {
		
		this.overcast = overcast;
		float scattering = Mathf.Lerp (minScattering, maxScattering, overcast);
		float sharpness = Mathf.Lerp (minSharpness, maxSharpness, overcast);
		float thickness = Mathf.Lerp (minThickness, maxThickness, overcast);
		Shader.SetGlobalFloat("ls_cloudscattering", scattering);
		Shader.SetGlobalFloat("ls_cloudsharpness", sharpness);
		Shader.SetGlobalFloat("ls_cloudthickness", thickness);
	}

	public float minHeight, maxHeight;
	void RandomizeCloudHeight () {

		float height = Random.Range (minHeight, maxHeight);
		Shader.SetGlobalFloat ("ls_cloudscale", height);
	}

	public AnimationCurve overcastToRandomization;
	private float initMiddayValue;
	[HideInInspector] public float middayValue;
	[HideInInspector] public Color middayTint;
	public Gradient _middayTint;
	public Gradient nightToDuskTint;
	void SetSkyboxTint (float tintValue) {

		float maxRandomValue = overcastToRandomization.Evaluate (overcast);
		middayValue = Mathf.Lerp(0, maxRandomValue, tintValue);
		initMiddayValue = middayValue;
		middayTint = _middayTint.Evaluate (middayValue);
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
	}

	public float minSpeed, maxSpeed;
	void SetCloudSpeed () {
		
		float speed = Mathf.Lerp (minSpeed, maxSpeed, WindControl.instance.windiness);
		Shader.SetGlobalFloat("ls_time", Time.time * speed * 0.25f);
	}
	
	[HideInInspector] public float grayAmount;
	public void SetStormTint (float _grayAmount, float _darkness) {

		grayAmount = _grayAmount;
		Color initMiddayTint = _middayTint.Evaluate (initMiddayValue);
		Color middayGrayscale = new Color (initMiddayTint.grayscale, initMiddayTint.grayscale, initMiddayTint.grayscale);
		Color middayAfterGray = Color.Lerp(initMiddayTint, middayGrayscale, _grayAmount);
		Color middayDarkened = Color.Lerp (middayAfterGray, Color.black, _darkness);
		middayTint = middayDarkened;
	}

	public float minDelaySpeed, maxDelaySpeed;
	public float minExtraOvercast, maxExtraOvercast;
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
