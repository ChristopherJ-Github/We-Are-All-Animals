#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;
using System.Reflection ;

public class LeafFallManager : Singleton<LeafFallManager> {
	
	public ParticleEmitter _particleEmitter;
	public ParticleAnimator particleAnimator;
	void Start () {

		originalPosition = _particleEmitter.transform.position;
	}
	
	public void ChangeColor (Color treeColor) {

		_particleEmitter.renderer.material.color = treeColor;
	}
	
	void LateUpdate () {

		float windiness = WindControl.instance.windiness;
		UpdateEmission (windiness);
		ShiftSource (windiness);
		UpdateVelocity (windiness);
	}
	
	public AnimationCurve leafFallOverYear;
	public int minEmission, maxEmission;
	public int minEnergy, maxEnergy;
	void UpdateEmission (float windiness) {
		
		float leafFall = leafFallOverYear.Evaluate (SceneManager.curvePos);
		int energy = (int)Mathf.Lerp (minEnergy, maxEnergy, windiness);
		_particleEmitter.maxEnergy = energy;
		int currentMinEmission = (int)Mathf.Lerp (0, minEmission, leafFall);
		int currentMaxEmission = (int)Mathf.Lerp (minEmission, maxEmission, leafFall);
		int emission = (int)Mathf.Lerp (currentMinEmission, currentMaxEmission, WindControl.instance.windiness);
		_particleEmitter.maxEmission = emission;
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
}
#endif