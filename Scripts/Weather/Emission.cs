using UnityEngine;
using System.Collections;
//script to apply temperature over time based on a curve
//Quentin Preik - 2012/10/22
//@fivearchers

public class Emission : GeneralWeather
{
	private bool applicationIsQuitting;
	public float maxHVelocity = 0.2f; 
	public float maxSpeed = 150;
	public float minSpeed = 50;
	public float maxEmission = 1000; 
	public float minEmission = 100;
	public float maxFogDesnity = 0.02f;
	public bool globalFog, lightning, dust;
	private Lightning _lightning;
	
	private float initCloudiness;
	private float initWindiness;
	private float initFogDesnity;
	
	public delegate void weatherChangeHandler (); 		
	public event weatherChangeHandler onStart;
	public event weatherChangeHandler onStop;
	
	public delegate void windHandler();
	public static event windHandler onNewWind;
	
	public void triggerNewWind() {
		
		if (onNewWind != null) 
			onNewWind();
	}
	
	public void notifyStart (){
		
		if (onStart != null) 
			onStart ();
	}
	
	public void notifyStop (){
		
		if (onStop != null) 
			onStop ();
	}
	
	void OnGuiEvent (float val) {
		
		severity = Mathf.Lerp (0, maxSeverity, val);
	}

	void Awake () {
		
		_lightning = GetComponent<Lightning> ();
	}
	
	void OnEnable () {
		
		notifyStart ();
		GUIManager.instance.OnGuiEvent += OnGuiEvent;
		initializeValues ();
	}
	
	void initializeValues() {

		initCloudiness = CloudControl.instance.overcast;
		initWindiness = WindControl.instance.windiness; //comment out for webbuild
		initFogDesnity = RenderSettings.fogDensity;
		if (globalFog)
			FogControl.instance.SetGlobalFog(true);
		if (lightning)
			_lightning.enabled = true;
		if (dust)
			WindControl.instance.createDust = true;
		changeSettings (0, 0, maxSpeed, 0, maxEmission);
	}
	
	void Update () {

		float grayAmount = Mathf.InverseLerp (0f, 0.7f, WeatherControl.instance.cloudTransition);
		float darkness = Mathf.InverseLerp (0.7f, 1f, WeatherControl.instance.cloudTransition) * severity;

		float transOvercast = Mathf.Lerp (initCloudiness, 1, grayAmount);
		float transWindiness = Mathf.Lerp (initWindiness, severity < initWindiness ? initWindiness : severity, WeatherControl.instance.cloudTransition);
		float transSeverity = Mathf.Lerp (0, severity, WeatherControl.instance.transition);

		CloudControl.instance.SetStormTint (grayAmount, darkness);
		CloudControl.instance.setOvercast (transOvercast); //limit the min value to prevent it from getting less cloudy 
		WindControl.instance.SetValues(transWindiness); //comment out for webbuild
		changeSettings (transSeverity, minSpeed, maxSpeed, 0, maxEmission);
	}
	
	/// <summary>
	/// Changes various properties of particles being emitted.
	/// </summary>
	/// <param name="srv">Srv.</param>
	/// <param name="minSpd">Minimum speed.</param>
	/// <param name="maxSpd">Maximum speed.</param>
	/// <param name="minEmn">Minimum emission.</param>
	/// <param name="maxEmn">Maximum emission.</param>
	void changeSettings (float srv, float minSpd, float maxSpd, float minEmn, float maxEmn) {
		
		float speed = Mathf.Lerp (minSpd, maxSpd, srv);
		float hVelocity = Mathf.Lerp (0, maxHVelocity, WindControl.instance.windiness);
		Vector3 velocity = WindControl.instance.direction * hVelocity;
		velocity.y = -1;
		velocity *= speed;
		particleEmitter.worldVelocity = velocity;
		particleEmitter.maxEmission =  Mathf.Lerp(minEmn, maxEmn, srv);
		
		float fogDesnity = Mathf.Lerp (initFogDesnity, maxFogDesnity, srv);
		FogControl.instance.SetFogDesnity (fogDesnity > initFogDesnity ? fogDesnity : initFogDesnity);
	}
	
	void OnDisable () {

		if (applicationIsQuitting) return;

		GUIManager.instance.OnGuiEvent -= OnGuiEvent;
		CloudControl.instance.SetStormTint (0, 0);
		CloudControl.instance.setOvercast (initCloudiness); //limit the min value to prevent it from getting less cloudy 
		WindControl.instance.SetValues(initWindiness); //comment out for webbuild
		changeSettings (0, minSpeed, maxSpeed, 0, maxEmission);

		if (globalFog)
			FogControl.instance.SetGlobalFog(false);
		if (lightning)
			_lightning.enabled = false;
		if (dust)
			WindControl.instance.createDust = false;
		notifyStop ();
	}

	void OnApplicationQuit () {
		
		applicationIsQuitting = true;
	}
}