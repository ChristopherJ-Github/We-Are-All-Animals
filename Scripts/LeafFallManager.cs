#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;
using System.Reflection ;

public class LeafFallManager : Singleton<LeafFallManager> {
	
	public ParticleEmitter _particleEmitter;
	public ParticleAnimator particleAnimator;
	void Start () {

		originalPosition = _particleEmitter.transform.position;
		StartCoroutine (IntervalCountDown ());
	}

	public float minInterval, maxInterval;
	IEnumerator IntervalCountDown () {

		_particleEmitter.maxEmission = 0;
		float timePassed = 0;
		float currentDelay = Mathf.Lerp (minInterval, maxInterval, WindControl.instance.windiness);
		while (timePassed < currentDelay) {
			currentDelay = Mathf.Lerp (minInterval, maxInterval, WindControl.instance.windiness);
			timePassed += Time.deltaTime;
			yield return null;
		}
		StartCoroutine (SpawnParticles ());
	}

	IEnumerator SpawnParticles () {

		float timer = 1f;
		while (timer > 0) {
			float windiness = WindControl.instance.windiness;
			UpdateEmission (windiness);
			timer -= Time.deltaTime;
			yield return null;
		}
		StartCoroutine (IntervalCountDown ());
	}

	public AnimationCurve leafFallOverYear;
	public int minEmission, maxEmission;
	public int minEnergy, maxEnergy;
	void UpdateEmission (float windiness) {
		
		float leafFall = leafFallOverYear.Evaluate (SceneManager.curvePos);
		int energy = (int)Mathf.Lerp (minEnergy, maxEnergy, windiness);
		_particleEmitter.maxEnergy = energy;
		int emission = (int)Mathf.Lerp (minEmission, maxEmission, leafFall);
		_particleEmitter.maxEmission = emission;
	}
	
	void LateUpdate () {

		float windiness = WindControl.instance.windiness;
		ShiftSource (windiness);
		UpdateVelocity (windiness);
	}

	public float horizontalShift, verticalShift;
	public AnimationCurve windinessToShift;
	private Vector3 originalPosition;
	void ShiftSource (float windiness) {

		Vector3 newPosition = originalPosition;
		float shiftAmount = windinessToShift.Evaluate (windiness);
		float currentHorizontalShift = Mathf.Lerp (0, horizontalShift, shiftAmount);
		float currentVericalShift = Mathf.Lerp (0, verticalShift, shiftAmount);
		newPosition += -WindControl.instance.direction * currentHorizontalShift;
		newPosition += Vector3.down * currentVericalShift;
		_particleEmitter.transform.position = newPosition;
	}

	public float minSpeed, maxSpeed;
	public float gravity;
	void UpdateVelocity (float windiness) {

		float speed = Mathf.Lerp (minSpeed, maxSpeed, windiness);
		Vector3 currentHorizontalVelocity = Vector3.Lerp(Vector3.zero, WindControl.instance.direction * speed, windiness);
		particleAnimator.force = Vector3.down * gravity + currentHorizontalVelocity;
	}

	public void ChangeColor (Color treeColor) {
		
		_particleEmitter.renderer.material.color = treeColor;
	}
}
#endif