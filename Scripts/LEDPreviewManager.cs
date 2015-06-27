using UnityEngine;
using System.Collections;
using System;

public class LEDPreviewManager : MonoBehaviour {
	
	public enum Season { Spring, Summer, Fall, Winter };
	public bool hotKeyMode;
	public Season season;
	public float transitionSpeed;
	
	void Start () {

		if (hotKeyMode)
			return;
		SceneManager.instance.OnNewDay += ForceSettings;
		if (season == Season.Spring)
			SceneManager.currentDate = new DateTime (2015, 4, 8, 9, 23, 0);
		if (season == Season.Summer)
			SceneManager.currentDate = new DateTime (2015, 6, 18, 8, 9, 0);
		if (season == Season.Fall)
			SceneManager.currentDate = new DateTime (2015, 10, 14, 9, 28, 0);
		if (season == Season.Winter) {
			SceneManager.currentDate = new DateTime (2015, 1, 1, 9, 28, 0);
			SnowManager.instance.snowLevel = 1;
		}
	}
	
	void ForceSettings () {
		
		WeatherControl.instance.TurnOff ();
		FilterManager.instance.blend = 0;
		float likelyCloudiness =  CloudControl.instance.likelyCloudinessOverYear.Evaluate (SceneManager.curvePos);
		CloudControl.instance.SetOvercast (likelyCloudiness);
	}
	
	void Update () {
		
		GetKeyInput ();
	}
	
	public float keyDelay;
	private float keyDelayCounter;
	void GetKeyInput () {
		
		bool keyPressed = false;
		keyDelayCounter -= Time.deltaTime;
		if (keyDelayCounter > 0)
			return;
		WeatherInput (ref keyPressed);
		FilterInput (ref keyPressed);
		TimeInput (ref keyPressed);
		AnimalInput(ref keyPressed);
		MiscInput (ref keyPressed);
		if (keyPressed)
			keyDelayCounter = keyDelay;
	}
	
	public Lightning lightning;
	void WeatherInput (ref bool keyPressed) {
		
		if (Input.GetKey (KeyCode.Alpha1)) {
			keyPressed = true;
			StopAllCoroutines ();
			SpawnWeather (0);
		}
		if (Input.GetKey (KeyCode.Alpha2)) {
			keyPressed = true;
			StopAllCoroutines ();
			SpawnWeather (1);
			lightning.SwitchState(false);
		}
		if (Input.GetKey (KeyCode.Alpha3)) {
			keyPressed = true;
			StopAllCoroutines ();
			SpawnWeather (2);
		}
		if (Input.GetKey (KeyCode.Alpha4)) {
			keyPressed = true;
			SpawnWeather (3);
		}
		if (Input.GetKey (KeyCode.Alpha5)) {
			keyPressed = true;
			StopAllCoroutines ();
			SpawnWeather (1);
			lightning.SwitchState(true);
		}
		if (Input.GetKey (KeyCode.Alpha6)) {
			keyPressed = true;
			WeatherControl.instance.TurnOff();
		}
		if (WeatherControl.currentWeather != null) {
			
			float severity = WeatherControl.instance.severity;
			if (Input.GetKey(KeyCode.Minus)) 
				severity -= transitionSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.Equals)) 
				severity += transitionSpeed * Time.deltaTime;
			WeatherControl.instance.severity = Mathf.Clamp01(severity);
		}
	}
	
	void SpawnWeather (int weatherIndex) {
		
		WeatherControl.instance.TurnOff();
		WeatherControl.instance.EnableWeather(WeatherControl.instance.weatherTypes[weatherIndex], (float)SceneManager.minsAtDayStart, 1440, 1, 1, 1);
	}
	
	void FilterInput (ref bool keyPressed) {
		
		if (Input.GetKey(KeyCode.T)) {
			keyPressed = true;
			FilterManager.instance.on = !FilterManager.instance.on;
		}
		if (Input.GetKey(KeyCode.Y)) {
			keyPressed = true;
			FilterManager.instance.NextFilter(true, false);
		}
		if (Input.GetKey(KeyCode.U)) {
			keyPressed = true;
			FilterManager.instance.NextFilter(false, true);
		}
	}
	
	public float daySpeed, yearSpeed;
	void TimeInput (ref bool keyPressed) {
		
		DateTime currentDate = SceneManager.currentDate;
		if (Input.GetKey(KeyCode.G)) {
			keyPressed = true;
			if (season == Season.Spring)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 7, 40, 0);
			if (season == Season.Summer)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 6, 27, 0);
			if (season == Season.Fall)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 7, 47, 0);
			if (season == Season.Winter)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 8, 20, 0);
		}
		if (Input.GetKey(KeyCode.H)) {
			if (season == Season.Spring)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 9, 23, 0);
			if (season == Season.Summer)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 8, 9, 0);
			if (season == Season.Fall)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 9, 28, 0);
			if (season == Season.Winter)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 9, 28, 0);
		}
		if (Input.GetKey(KeyCode.J)) 
			SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 15, 32, 0);
		if (Input.GetKey(KeyCode.K)) {
			if (season == Season.Spring)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 19, 12, 0);
			if (season == Season.Summer)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 18, 41, 0);
			if (season == Season.Fall)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 19, 12, 0);
			if (season == Season.Winter)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 18, 54, 0);
		}
		if (Input.GetKey(KeyCode.L)) 
			SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 20, 35, 0);
		
		if (Input.GetKey(KeyCode.DownArrow))
			SceneManager.currentDate = SceneManager.currentDate.AddMinutes(-daySpeed * Time.deltaTime);
		if (Input.GetKey(KeyCode.UpArrow))
			SceneManager.currentDate = SceneManager.currentDate.AddMinutes(daySpeed * Time.deltaTime);
		if (Input.GetKey(KeyCode.LeftArrow)) 
			SceneManager.currentDate = SceneManager.currentDate.AddMinutes(-yearSpeed * Time.deltaTime);
		if (Input.GetKey(KeyCode.RightArrow)) 
			SceneManager.currentDate = SceneManager.currentDate.AddMinutes(yearSpeed * Time.deltaTime);
	}

	public GameObject animalPreviewPrefab;
	private bool allAnimalsSpawned;
	private bool idlePreviewSpawned;
	void AnimalInput (ref bool keyPressed) {

		if (Input.GetKey(KeyCode.I) && !idlePreviewSpawned) {
			keyPressed = true;
			idlePreviewSpawned = true;
			StartCoroutine(SpawnIdleAnimations());
		}
		if (Input.GetKey(KeyCode.O) && !allAnimalsSpawned) {
			allAnimalsSpawned = true;
			AnimationSpawner.instance.SpawnAllAnimals();
		}
		if (Input.GetKey(KeyCode.P)) {
			idlePreviewSpawned = false;
			allAnimalsSpawned = false;
		}
	}

	public IdleAnimalInfo[] idleAnimations;
	IEnumerator SpawnIdleAnimations () {

		for (int i = 0; i < idleAnimations.Length; i ++) {
			GameObject animal = Instantiate(idleAnimations[i].animalAnimator.gameObject) as GameObject;
			AnimalAnimator animalAnimator = animal.GetComponent<AnimalAnimator>();
			animalAnimator.splineIndex = idleAnimations[i].splineIndex;
			yield return null;
			animalAnimator.ForceIdle(idleAnimations[i].nodeIndex, true);
		}
	}
	
	void MiscInput (ref bool keyPressed) {
		
		if (Input.GetKey(KeyCode.Alpha8)) {
			keyPressed = true;
			bool riverIsFrozen = SnowManager.instance.frozenRiver.activeSelf;
			SnowManager.instance.SetRiver(!riverIsFrozen);
		}
		if (Input.GetKey(KeyCode.Alpha9))
			SnowManager.instance.snowLevel -= transitionSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.Alpha0))
			SnowManager.instance.snowLevel += transitionSpeed * Time.deltaTime;
		
		if (WeatherControl.currentWeather != null)
			return;
		
		float overcast = CloudControl.instance.overcast;
		if (Input.GetKey(KeyCode.Q)) 
			overcast += transitionSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.W)) 
			overcast -= transitionSpeed * Time.deltaTime;
		CloudControl.instance.SetOvercast(Mathf.Clamp01(overcast));

#if !UNITY_WEBPLAYER
		float windiness = WindControl.instance.windiness;
		if (Input.GetKey(KeyCode.E))
			windiness -= transitionSpeed * Time.deltaTime;
		if (Input.GetKey(KeyCode.R))
			windiness += transitionSpeed * Time.deltaTime;
		WindControl.instance.SetValues (Mathf.Clamp (windiness, 0, WindControl.instance.maxDailyWindiness));
#endif
	}
}

[Serializable]
public class IdleAnimalInfo {

	public AnimalAnimator animalAnimator;
	public int splineIndex;
	public int nodeIndex;
}