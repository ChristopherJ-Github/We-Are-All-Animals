#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;
using System.Reflection ;

public class LeafFallManager : Singleton<LeafFallManager> {
	
	public ParticleEmitter _particleEmitter;

	AnimationClip clip;
	void Start () {

		originalPosition = _particleEmitter.transform.position;
		clip = new AnimationClip ();
		animation.AddClip (clip, "Multiplier");
	}
	
	public void ChangeColor (Color treeColor) {

		_particleEmitter.renderer.material.color = treeColor;
	}
	
	void Update () {

		UpdateEmission ();
		UpdateVelocity ();
		InvokeTest ();
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
	public float maxHVelocity;
	public float maxShift;
	private Vector3 originalPosition;
	void UpdateVelocity () {

		Vector3 newPosition = originalPosition;
		float shift = Mathf.Lerp (0, maxShift, WindControl.instance.windiness);
		newPosition += -WindControl.instance.direction * shift;
		_particleEmitter.transform.position = newPosition;

		float speed = Mathf.Lerp (minSpeed, maxSpeed, WindControl.instance.windiness);
		float hVelocity = Mathf.Lerp (0, maxHVelocity, WindControl.instance.windiness);
		Vector3 velocity = WindControl.instance.direction * hVelocity;
		velocity.y = -1;
		velocity *= speed;
		_particleEmitter.worldVelocity = velocity;
	}
	
	private System.Type m_ParticleSystemType;
	public Component leafParticleSystem;
	public float multiplier;
	void InvokeTest () {

		/*
		leafParticleSystem = leafParticleSystem.GetComponent("ExternalForcesModule");
		m_ParticleSystemType = leafParticleSystem.GetType();
		
		MemberInfo[] members = m_ParticleSystemType.GetMembers (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach (MemberInfo member in members) {
			//Debug.Log(member.Name);
		}
		*/
		if (Input.GetKeyDown(KeyCode.T))
			multiplier = 0;
		if (Input.GetKeyDown(KeyCode.Y))
			multiplier = 3;

		AnimationCurve curve = AnimationCurve.Linear (0, multiplier, 0, multiplier);
		clip.ClearCurves ();
		clip.SetCurve ("", typeof(ParticleSystem), "ExternalForcesModule.multiplier", curve);
		animation.Stop ("Multiplier");
		animation.Play ("Multiplier");
	}
}
#endif