using UnityEngine;
using System.Collections;

public class SnowManager : Singleton<SnowManager> {
	
	public Emission reactTo;
	public delegate void stateHandler (); 		
	public event stateHandler reactionState;
	private float _snowlevel;
	public float snowLevel{
		get { return _snowlevel; }
		set { _snowlevel = Mathf.Clamp01(value); }
	}
	public Gradient objectSnowTint, terrainSnowTint;
	
	void Start () {								
		
		reactionState = melting;
		reactTo.onStart += startReaction;
		reactTo.onStop += stopReaction;
		SceneManager.instance.OnNewDay += SetRiverEvent;
		SceneManager.instance.OnNewDay += UpdateBillboards;
		DataManager.instance.OnSave += OnSave;
		retrieveData ();
		SetRiver ();
		UpdateBillboards ();
	}
	
	void retrieveData () {
		
		if (DataManager.instance.successfullyLoaded) 
			snowLevel = DataManager.instance.data.snowLevel;
		else 
			snowLevel = 0;
		TriggerSnowChange (snowLevel);
	}
	
	void SetRiverEvent () {
		
		SetRiver ();
	}
	
	public float freezelevel;
	public GameObject river, frozenRiver;
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
			if (snowLevel >= freezelevel) {
				river.SetActive(false);
				frozenRiver.SetActive(true);
			} else {
				river.SetActive(true);
				frozenRiver.SetActive(false);
			}
		}
	}
	
	public bool updateBillboards;
	void UpdateBillboards () {
		
		if (updateBillboards)
			StartCoroutine (MoveCamera ());
	}
	
	IEnumerator MoveCamera () {
		
		Transform mainCamera = Camera.main.transform;
		Vector3 euler = mainCamera.transform.eulerAngles;
		float shift = 0.5f;
		euler.y += shift;
		mainCamera.transform.rotation = Quaternion.Euler (euler);
		yield return new WaitForEndOfFrame ();
		
		euler = mainCamera.transform.eulerAngles;
		euler.y -= shift;
		mainCamera.transform.rotation = Quaternion.Euler (euler);
	}
	
	void startReaction () {
		
		reactionState = accumulating;
	}
	
	void stopReaction () {
		
		reactionState = melting; 
	}
	
	void Update () {

		TriggerSnowChange (snowLevel);
		reactionState ();
	}
	
	void idle() {}
	
	public float accumRate;
	void accumulating () {
		
		if (WeatherControl.instance.transition != 1 && WeatherControl.instance.cloudTransition != 1)
			return;
		
		snowLevel += accumRate * Time.deltaTime;
		TriggerSnowChange (snowLevel);
		
		if (snowLevel >= 1) {
			snowLevel = 1;
			reactionState = idle;
		}
	}
	
	public float meltRate;
	void melting () {
		
		float tmpRate = meltRate * temperature.tempPercentage;
		tmpRate = Mathf.Clamp (tmpRate, 0f, 1f);
		snowLevel -= tmpRate * Time.deltaTime;
		TriggerSnowChange (snowLevel);
		if (snowLevel <= 0) {
			snowLevel = 0;
			reactionState = idle;
		}
	}
	
	void OnSave () {
		
		DataManager.instance.data.snowLevel = snowLevel;
	}
	
	public delegate void eventHandler (float snowLvl); 
	public event eventHandler OnSnowChange;
	public void TriggerSnowChange (float snowLvl) {

		Shader.SetGlobalFloat ("_SnowNormalized", snowLvl);
		if (OnSnowChange != null) 
			OnSnowChange (snowLvl);
	}
}
