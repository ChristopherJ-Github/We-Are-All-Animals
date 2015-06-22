using UnityEngine;
using System.Collections;

public class RainReactions : MonoBehaviour {
	public Emission reactTo;
	public float evapRate;
	public float AccumRate;
	public float minHeight, maxHeight;
	public float height;
	public delegate void stateHandler (); 		
	public event stateHandler reactionState;
	
	
	void Start ()									
	{
		reactionState = evaporating;
		reactTo.onStart += startReaction;
		reactTo.onStop += stopReaction;
		retrieveData ();
		DataManager.instance.OnSave += OnSave;
	}
	
	void retrieveData () {
		
		if (DataManager.instance.successfullyLoaded) 
			height = DataManager.instance.data.rainHeight;
		else 
			height = minHeight;
		
		height = Mathf.Clamp (height, minHeight, maxHeight);
		Vector3 position = transform.position;
		position.y = height;
		transform.transform.position = position;
	}
	
	void startReaction () {
		
		reactionState = accumulating;
	}
	
	void stopReaction () {
		
		reactionState = evaporating; 
	}
	
	void Update () {
		
		reactionState ();
	}
	
	void idle() {
		
		//Debug.Log ("Rain Idling");
	}
	
	void accumulating () {
		
		if (WeatherControl.instance.transition != 1 && WeatherControl.instance.cloudTransition != 1)
			return;
		
		float tmpRate = evapRate * temperature.tempPercentage;
		tmpRate = Mathf.Clamp (tmpRate, 0f, 1f);
		
		height = Mathf.MoveTowards (height, maxHeight, AccumRate * Time.deltaTime);
		Vector3 position = transform.position;
		position.y = height;
		transform.transform.position = position;
		
		if (height >= maxHeight)
			reactionState = idle;
	}
	
	void evaporating () {
		
		float tmpRate = evapRate * temperature.tempPercentage;
		tmpRate = Mathf.Clamp (tmpRate, 0f, 1f);
		
		height = Mathf.MoveTowards (height, minHeight, tmpRate * Time.deltaTime);
		Vector3 position = transform.position;
		position.y = height;
		transform.transform.position = position;
		
		if (height <= minHeight)
			reactionState = idle;
	}
	
	void OnSave () {
		
		DataManager.instance.data.rainHeight = height;
	}
}
