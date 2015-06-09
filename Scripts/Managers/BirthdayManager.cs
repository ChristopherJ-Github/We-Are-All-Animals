using UnityEngine;
using System.Collections;

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
			if (birthday.month == SceneManager.currentDate.Month &&
			    birthday.day == SceneManager.currentDate.Day) {
				ActivateBirthday(birthday);
			}
		}
	}

	void ActivateBirthday (BirthdayInfo birthday) {
		
		currentBirthday = birthday;
		if (currentBirthday.type == spawnType.Plant || currentBirthday.type == spawnType.Effect) //don't bother updating every minute if its a one time spawn thing
			currentBirthday.toSpawn.SetActive(true);
		if (currentBirthday.type == spawnType.Animal) 
			SceneManager.instance.OnNewMin += MakeAnimalSpawnAttempt;
	}

	private GameObject currentAnimal;
	void MakeAnimalSpawnAttempt () {

		if (Random.Range (0,100) < currentBirthday.spawnChance && currentAnimal == null) 
			currentAnimal = Instantiate(currentBirthday.toSpawn) as GameObject;
	}

	void DeactivateLastBirthday () {

		if (currentBirthday == null)
			return;
		if (currentBirthday.type == spawnType.Plant || currentBirthday.type == spawnType.Effect)
			currentBirthday.toSpawn.SetActive (false);
		if (currentBirthday.type == spawnType.Animal) 
			SceneManager.instance.OnNewMin += MakeAnimalSpawnAttempt; 
	}
}
