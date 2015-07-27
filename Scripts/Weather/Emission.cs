using UnityEngine;
using System.Collections;

public class Emission : MonoBehaviour {

	public delegate void weatherChangeHandler (); 
	public event weatherChangeHandler OnStart;
	public void NotifyStart () {
		
		if (OnStart != null) 
			OnStart ();
	}

	public event weatherChangeHandler OnStop;
	public void NotifyStop () {
		
		if (OnStop != null) 
			OnStop ();
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
			NotifyStart ();
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
			Dust.instance.createDust = true;
		particleEmitter.ClearParticles ();
	}
	
	void Update () {

		float transSeverity = Mathf.Lerp (0, WeatherControl.instance.severity, WeatherControl.instance.transition);
		float transWindiness = UpdateWind (transSeverity);
		UpdateVelocity (transWindiness);
		ShiftSource (transWindiness);
		UpdateEmission (WeatherControl.instance.severity, WeatherControl.instance.transition);
		SetBrightness ();
		if (mainSystem) {
			float overcastAmount = UpdateOvercast ();
			SkyManager.instance.sun.weatherDarkness = overcastAmount; 
			UpdateFog (transSeverity);
		}
	}

	private float initOvercast;
	public enum CloudTinting {darken, desaturate, none};
	public CloudTinting cloudTinting;
	[HideInInspector] public bool lightningActivated;
	float UpdateOvercast () {

		float overcastAmount = Mathf.InverseLerp (0f, 0.7f, WeatherControl.instance.cloudTransition);
		float maxOvercast = overcastAmount < initOvercast ? initOvercast : overcastAmount;
		float transOvercast = Mathf.Lerp (initOvercast, maxOvercast, overcastAmount);
		float grayAmount = 0, darkness = 0;
		switch (cloudTinting) {
			case CloudTinting.none:
				break;
			case CloudTinting.darken:
				float darknessInfluence = WeatherControl.instance.severity * (1 - SnowManager.instance.snowLevel);
				darkness = Mathf.InverseLerp (0.7f, 1f, WeatherControl.instance.cloudTransition) * darknessInfluence;
				goto case CloudTinting.desaturate;
			case CloudTinting.desaturate:
				grayAmount = transOvercast;
				break;
		}
		CloudControl.instance.SetStormTint (grayAmount, darkness, lightningActivated);
		CloudControl.instance.SetOvercast (transOvercast * Tester.instance.testValue01); 
		return overcastAmount;
	}

	private float initWindiness;
	float UpdateWind (float transSeverity) {

		float maxWindiness = WeatherControl.instance.severity < initWindiness ? initWindiness : WeatherControl.instance.severity;
		float transWindiness = Mathf.Lerp (initWindiness, maxWindiness, WeatherControl.instance.cloudTransition);
		if (mainSystem)
			WindControl.instance.SetValues(transWindiness, transSeverity);
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
	void UpdateEmission (float severity, float transition) {

		particleEmitter.maxEmission =  Mathf.Lerp(minEmission, maxEmission, severity) * transition;
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
	void SetBrightness () {

		Color nightColor = Color.Lerp (originalColor, new Color(0,0,0, originalColor.a), nightDarkness);
		float particleBrightness = AmbientLightingChanger.instance.GetParticleBrightness ();
		Color currentColor = Color.Lerp (nightColor, originalColor, particleBrightness);
		renderer.material.SetColor ("_TintColor", currentColor);
	}
	
	void OnDisable () {

		if (applicationIsQuitting) return;
		if (globalFog)
			FogControl.instance.SetGlobalFog(false);
		if (lightning)
			_lightning.enabled = false;
		if (dust)
			Dust.instance.createDust = false;
		UpdateEmission (0, 0);
		UpdateVelocity (0);
		if (mainSystem) {
			UpdateFog (0);
			CloudControl.instance.SetStormTint (0, 0, lightningActivated);
			CloudControl.instance.SetOvercast (initOvercast); 
			SkyManager.instance.sun.weatherDarkness = 0;
			WindControl.instance.SetValues(initWindiness); 
			NotifyStop ();
		}
	}

	private bool applicationIsQuitting;
	void OnApplicationQuit () {
		
		applicationIsQuitting = true;
	}
}