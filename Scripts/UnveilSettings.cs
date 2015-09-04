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
	
	void OnEnable () {

		SceneManager.instance.OnNewDay += CheckDate;
	}

	public int unveilMonth, unveilDay;
	void CheckDate () {

		if (SceneManager.realDate.Month == unveilMonth && SceneManager.realDate.Day == unveilDay) {
			StartCoroutine (Initialize ());
			active = true;
		} else {
			active = false;
		}
	}

	IEnumerator Initialize () {

		for (int delay = 0; delay < 1; delay ++)
			yield return null;
		SkyManager.instance.SetPhaseTimes (4f, 6f, 21.5f, 24f);
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
		float startTime = (float)SceneManager.minsAtDayStart + (18.5f * 60);
		WeatherControl.instance.EnableWeather(weatherType, startTime, 1440, 15, 1, 0.5f);
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
