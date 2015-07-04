#if !UNITY_WEBPLAYER
using UnityEngine;
using System;
using System.Collections;

public class LeafParticles : MonoBehaviour {
	
	public void Init() {

		TreeColorManager.instance.OnColorChange += ChangeColor;
		SceneManager.instance.OnNewDay += SetAllowLeaves;
		SetAllowLeaves ();
		particleAnimator = GetComponent<ParticleAnimator> ();
		originalPosition = transform.position;
		ChangeColor (TreeColorManager.instance.currentColor);
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
		
		Color nightColor = Color.Lerp (colorForTheDay, Color.black, nightDarkness);
		float particleBrightness = SkyManager.instance.intensityLerp;
		Color currentColor = Color.Lerp (nightColor, colorForTheDay, particleBrightness);
		particleEmitter.renderer.material.SetColor ("_TintColor", currentColor);
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
	private ParticleAnimator particleAnimator;
	public float minRandomization, maxRandomization;
	void UpdateVelocity (float windiness) {
		
		float speed = Mathf.Lerp (minSpeed, maxSpeed, windiness);
		Vector3 currentHorizontalVelocity = WindControl.instance.direction * speed;
		float gravity = Mathf.Lerp (minGravity, maxGravity, windiness);
		particleAnimator.force = Vector3.down * gravity + currentHorizontalVelocity;
		float currentRandomization = Mathf.Lerp (minRandomization, maxRandomization, windiness);
		particleAnimator.rndForce = (new Vector3 (1, 0, 1)) * currentRandomization;
	}

	public Color originalColor;
	private Color colorForTheDay;
	public void ChangeColor (Color treeColor) {

		Color newColor = Color.white;
		newColor.r = Mathf.Clamp01(originalColor.r - 1 + treeColor.r);
		newColor.g = Mathf.Clamp01(originalColor.g - 1 + treeColor.g);
		newColor.b = Mathf.Clamp01(originalColor.b - 1 + treeColor.b);
		particleEmitter.renderer.material.SetColor ("_TintColor", newColor);
		colorForTheDay = newColor;
	}
}
#endif