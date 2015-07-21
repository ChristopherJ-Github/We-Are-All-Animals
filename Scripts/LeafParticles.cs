#if !UNITY_WEBPLAYER
using UnityEngine;
using System;
using System.Collections;

public class LeafParticles : MonoBehaviour {

	public float tintMultiplier;
	public void Init() {

		SceneManager.instance.OnNewDay += SetAllowLeaves;
		SceneManager.instance.OnNewDay += particleEmitter.ClearParticles;
		SetAllowLeaves ();
		renderer.material.SetFloat ("_TintMultiplier", tintMultiplier);
		particleAnimator = GetComponent<ParticleAnimator> ();
		originalPosition = transform.position;
		StartCoroutine (IntervalCountDown ());
	}

	public int startMonth, startDay;
	public int stopMonth, stopDay;
	private bool allowLeaves;
	void SetAllowLeaves () {
		
		DateTime currentDate = SceneManager.currentDate;
		DateTime startDate = new DateTime (currentDate.Year, startMonth, startDay);
		DateTime stopDate = new DateTime (currentDate.Year, stopMonth, stopDay);
		double minsAtStart = (startDate - SceneManager.yearStart).TotalMinutes;
		double minsAtStop = (stopDate - SceneManager.yearStart).TotalMinutes;
		double startPos = minsAtStart / SceneManager.minsInYear;
		double stopPos = minsAtStop / SceneManager.minsInYear;
		allowLeaves = SceneManager.curvePos >= startPos && SceneManager.curvePos <= stopPos;
	}
	
	public float minInterval, maxInterval;
	IEnumerator IntervalCountDown () {
		
		particleEmitter.maxEmission = 0;
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

		float curvePos = SceneManager.curvePos;
		float leafFall = leafFallOverYear.Evaluate (curvePos);
		int energy = (int)Mathf.Lerp (minEnergy, maxEnergy, windiness);
		particleEmitter.maxEnergy = energy;
		int emission = (int)Mathf.Lerp (minEmission, maxEmission, leafFall);
		particleEmitter.maxEmission = allowLeaves ? emission : 0;
	}

	void Update () {

		SetBrightness ();
	}

	public float nightDarkness;
	void SetBrightness () {
		
		Color nightColor = Color.Lerp (Color.white, Color.black, nightDarkness);
		float particleBrightness = SkyManager.instance.intensityLerp;
		Color currentTint = Color.Lerp (nightColor, Color.white, particleBrightness);
		particleEmitter.renderer.material.SetColor ("_BrightnessTint", currentTint);
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
		particleEmitter.transform.position = newPosition;
	}
	
	public float minSpeed, maxSpeed;
	public float minGravity, maxGravity;
	public AnimationCurve windToVelocity;
	private ParticleAnimator particleAnimator;
	public float minRandomization, maxRandomization;
	public AnimationCurve windToRandomization;
	void UpdateVelocity (float windiness) {

		float velocityAmount = windToVelocity.Evaluate (windiness);
		float speed = Mathf.Lerp (minSpeed, maxSpeed, velocityAmount);
		Vector3 currentHorizontalVelocity = WindControl.instance.direction * speed;
		float gravity = Mathf.Lerp (minGravity, maxGravity, velocityAmount);
		particleAnimator.force = Vector3.down * gravity + currentHorizontalVelocity;
		float randomizationAmount = windToRandomization.Evaluate (windiness);
		float currentRandomization = Mathf.Lerp (minRandomization, maxRandomization, randomizationAmount);
		particleAnimator.rndForce = (new Vector3 (1, 0, 1)) * currentRandomization;
	}
}
#endif