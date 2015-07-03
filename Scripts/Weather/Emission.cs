using UnityEngine;
using System.Collections;

public class Emission : MonoBehaviour {

	public delegate void weatherChangeHandler (); 
	public event weatherChangeHandler onStart;
	public void notifyStart () {
		
		if (onStart != null) 
			onStart ();
	}

	public event weatherChangeHandler onStop;
	public void notifyStop () {
		
		if (onStop != null) 
			onStop ();
	}

	void Awake () {
		
		_lightning = GetComponent<Lightning> ();
		particleAnimator = GetComponent<ParticleAnimator> ();
		originalPosition = transform.position;
		originalColor = renderer.material.GetColor ("_TintColor");
	}

	public bool mainSystem = true;
	void OnEnable () {

		if (mainSystem)
			notifyStart ();
		initializeValues ();
	}
	
	public bool globalFog, lightning, dust;
	private Lightning _lightning;
	void initializeValues() {

		initOvercast = CloudControl.instance.overcast;
		initWindiness = WindControl.instance.windiness; 
		initFogDesnity = RenderSettings.fogDensity;
		if (globalFog)
			FogControl.instance.SetGlobalFog(true);
		if (lightning)
			_lightning.enabled = true;
		if (dust)
			WindControl.instance.createDust = true;
		particleEmitter.ClearParticles ();
	}
	
	void Update () {

		float transWindiness = UpdateWind ();
		UpdateVelocity (transWindiness);
		ShiftSource (transWindiness);
		float transSeverity = Mathf.Lerp (0, WeatherControl.instance.severity, WeatherControl.instance.transition);
		UpdateEmission (transSeverity);
		SetDaytimeAlpha ();
		if (mainSystem) {
			float transOvercast = UpdateOvercast ();
			SkyManager.instance.sun.weatherDarkness = transOvercast; 
			UpdateFog (transSeverity);
		}
	}

	private float initOvercast;
	public enum CloudTinting {darken, desaturate, none};
	public CloudTinting cloudTinting;
	float UpdateOvercast () {

		float transOvercast = Mathf.InverseLerp (0f, 0.7f, WeatherControl.instance.cloudTransition);
		float grayAmount = 0, darkness = 0;
		switch (cloudTinting) {
			case CloudTinting.none:
				break;
			case CloudTinting.darken:
				darkness = Mathf.InverseLerp (0.7f, 1f, WeatherControl.instance.cloudTransition) * WeatherControl.instance.severity;
				goto case CloudTinting.desaturate;
			case CloudTinting.desaturate:
				grayAmount = transOvercast;
				break;
		}
		CloudControl.instance.SetStormTint (grayAmount, darkness);
		CloudControl.instance.SetOvercast (transOvercast); 
		return transOvercast;
	}

	private float initWindiness;
	float UpdateWind () {

		float transWindiness = Mathf.Lerp (initWindiness, WeatherControl.instance.severity < initWindiness ? initWindiness : 
		                                   WeatherControl.instance.severity, WeatherControl.instance.cloudTransition);
		if (mainSystem)
			WindControl.instance.SetValues(transWindiness);
		return transWindiness;
	}

	public float minSpeed = 50, maxSpeed = 150;
	public float minGravity, maxGravity;
	private ParticleAnimator particleAnimator;
	public float minRandomization, maxRandomization;
	void UpdateVelocity (float windiness) {
		
		float speed = Mathf.Lerp (minSpeed, maxSpeed, windiness);
		Vector3 currentHorizontalVelocity = Vector3.Lerp(Vector3.zero, WindControl.instance.direction * speed, windiness);
		float gravity = Mathf.Lerp (minGravity, maxGravity, windiness);
		particleAnimator.force = Vector3.down * gravity + currentHorizontalVelocity;
		float currentRandomization = Mathf.Lerp (minRandomization, maxRandomization, windiness);
		particleAnimator.rndForce = (new Vector3 (1, 0, 1)) * currentRandomization;
	}
	
	public float minEmission = 100, maxEmission = 1000; 
	public float maxFogDesnity = 0.02f;
	void UpdateEmission (float severity) {

		particleEmitter.maxEmission =  Mathf.Lerp(minEmission, maxEmission, severity);
	}

	private float initFogDesnity;
	void UpdateFog (float severity) {

		float fogDesnity = Mathf.Lerp (initFogDesnity, maxFogDesnity, severity);
		FogControl.instance.SetFogDesnity (fogDesnity > initFogDesnity ? fogDesnity : initFogDesnity);
	}

	public float horizontalShift, verticalShift;
	public AnimationCurve windinessToShift;
	private Vector3 originalPosition;
	void ShiftSource (float windiness) {
		
		Vector3 newPosition = originalPosition;
		float shiftAmount = windinessToShift.Evaluate (windiness);
		float currentHorizontalShift = Mathf.Lerp (0, horizontalShift, shiftAmount);
		float currentVericalShift = Mathf.Lerp (0, verticalShift, shiftAmount);
		newPosition += -WindControl.instance.direction * currentHorizontalShift;
		newPosition += Vector3.down * currentVericalShift;
		particleEmitter.transform.position = newPosition;
	}

	public float nightDarkness;
	private Color originalColor;
	public AnimationCurve daytimeToDarkening;
	void SetDaytimeAlpha () {

		Color nightColor = Color.Lerp (originalColor, new Color(0,0,0, originalColor.a), nightDarkness);
		float daytimeInfluence = daytimeToDarkening.Evaluate (SkyManager.instance.nightDayLerp);
		Color currentColor = Color.Lerp (nightColor, originalColor, daytimeInfluence);
		renderer.material.SetColor ("_TintColor", currentColor);
	}
	
	void OnDisable () {

		if (applicationIsQuitting) return;
		if (globalFog)
			FogControl.instance.SetGlobalFog(false);
		if (lightning)
			_lightning.enabled = false;
		if (dust)
			WindControl.instance.createDust = false;
		UpdateEmission (0);
		UpdateVelocity (0);
		if (mainSystem) {
			UpdateFog (0);
			CloudControl.instance.SetStormTint (0, 0);
			CloudControl.instance.SetOvercast (initOvercast); 
			SkyManager.instance.sun.weatherDarkness = 0;
			WindControl.instance.SetValues(initWindiness); 
			notifyStop ();
		}
	}

	private bool applicationIsQuitting;
	void OnApplicationQuit () {
		
		applicationIsQuitting = true;
	}
}