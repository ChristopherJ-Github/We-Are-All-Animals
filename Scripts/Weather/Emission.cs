using UnityEngine;
using System.Collections;

public class Emission : GeneralWeather {

	public delegate void weatherChangeHandler (); 
	public event weatherChangeHandler onStart;
	public void notifyStart (){
		
		if (onStart != null) 
			onStart ();
	}

	public event weatherChangeHandler onStop;
	public void notifyStop (){
		
		if (onStop != null) 
			onStop ();
	}
	
	void OnGuiEvent (float val) {
		
		severity = Mathf.Lerp (0, maxSeverity, val);
	}

	void Awake () {
		
		_lightning = GetComponent<Lightning> ();
		particleAnimator = GetComponent<ParticleAnimator> ();
		originalPosition = transform.position;
	}
	
	void OnEnable () {
		
		notifyStart ();
		GUIManager.instance.OnGuiEvent += OnGuiEvent;
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

		float transOvercast = UpdateOvercast ();
		SkyManager.instance.sun.weatherDarkness = transOvercast; 
		float transWindiness = UpdateWind ();
		UpdateVelocity (transWindiness);
		ShiftSource (transWindiness);
		float transSeverity = Mathf.Lerp (0, severity, WeatherControl.instance.transition);
		UpdateEmission (transSeverity);
		UpdateFog (transSeverity);
	}

	private float initOvercast;
	float UpdateOvercast () {

		float grayAmount = Mathf.InverseLerp (0f, 0.7f, WeatherControl.instance.cloudTransition);
		float darkness = Mathf.InverseLerp (0.7f, 1f, WeatherControl.instance.cloudTransition) * severity;
		CloudControl.instance.SetStormTint (grayAmount, darkness);
		float transOvercast = Mathf.Lerp (initOvercast, 1, grayAmount);
		CloudControl.instance.SetOvercast (transOvercast); 
		return transOvercast;
	}

	private float initWindiness;
	float UpdateWind () {

		float transWindiness = Mathf.Lerp (initWindiness, severity < initWindiness ? initWindiness : severity, WeatherControl.instance.cloudTransition);
		WindControl.instance.SetValues(transWindiness);
		return transWindiness;
	}

	public float minSpeed = 50, maxSpeed = 150;
	public float gravity;
	private ParticleAnimator particleAnimator;
	public float minRandomization, maxRandomization;
	void UpdateVelocity (float windiness) {
		
		float speed = Mathf.Lerp (minSpeed, maxSpeed, windiness);
		Vector3 currentHorizontalVelocity = Vector3.Lerp(Vector3.zero, WindControl.instance.direction * speed, windiness);
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
	
	void OnDisable () {

		if (applicationIsQuitting) return;
		GUIManager.instance.OnGuiEvent -= OnGuiEvent;
		CloudControl.instance.SetStormTint (0, 0);
		CloudControl.instance.SetOvercast (initOvercast); //limit the min value to prevent it from getting less cloudy 
		SkyManager.instance.sun.weatherDarkness = 0;
		WindControl.instance.SetValues(initWindiness); //comment out for webbuild
		UpdateEmission (0);
		UpdateVelocity (0);
		UpdateFog (0);
		if (globalFog)
			FogControl.instance.SetGlobalFog(false);
		if (lightning)
			_lightning.enabled = false;
		if (dust)
			WindControl.instance.createDust = false;
		notifyStop ();
	}

	private bool applicationIsQuitting;
	void OnApplicationQuit () {
		
		applicationIsQuitting = true;
	}
}