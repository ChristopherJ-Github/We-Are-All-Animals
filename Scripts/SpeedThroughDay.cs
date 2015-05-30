using UnityEngine;
using System.Collections;
using System;

public class SpeedThroughDay : Singleton<SpeedThroughDay> {
	
	[HideInInspector] 
	public string minVal = "";
	double goal;
	float increment;
	float counter;
	[HideInInspector]
	public bool on;
	
	delegate void stateHandler();
	stateHandler state;
	
	
	void Start () {
		
		state = idle;
	}
	
	void idle () {
		
	}
	
	void Update () {
		
		state ();
	}
	
	public void activate () {
		
		counter = 0f;
		float minValint = (float.Parse (minVal));
		if (minValint <= 0 || minValint == null) {
			state = idle;
			return;
		}
		float minsInDay =  60 * 24;
		increment = minsInDay / minValint;
		goal = minsInDay;
		on = true;
		state = running;
	}
	
	void running () {
		
		counter += increment * Time.deltaTime / 60f;
		SceneManager.instance.ChangeTime (0, 525600, counter);
	}
	
	public void stop () {
		
		on = false;
		state = idle;
	}
	
	void OnGUI() {
		
		if (GUIManager.instance.toggleStats) {
			minVal = GUI.TextField (new Rect (Screen.width * 0.8f, Screen.height * 0.7f, 40, 20), minVal, 25);
			GUI.Label(new Rect(Screen.width * 0.86f, Screen.height * 0.7f, 200, 20), "minutes in a day");
			if (state == idle)
				if (GUI.Button (new Rect(Screen.width * 0.8f, Screen.height * 0.8f, Screen.width * 0.17f, Screen.height * 0.15f), "Fast Forward")) 
					activate();
			if (state == running)
				if (GUI.Button (new Rect(Screen.width * 0.8f, Screen.height * 0.8f, Screen.width * 0.17f, Screen.height * 0.15f), "Stop")) 
					stop ();
		}
	}
}
