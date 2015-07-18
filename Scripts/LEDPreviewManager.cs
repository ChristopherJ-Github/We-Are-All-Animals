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

	void GetKeyInput () {

		WeatherInput ();
		FilterInput ();
		TimeInput ();
		AnimalInput();
		MiscInput ();
	}
	
	public Lightning lightning;
	void WeatherInput () {
		
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			StopAllCoroutines ();
			SpawnWeather (0);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			StopAllCoroutines ();
			SpawnWeather (1);
			lightning.SwitchState(false);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			StopAllCoroutines ();
			SpawnWeather (2);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			SpawnWeather (3);
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			StopAllCoroutines ();
			SpawnWeather (1);
			lightning.SwitchState(true);
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			WeatherControl.instance.TurnOff();
		}
		if (WeatherControl.currentWeather != null) {
			float severity = WeatherControl.instance.severity;
			if (Input.GetKey(KeyCode.Minus)) 
				severity -= transitionSpeed * Time.deltaTime;
			if (Input.GetKey(KeyCode.Equals)) 
				severity += transitionSpeed * Time.deltaTime;
			float maxSeverity = WeatherControl.instance.GetMaxSeverity();
			WeatherControl.instance.severity = Mathf.Clamp(severity, 0, maxSeverity);
		}
	}
	
	void SpawnWeather (int weatherIndex) {
		
		WeatherControl.instance.TurnOff();
		WeatherInfo weatherType = WeatherControl.instance.weatherTypes [weatherIndex];
		float maxSeverity = WeatherControl.instance.GetMaxSeverity(weatherType);
		WeatherControl.instance.EnableWeather(weatherType, (float)SceneManager.minsAtDayStart, 1440, 1, 1, maxSeverity);
	}
	
	void FilterInput () {
		
		if (Input.GetKeyDown(KeyCode.T)) {
			FilterManager.amplifyColorEffect.enabled = !FilterManager.amplifyColorEffect.enabled;
			StormFilterManager.amplifyColorEffect.enabled = !StormFilterManager.amplifyColorEffect.enabled;
			DarkFilterManager.amplifyColorEffect.enabled = !DarkFilterManager.amplifyColorEffect.enabled;
		}
		if (Input.GetKeyDown(KeyCode.Y)) 
			FilterManager.instance.NextFilter();
		if (Input.GetKeyDown(KeyCode.U)) 
			StormFilterManager.instance.NextFilter();
	}
	
	public float daySpeed, yearSpeed;
	void TimeInput () {
		
		DateTime currentDate = SceneManager.currentDate;
		if (Input.GetKeyDown(KeyCode.G)) {
			if (season == Season.Spring)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 7, 40, 0);
			if (season == Season.Summer)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 6, 27, 0);
			if (season == Season.Fall)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 7, 47, 0);
			if (season == Season.Winter)
				SceneManager.currentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 8, 20, 0);
		}
		if (Input.GetKeyDown(KeyCode.H)) {
			if (season == Season.Spring)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 9, 23, 0);
			if (season == Season.Summer)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 8, 9, 0);
			if (season == Season.Fall)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 9, 28, 0);
			if (season == Season.Winter)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 9, 28, 0);
		}
		if (Input.GetKeyDown(KeyCode.J)) 
			SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 15, 32, 0);
		if (Input.GetKeyDown(KeyCode.K)) {
			if (season == Season.Spring)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 19, 12, 0);
			if (season == Season.Summer)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 18, 41, 0);
			if (season == Season.Fall)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 19, 12, 0);
			if (season == Season.Winter)
				SceneManager.currentDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, 18, 54, 0);
		}
		if (Input.GetKeyDown(KeyCode.L)) 
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
	void AnimalInput () {

		if (Input.GetKeyDown(KeyCode.I) && !idlePreviewSpawned) {
			idlePreviewSpawned = true;
			StartCoroutine(SpawnIdleAnimations());
		}
		if (Input.GetKeyDown(KeyCode.O) && !allAnimalsSpawned) 
			AnimationSpawner.instance.SpawnAllAnimals();	
		if (Input.GetKeyDown(KeyCode.P)) {
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
	
	void MiscInput () {
		
		if (Input.GetKeyDown(KeyCode.Alpha8)) {
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