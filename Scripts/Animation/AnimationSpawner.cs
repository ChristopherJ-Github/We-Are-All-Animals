using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSpawner : Singleton<AnimationSpawner> {
	
	public AnimalAnimator[] animations;
	[HideInInspector]
	public List<AnimalAnimator> currentAnimations;
	public int maxAllowed;
	public float chanceDivisor = 1f;
	
	void OnEnable () {
		
		SceneManager.instance.OnNewMin += minUpdate;
	}
	
	void minUpdate () {
		
		tryToSpawn ();
	}

	void Update () {

		if (Input.GetKey(KeyCode.P)) {
			ClearAnimations();
		}
	}
	
	void tryToSpawn() {

		for (int i = 0; i < maxAllowed - currentAnimations.Count; i++) {
			AnimalAnimator animalAnimator = animations[Random.Range(0, animations.Length)];
			if (Random.value < animalAnimator.spawnChance.Evaluate(SceneManager.curvePos)/chanceDivisor) 
				Instantiate(animalAnimator.gameObject);
		}
	}

	IEnumerator ForceSpawn () {

		while (currentAnimations.Count < maxAllowed) {
			tryToSpawn();
			yield return null;
		}
	}

	public void SpawnAllAnimals () {

		Debug.Log ("pass");
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
