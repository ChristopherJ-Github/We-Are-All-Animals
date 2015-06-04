#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;
using System.Reflection ;

public class LeafFallManager : Singleton<LeafFallManager> {
	
	public ParticleEmitter _particleEmitter;
	void Start () {

		originalPosition = _particleEmitter.transform.position;
	}
	
	public void ChangeColor (Color treeColor) {

		_particleEmitter.renderer.material.color = treeColor;
	}
	
	void LateUpdate () {

		UpdateEmission ();
		UpdateVelocity ();
	}
	
	public AnimationCurve leafFallOverYear;
	public int minEmission, maxEmission;
	public int minEnergy, maxEnergy;
	void UpdateEmission () {
		
		float leafFall = leafFallOverYear.Evaluate (SceneManager.curvePos);
		int energy = (int)Mathf.Lerp (minEnergy, maxEnergy, WindControl.instance.windiness);
		_particleEmitter.maxEnergy = energy;
		int currentMinEmission = (int)Mathf.Lerp (0, minEmission, leafFall);
		int currentMaxEmission = (int)Mathf.Lerp (minEmission, maxEmission, leafFall);
		int emission = (int)Mathf.Lerp (currentMinEmission, currentMaxEmission, WindControl.instance.windiness);
		_particleEmitter.maxEmission = emission;
	}

	public float minSpeed, maxSpeed;
	public float horizontalShift, verticalShift;
	public AnimationCurve windinessToShift;
	private Vector3 originalPosition;
	public float transitionSpeed;
	void UpdateVelocity () {

		float windiness = WindControl.instance.windiness;
		Vector3 newPosition = originalPosition;
		float shiftAmount = windinessToShift.Evaluate (windiness);
		float currentHorizontalShift = Mathf.Lerp (0, horizontalShift, shiftAmount);
		float currentVericalShift = Mathf.Lerp (0, verticalShift, shiftAmount);
		newPosition += -WindControl.instance.direction * currentHorizontalShift;
		newPosition += Vector3.down * currentVericalShift;
		_particleEmitter.transform.position = newPosition;

		float speed = Mathf.Lerp (minSpeed, maxSpeed, windiness);
		Particle[] particles = _particleEmitter.particles;
		Vector3 targetDirection = Vector3.Lerp (Vector3.down, WindControl.instance.direction, windiness);
		Vector3 targetVelocity = targetDirection * speed;
		float currentTransitionSpeed = Mathf.Lerp (0, transitionSpeed, windiness) * Time.deltaTime;
		for (int i = 0; i < particles.Length; i++) {

			Particle particle = particles[i];
			particle.velocity = Vector3.MoveTowards(particle.velocity, targetVelocity, currentTransitionSpeed);
			particles[i] = particle;
		}
		_particleEmitter.particles = particles;
	}
}
#endif