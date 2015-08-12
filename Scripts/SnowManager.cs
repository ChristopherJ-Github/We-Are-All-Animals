using UnityEngine;
using System.Collections;

public class SnowManager : Singleton<SnowManager> {
	
	public Emission SnowWeather;
	public delegate void stateHandler (); 		
	public event stateHandler reactionState;
	public float snowLevel;
	public Gradient objectSnowTint, terrainSnowTint;
	
	void Start () {								
		
		reactionState = melting;
		SnowWeather.OnStart += StartAccumulating;
		SnowWeather.OnStop += StartMelting;
		SceneManager.instance.OnNewDay += SetRiverEvent;
		DataManager.instance.OnSave += OnSave;
		RetrieveData ();
		SetRiver ();
	}
	
	void RetrieveData () {
		
		if (DataManager.instance.successfullyLoaded) 
			linearSnowLevel = DataManager.instance.data.linearSnowLevel;
		else 
			linearSnowLevel = 0;
		snowLevel = GetSnowLevel (linearSnowLevel);
		TriggerSnowChange (snowLevel);
	}

	public delegate void eventHandler (float snowLevel); 
	public event eventHandler OnSnowChange;
	public float secondStageThreshold;
	public AnimationCurve snowCurve;
	public void TriggerSnowChange (float snowLevel) {
		
		Shader.SetGlobalFloat ("_SnowNormalized", snowLevel);
		Shader.SetGlobalFloat ("_Stage2Thres", secondStageThreshold); //might only be needed at start
		snowLevel = snowCurve.Evaluate (snowLevel);
		if (OnSnowChange != null) 
			OnSnowChange (snowLevel);
	}
	
	void SetRiverEvent () {
		
		SetRiver ();
	}

	public GameObject river, frozenRiver;
	[HideInInspector] public bool riverFrozen {
		get { return frozenRiver.activeSelf; } 
	}

	public float freezelevel;
	public void SetRiver (bool? freeze = null) {
		
		if (freeze != null) {
			if (freeze == true) {
				river.SetActive(false);
				frozenRiver.SetActive(true);
			} 
			if (freeze == false) {
				river.SetActive(true);
				frozenRiver.SetActive(false);
			}
		} else {
			if (snowLevel >= freezelevel && !LeafFallManager.thereAreLeaves) {
				river.SetActive(false);
				frozenRiver.SetActive(true);
			} else {
				river.SetActive(true);
				frozenRiver.SetActive(false);
			}
		}
	}

	[Tooltip("In minutes")]
	public float minSeverityAccumTime, maxSeverityAccumeTime;
	private float accumTimeNeeded;
	void StartAccumulating () {

		accumTimeNeeded = Mathf.Lerp (minSeverityAccumTime, maxSeverityAccumeTime, WeatherControl.instance.severity) * 60;
		timePassed = Mathf.Lerp (0, accumTimeNeeded, linearSnowLevel);
		reactionState = accumulating;
	}

	private float timePassed;
	public void accumulating () {
		
		if (WeatherControl.instance.transition != 1 && WeatherControl.instance.cloudTransition != 1)
			return;
		timePassed += Time.deltaTime;
		snowLevel = GetSnowLevel(timePassed / accumTimeNeeded);
		TriggerSnowChange (snowLevel);
		if (snowLevel >= 1) 
			reactionState = idle;
	}

	public float maxSnowThreshold;
	public AnimationCurve maxSnowOverYear;
	private float _linearSnowLevel;
	public float linearSnowLevel {
		get { return _linearSnowLevel; }
		set {
			float maxSnowLevel = maxSnowOverYear.Evaluate(SceneManager.curvePos);
			_linearSnowLevel = Mathf.Clamp(value, 0, maxSnowLevel); 
		}
	}
	float GetSnowLevel (float initSnowLevel) {

		linearSnowLevel = initSnowLevel;
		float newSnowLevel = 0;
		if (initSnowLevel < maxSnowThreshold) 
			newSnowLevel = Mathf.InverseLerp(0, maxSnowThreshold, initSnowLevel);
		else
			newSnowLevel = 1;
		return newSnowLevel;
	}

	void idle() {}

	[Tooltip("In minutes")]
	public float minTempMeltTime, maxTempMeltTime;
	private float meltTime;
	public void StartMelting () {

		meltTime = Mathf.Lerp (minTempMeltTime, maxTempMeltTime, TemperatureManager.temperature) * 60;
		timeLeft = Mathf.Lerp (0, meltTime, linearSnowLevel);
		reactionState = melting; 
	}

	private float timeLeft;
	void melting () {

		timeLeft -= Time.deltaTime;
		snowLevel = GetSnowLevel(timeLeft / meltTime);
		TriggerSnowChange (snowLevel);
		if (snowLevel <= 0) 
			reactionState = idle;
	}
	
	void Update () {

		float _meltTime = Mathf.Lerp (minTempMeltTime, maxTempMeltTime, TemperatureManager.temperature) * 60;
		TriggerSnowChange (snowLevel);
		reactionState ();
	}

	void OnSave () {
		
		DataManager.instance.data.linearSnowLevel = linearSnowLevel;
	}
}
