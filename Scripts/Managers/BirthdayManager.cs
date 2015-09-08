using UnityEngine;
using System.Collections;
using System;

public class BirthdayManager : Singleton<BirthdayManager> {

	public enum spawnType {Animal, Plant, Effect};
	[System.Serializable]
	public class BirthdayInfo { 

		public int month;
		public int day;
		public GameObject toSpawn;
		public spawnType type;
		public float spawnChance = 100;
	}
	
	void Start () {

		SceneManager.instance.OnNewDay += CheckForBirthday;
		CheckForBirthday ();
	}

	public BirthdayInfo[] birthdays;
	private BirthdayInfo currentBirthday;
	void CheckForBirthday () { 

		DeactivateLastBirthday ();
		foreach (BirthdayInfo birthday in birthdays) {
			if (birthday.month == SceneManager.realDate.Month &&
			    birthday.day == SceneManager.realDate.Day) {
				ActivateBirthday(birthday);
			}
		}
	}

	void ActivateBirthday (BirthdayInfo birthday) {

		currentBirthday = birthday;
		if (currentBirthday.type == spawnType.Plant || currentBirthday.type == spawnType.Effect) //don't bother updating every minute if its a one time spawn thing
			currentBirthday.toSpawn.SetActive(true);
		if (currentBirthday.type == spawnType.Animal) {
			AnimationSpawner.instance.on = false;
			AnimationSpawner.instance.ClearAnimations();
			SceneManager.instance.OnNewMin += MakeAnimalSpawnAttempt;
			MakeAnimalSpawnAttempt();
		}
	}

	private AnimalAnimator currentAnimal;
	void MakeAnimalSpawnAttempt () {

		if (UnityEngine.Random.Range (0,100) < currentBirthday.spawnChance && currentAnimal == null) {
			GameObject animalObj = Instantiate(currentBirthday.toSpawn) as GameObject;
			currentAnimal = animalObj.GetComponent<AnimalAnimator> ();
		}
	}

	void DeactivateLastBirthday () {

		if (currentBirthday == null)
			return;
		if (currentBirthday.type == spawnType.Plant || currentBirthday.type == spawnType.Effect)
			currentBirthday.toSpawn.SetActive (false);
		if (currentBirthday.type == spawnType.Animal) {
			if (currentAnimal != null)
				currentAnimal.RemoveSelf();
			AnimationSpawner.instance.on = true;
			SceneManager.instance.OnNewMin -= MakeAnimalSpawnAttempt; 
		}
	}

	void Update () {

		if (Tester.buttonPressed) {
			MakeAnimalSpawnAttempt();
		}
	}
}
