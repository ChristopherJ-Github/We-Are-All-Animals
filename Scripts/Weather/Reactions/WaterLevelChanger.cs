using UnityEngine;
using System.Collections;

public class WaterLevelChanger : MonoBehaviour {

	public Emission Rain;
	public delegate void stateHandler (); 		
	public event stateHandler reactionState;
	private float _waterLevel;
	public float waterLevel{
		get { return _waterLevel; }
		set { _waterLevel = Mathf.Clamp01(value); }
	}
	
	void Start () {

		reactionState = evaporating;
		Rain.OnStart += StartAccumulating;
		Rain.OnStop += StartEvaporating;
		RetrieveData ();
		DataManager.instance.OnSave += OnSave;
		SetHeight (waterLevel);
	}
	
	void RetrieveData () {

		if (DataManager.instance.successfullyLoaded)
			waterLevel = DataManager.instance.data.waterLevel;
		else
			waterLevel = 0;
	}

	public float minHeight, maxHeight;
	void SetHeight (float waterLevel) {

		float height = Mathf.Lerp (minHeight, maxHeight, waterLevel);
		Vector3 position = transform.position;
		position.y = height;
		transform.transform.position = position;
	}

	[Tooltip("In minutes")]
	public float minSeverityAccumTime, maxSeverityAccumeTime;
	private float accumTimeNeeded;
	void StartAccumulating () {
		
		accumTimeNeeded = Mathf.Lerp (minSeverityAccumTime, maxSeverityAccumeTime, WeatherControl.instance.severity) * 60;
		timePassed = Mathf.Lerp (0, accumTimeNeeded, waterLevel);
		reactionState = accumulating;
	}

	private float timePassed;
	void accumulating () {
		
		if (WeatherControl.instance.transition != 1 && WeatherControl.instance.cloudTransition != 1)
			return;
		timePassed += Time.deltaTime;
		waterLevel = timePassed / accumTimeNeeded;
		SetHeight (waterLevel);
		if (waterLevel >= 1)
			reactionState = idle;
	}

	void idle() {}

	[Tooltip("In minutes")]
	public float minTempEvapTime, maxTempEvapTime;
	private float evaporationTime;
	void StartEvaporating () {
		
		evaporationTime = Mathf.Lerp (minTempEvapTime, maxTempEvapTime, TemperatureManager.temperature) * 60;
		timeLeft = Mathf.Lerp (0, evaporationTime, waterLevel);
		reactionState = evaporating; 
	}

	private float timeLeft;
	void evaporating () {

		timeLeft -= Time.deltaTime;
		waterLevel = timeLeft / evaporationTime;
		SetHeight (waterLevel);
		if (waterLevel <= 0)
			reactionState = idle;
	}
	
	void Update () {
		
		reactionState ();
	}

	void OnSave () {
		
		DataManager.instance.data.waterLevel = waterLevel;
	}
}
