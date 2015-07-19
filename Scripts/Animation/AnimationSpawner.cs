using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSpawner : Singleton<AnimationSpawner> {
	
	public AnimalAnimator[] animations;
	[HideInInspector] public List<AnimalAnimator> currentAnimations;
	public int maxAllowed;
	[HideInInspector] public bool on = true;
	
	void OnEnable () {
		
		SceneManager.instance.OnNewMin += MinuteSpawnAttempt;
		SceneManager.instance.OnNewHour += HourSpawnAttempt;
	}

	void Update () {

		if (Input.GetKey(KeyCode.P)) {
			ClearAnimations();
		}
	}

	public float minuteSpawnChance;
	void MinuteSpawnAttempt () {

		MakeSpawnAttempt (minuteSpawnChance);
	}

	public float hourSpawnChance;
	void HourSpawnAttempt () {

		MakeSpawnAttempt (hourSpawnChance);
	}

	void MakeSpawnAttempt(float spawnChance) {

		if (!on || WeatherControl.instance.storm || currentAnimations.Count >= maxAllowed) 
			return;
		if (spawnChance >= Random.Range(0.0f, 100.0f)) 
			RandomlySpawn ();
	}

	public void RandomlySpawn () {

		AnimalAnimator animalAnimator = GetRandomAnimal();
		if (animalAnimator == null)
			return;
		Instantiate(animalAnimator.gameObject);
	}

	AnimalAnimator GetRandomAnimal () {
		
		float[] cSum = new float[animations.Length];
		float total = 0;
		for (int i = 0; i < cSum.Length; i++) {
			float currentChance = animations[i].spawnChance.Evaluate(SceneManager.curvePos);
			currentChance = Mathf.Clamp01(currentChance);
			total += currentChance;
			cSum[i] = total;
		}
		float randomAnimationVal = Random.Range (0, total);
		if (total == 0)
			return null;
		for (int i = 0; i < cSum.Length; i++) {
			if (cSum[i] >= randomAnimationVal) 
				return animations[i];
		}
		return null; //shouldn't be reached
	}

	public void SpawnAllAnimals () {

		for (int i = 0; i < animations.Length; i++) {
			AnimalAnimator animalAnimator = animations[i];
			Instantiate(animalAnimator.gameObject);
		}
	}

	public void ClearAnimations () {

		foreach (AnimalAnimator animation in currentAnimations) 
			Destroy(animation.gameObject);
		currentAnimations.Clear ();
	}

}
