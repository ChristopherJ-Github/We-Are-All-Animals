using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour {
	
	void OnEnable () {

		SceneManager.instance.OnNewHour += SwitchAnimation;
		SwitchAnimation ();
	}
	
	public GameObject[] animations;
	private GameObject currentAnimation;
	void SwitchAnimation () {

		int randomIndex = Random.Range (0, animations.Length);
		currentAnimation = animations [randomIndex];
		currentAnimation.SetActive (true);
		foreach (GameObject animation in animations) {
			if (animation != currentAnimation) 
				animation.SetActive(false);
		}
	}

	void OnDisable () {

		foreach (GameObject animation in animations) 
			animation.SetActive(false);
	}
}
