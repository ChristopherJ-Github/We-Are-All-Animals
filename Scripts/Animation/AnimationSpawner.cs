using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSpawner : Singleton<AnimationSpawner> {
	
	public AnimalAnimator[] animations;
	private AnimalAnimator[] birds;
	[HideInInspector] public List<AnimalAnimator> currentAnimations;
	public int maxAllowed;
	[HideInInspector] public bool on = true;
	
	void OnEnable () {
		
		SceneManager.instance.OnNewMin += MinuteSpawnAttempt;
		SceneManager.instance.OnNewHour += HourSpawnAttempt;
		SceneManager.instance.OnNewDay += ClearAnimations;
	}

	void InitializeBirds () {

		List<AnimalAnimator> birds = new List<AnimalAnimator> ();
		foreach (AnimalAnimator animation in animations) {
			if (animation.bird)
				birds.Add(animation);
		}
		this.birds = birds.ToArray ();
	}

	void Update () {

		if (Input.GetKey(KeyCode.P)) {
			ClearAnimations();
		}
		if (Tester.buttonPressed)
			MinuteSpawnAttempt();
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
		AnimalAnimator animalAnimator = GetRandomAnimal();
		spawnChance = CalculateSpawnChance (spawnChance, ref animalAnimator);
		if (spawnChance >= Random.Range(0.0f, 100.0f)) 
			Spawn (animalAnimator);
	}

	public float birdChance;
	float CalculateSpawnChance (float originalChance, ref AnimalAnimator animalAnimator) {

		if (animalAnimator.bird) {
			if (birdChance >= Random.Range(0.0f, 100.0f)) {
				animalAnimator.dontIdle = true;
				return 100;
			}
		}
		return originalChance;
	}

	public void Spawn (AnimalAnimator animalAnimator) {

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
