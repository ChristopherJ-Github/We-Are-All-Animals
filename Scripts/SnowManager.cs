using UnityEngine;
using System.Collections;

public class SnowManager : Singleton<SnowManager> {
	
	public Emission SnowWeather;
	public delegate void stateHandler (); 		
	public event stateHandler reactionState;
	private float _snowLevel;
	public float snowLevel{
		get { return _snowLevel; }
		set { _snowLevel = Mathf.Clamp01(value); }
	}
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
			snowLevel = DataManager.instance.data.snowLevel;
		else 
			snowLevel = 0;
		TriggerSnowChange (snowLevel);
	}

	public delegate void eventHandler (float snowLevel); 
	public event eventHandler OnSnowChange;
	public float secondStageThreshold;
	public void TriggerSnowChange (float snowLevel) {
		
		Shader.SetGlobalFloat ("_SnowNormalized", snowLevel);
		Shader.SetGlobalFloat ("_Stage2Thres", secondStageThreshold);
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
		timePassed = Mathf.Lerp (0, accumTimeNeeded, snowLevel);
		reactionState = accumulating;
	}

	private float timePassed;
	public void accumulating () {
		
		if (WeatherControl.instance.transition != 1 && WeatherControl.instance.cloudTransition != 1)
			return;
		timePassed += Time.deltaTime;
		snowLevel = timePassed / accumTimeNeeded;
		snowLevel = LeafFallManager.thereAreLeaves ? 0 : snowLevel;
		TriggerSnowChange (snowLevel);
		if (snowLevel >= 1) 
			reactionState = idle;
	}

	void idle() {}

	[Tooltip("In minutes")]
	public float minTempMeltTime, maxTempMeltTime;
	private float meltTime;
	public void StartMelting () {

		meltTime = Mathf.Lerp (minTempMeltTime, maxTempMeltTime, TemperatureManager.temperature) * 60;
		timeLeft = Mathf.Lerp (0, meltTime, snowLevel);
		reactionState = melting; 
	}

	private float timeLeft;
	void melting () {

		timeLeft -= Time.deltaTime;
		snowLevel = timeLeft / meltTime;
		snowLevel = LeafFallManager.thereAreLeaves ? 0 : snowLevel;
		TriggerSnowChange (snowLevel);
		if (snowLevel <= 0) 
			reactionState = idle;
	}
	
	void Update () {

		TriggerSnowChange (snowLevel);
		reactionState ();
	}

	void OnSave () {
		
		DataManager.instance.data.snowLevel = snowLevel;
	}
}
