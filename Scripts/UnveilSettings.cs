using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class AnimalInfo {

	public AnimalAnimator animalAnimator;
	public Vector2 spawnTime;
	public DateTime dateTime;
	[HideInInspector] public bool spawned;

}

public class UnveilSettings : MonoBehaviour {
	
	void Start () {

		SceneManager.instance.OnNewDay += CheckDate;
	}

	public int unveilMonth, unveilDay;
	void CheckDate () {

		if (SceneManager.realDate.Month == unveilMonth && SceneManager.realDate.Day == unveilDay) {
			Initialize ();
			active = true;
		} else {
			active = false;
		}
	}

	void Initialize () {

		SkyManager.instance.SetPhaseTimes (12, 15, 19.5f, 20.25f);
		InitializeDateTimes ();
		SpawnSnow ();
	}

	void InitializeDateTimes () {

		for (int i = 0; i < animals.Length; i++) {
			Vector2 spawnTime = animals[i].spawnTime;
			DateTime dateTime = new DateTime(SceneManager.currentDate.Year, 
			                                 unveilMonth, unveilDay, (int)spawnTime.x, (int)spawnTime.y, 0);
			animals[i].dateTime = dateTime;
		}
	}

	void SpawnSnow () {

		WeatherControl.instance.TurnOff();
		WeatherInfo weatherType = WeatherControl.instance.weatherTypes [0];
		float maxSeverity = WeatherControl.instance.GetMaxSeverity(weatherType);
		WeatherControl.instance.EnableWeather(weatherType, (float)SceneManager.minsAtDayStart, 1440, 1, 1, maxSeverity);
	}

	private bool active;
	void Update () {

		if (!active) return;
		MakeAnimalSpawnAttempt ();
	}

	public AnimalInfo[] animals;
	void MakeAnimalSpawnAttempt () {

		foreach (AnimalInfo animalInfo in animals) {
			if (animalInfo.spawned) continue;
			if (SceneManager.realDate >= animalInfo.dateTime) {
				AnimationSpawner.instance.Spawn(animalInfo.animalAnimator);
				animalInfo.spawned = true;
			}
		}
	}
}
