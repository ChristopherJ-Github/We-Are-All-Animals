using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSpawner : Singleton<AnimationSpawner> {

	void OnEnable () {

		InitializeBirds ();
		SceneManager.instance.OnNewSec += BirdSpawnAttempt;
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
	
	public AnimationCurve birdSpawnChanceOverYear;
	public float minBirdSpawnChance, maxBirdSpawnChance;
	void BirdSpawnAttempt () {

		if (currentBirdAmount >= maxBirdsAllowed) return;
		float chanceNormalized = birdSpawnChanceOverYear.Evaluate (SceneManager.curvePos);
		float spawnChance = Mathf.Lerp (minBirdSpawnChance, maxBirdSpawnChance, chanceNormalized);
		float currentSpawnChance = Mathf.Lerp (spawnChance, 0, WeatherControl.instance.storminess);
		if (currentSpawnChance >= Random.Range(0.0f, 100.0f)) 
			SpawnBird();
	}
	
	private AnimalAnimator[] birds;
	[HideInInspector] public bool on = true;
	public int maxBirdsAllowed;
	[HideInInspector] public int currentBirdAmount;
	void SpawnBird (AnimalAnimator animalAnimator = null) {

		animalAnimator = animalAnimator ?? birds [Random.Range (0, birds.Length)];
		GameObject animal = Instantiate(animalAnimator.gameObject) as GameObject;
		AnimalAnimator animalAnimatorInst = animal.GetComponent<AnimalAnimator> ();
		animalAnimatorInst.allowIdling = false;
	}

	public float minuteSpawnChance;
	void MinuteSpawnAttempt () {

		MakeSpawnAttempt (minuteSpawnChance);
	}

	public float hourSpawnChance;
	void HourSpawnAttempt () {

		MakeSpawnAttempt (hourSpawnChance);
	}

	public int maxAllowed;
	[HideInInspector] public List<AnimalAnimator> currentAnimations;
	void MakeSpawnAttempt(float spawnChance) {
		
		if (!on) return;
		if (currentAnimations.Count - currentBirdAmount >= maxAllowed) return;
		float currentSpawnChance = Mathf.Lerp (spawnChance, 0, WeatherControl.instance.storminess);
		if (currentSpawnChance >= Random.Range(0.0f, 100.0f)) 
			Spawn ();
	}
	
	public void Spawn (AnimalAnimator animalAnimator = null) {
		
		animalAnimator = animalAnimator ?? GetRandomAnimal ();
		if (animalAnimator == null)
			return;
		Instantiate(animalAnimator.gameObject);
	}

	public AnimalAnimator[] animations;
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

	void Update () {

		if (Input.GetKey(KeyCode.P)) {
			ClearAnimations();
		}
	}

	public void SpawnAllAnimals () {

		for (int i = 0; i < animations.Length; i++) {
			AnimalAnimator animalAnimator = animations[i];
			Instantiate(animalAnimator.gameObject);
		}
	}

	public void ClearAnimations () {

		foreach (AnimalAnimator animation in currentAnimations) 
			animation.RemoveSelf(false);
		currentAnimations.Clear ();
	}

}
