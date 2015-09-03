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

	void CheckDate () {

		if (SceneManager.realDate.Month == 9 && SceneManager.realDate.Day == 17) {
			Initialize ();
			active = true;
		} else {
			active = false;
		}
	}

	void Initialize () {

		SkyManager.instance.SetPhaseTimes (12, 15, 19.5f, 20.25f);
		InitializeDateTimes ();;
	}

	void InitializeDateTimes () {

		for (int i = 0; i < animals.Length; i++) {
			Vector2 spawnTime = animals[i].spawnTime;
			DateTime dateTime = new DateTime(SceneManager.currentDate.Year, 
			                                 9, 17, (int)spawnTime.x, (int)spawnTime.y, 0);
			animals[i].dateTime = dateTime;
		}
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
