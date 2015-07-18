using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

[System.Serializable]
public class WeatherInfo {

	public GameObject weather;
	public AnimationCurve spawnChance;
	[HideInInspector] public float maxSeverity { 
		get{ return severityOverYear.Evaluate (SceneManager.curvePos); } 
	}
	public AnimationCurve severityOverYear;
	public float minCloudTransition;
	public float maxCloudTransition;
	public float minTransition = 0.25f;
	public float maxTransition = 2;
	public float minDuration = 1; 
	public float maxDuration = 5;
	public bool onceAYear;
	public bool changesClouds = true;
	public bool usesFilter = true;
	public bool causesStorms = true;
}

public class WeatherControl : Singleton<WeatherControl> {

	public delegate void weatherStateHandler();
	public static weatherStateHandler weatherState;
	
	void Start () {
		
		TurnOff ();
		SceneManager.instance.OnNewDay += AttemptToSpawn;
		GUIManager.instance.OnGuiEvent += OnGuiEvent;
		AttemptToSpawn ();
	}

	public WeatherInfo[] weatherTypes;
	[HideInInspector] private float currentChance;
	public float chanceDivisor = 1f;
	/// <summary>
	/// State where there is no weather. Attempts to spawn weather every call
	/// </summary>
	void AttemptToSpawn () {

		TurnOff ();
		WeatherInfo weatherType = weatherTypes [UnityEngine.Random.Range(0, weatherTypes.Length)];
		currentChance = 100 * (weatherType.spawnChance.Evaluate(SceneManager.curvePos));
		if (currentChance/chanceDivisor > UnityEngine.Random.Range (0.0F, 100.0F)) {
			float randomValue = UnityEngine.Random.value;
			float transitionLength = Mathf.Lerp (weatherType.minTransition, weatherType.maxTransition, randomValue);
			float cloudTransitionLength = Mathf.Lerp (weatherType.minCloudTransition, weatherType.maxCloudTransition, randomValue);
			float weatherlength = Mathf.Lerp (weatherType.minDuration, weatherType.maxDuration, randomValue);
			float startTime = (float)SceneManager.minsAtDayStart + UnityEngine.Random.Range (0, 12 * 60);
			EnableWeather(weatherType, startTime, weatherlength, transitionLength, cloudTransitionLength);
		}
	}

	public static WeatherInfo currentWeather;
	[HideInInspector] private float cloudTransInTime, transInTime, idleTime, transOutTime, cloudTransOutTime, stopTime;
	[HideInInspector] public bool safeToPress;
	/// <summary>
	/// Starting point of weather cycle where weatherType is enabled
	/// </summary>
	/// <param name="weatherType">Weather type.</param>
	public void EnableWeather (WeatherInfo weatherType, float startTime, float weatherLength, float transitionLength, float cloudTransitionLength = 0, float? severity = null) {

		currentWeather = weatherType;
		SetSeverity (weatherType, severity);
		currentWeather.weather.SetActive(true);
		if (currentWeather.changesClouds) {
			cloudTransInTime = startTime;
			transInTime = cloudTransInTime + cloudTransitionLength;
			idleTime = transInTime + transitionLength;
			transOutTime = idleTime + weatherLength;
			cloudTransOutTime = transOutTime + transitionLength;
			stopTime = cloudTransOutTime + cloudTransitionLength; 
		} else {
			transInTime = startTime;
			idleTime = transInTime + transitionLength;
			transOutTime = idleTime + weatherLength;
			stopTime = transOutTime + transitionLength; 
		}
		safeToPress = false;
		weatherState = On;
	}

	void SetSeverity (WeatherInfo weatherType, float? severity) {

		float maxSeverity = GetMaxSeverity (weatherType);
		this.severity = severity ?? UnityEngine.Random.Range (0, maxSeverity);
	}

	public float GetMaxSeverity (WeatherInfo weatherType = null) {

		WeatherInfo _weatherType = weatherType ?? currentWeather;
		if (_weatherType == null) 
			return 0;
		float maxSeverity = _weatherType.maxSeverity;
		if (_weatherType.weather.name == "Wind") {
			maxSeverity = WindStorm.GetMaxSeverity(maxSeverity);
		} else if (_weatherType.weather.name == "Fog") {
			Fog fog = _weatherType.weather.GetComponent<Fog>();
			maxSeverity = fog.GetMaxSeverity (maxSeverity);
		}
		return maxSeverity;
	}

	[HideInInspector] public float transition, cloudTransition, totalTransition;
	void On () {
		
		float currentMinutes = (float)SceneManager.currentMinutes;
		if (currentMinutes < idleTime) {
			cloudTransition = Mathf.InverseLerp (cloudTransInTime, transInTime, currentMinutes);
			transition = Mathf.InverseLerp (transInTime, idleTime, currentMinutes);
			totalTransition = currentWeather.changesClouds ? Mathf.InverseLerp(cloudTransInTime, idleTime, currentMinutes) : transition;
		} else {
			transition = Mathf.InverseLerp (currentWeather.changesClouds ? cloudTransOutTime : stopTime, transOutTime, currentMinutes);
			cloudTransition = Mathf.InverseLerp (stopTime, cloudTransOutTime, currentMinutes);
			totalTransition = Mathf.InverseLerp(stopTime, transOutTime, currentMinutes);
		}
		SetStormSatus ();
	}

	[HideInInspector] public bool storm;
	public float stormThreshold;
	void SetStormSatus (bool? status = null) {

		if (status == null) {
			if (currentWeather.causesStorms) {
				storm = totalTransition * severity >= stormThreshold;
			} else {
				storm = false;
			}
		} else {
			storm = (bool)status;
		}
	}
	
	void Update () {
		
		weatherState ();
	}
	
	/// <summary>
	/// Sets to idle state as well as signals that it's safe
	/// to spawn new weather.
	/// </summary>
	public void TurnOff () {

		if (currentWeather != null)
			currentWeather.weather.SetActive(false);
		cloudTransition = 0;
		transition = 0;
		totalTransition = 0;
		SetStormSatus (false);
		safeToPress = true;
		currentWeather = null;
		weatherState = Off;
	}
	
	void Off() {}
	
	public void ForceSpawn () {
		
		StopAllCoroutines ();
		StartCoroutine (ForceSpawnRoutine ());
	}
	
	IEnumerator ForceSpawnRoutine () {
		
		bool weatherSpawned = false;
		while (!weatherSpawned) {
			AttemptToSpawn ();
			weatherSpawned = !safeToPress;
			yield return null;
		}
	}

	[HideInInspector] public float severity;
	void OnGuiEvent (float val) {

		if (currentWeather != null)
			severity = Mathf.Lerp (0, GetMaxSeverity(), val);
	}

	private Dictionary <string, int[]> spawnedInYear;
	public string dataPath = @"C:\Data\data.txt";
}