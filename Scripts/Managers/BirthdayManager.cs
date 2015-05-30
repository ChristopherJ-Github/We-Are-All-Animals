using UnityEngine;
using System.Collections;

public class BirthdayManager : Singleton<BirthdayManager> {
	

	public enum spawnType {Animal, Plant, Effect};

	[System.Serializable]
	public class bdayInfo 
	{ 
		public int month;
		public int day;
		public GameObject toSpawn;
		public spawnType type;
	}

	public bdayInfo[] birthdays;
	bdayInfo currentBDay;
	
	void Start () {

		SceneManager.instance.OnNewDay += dayUpdate;
		dayUpdate ();

	}

	void dayUpdate () { //make sure nothing is activated while activating birthday events 

		deactivateEverything ();

		SceneManager.instance.OnNewMin -= minUpdate;
		foreach (bdayInfo birthday in birthdays) {
			if (birthday.month == SceneManager.currentDate.Month &&
			    birthday.day == SceneManager.currentDate.Day) {
				currentBDay = birthday;
				SceneManager.instance.OnNewMin += minUpdate;
				minUpdate();
			}
		}

	}

	void updateBirthday () {

		if (currentBDay.type == spawnType.Plant || currentBDay.type == spawnType.Effect) { //don't bother updating every minute if its a one time spawn thing
			currentBDay.toSpawn.SetActive(true);
			SceneManager.instance.OnNewMin -= minUpdate; 
		}
		
		if (currentBDay.type == spawnType.Animal) { 
			
			AnimalAnimator animalComponent = currentBDay.toSpawn.GetComponent<AnimalAnimator> ();
			float currentChance = 100 * animalComponent.spawnChance.Evaluate(SceneManager.curvePosDay);
			if (Random.Range (0,100) < currentChance && !currentBDay.toSpawn.activeInHierarchy) 
				currentBDay.toSpawn.SetActive(true);
		}

	}

	void minUpdate () {

		updateBirthday ();
	}

	void deactivateEverything() {

		foreach (bdayInfo birthday in birthdays) 
			birthday.toSpawn.SetActive(false);	

		if (!SceneManager.mainScene.activeInHierarchy) {
			SceneManager.mainScene.SetActive(true);
			SceneManager.currentScene = SceneManager.currentScene;
		}

	}
}
