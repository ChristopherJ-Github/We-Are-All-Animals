#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;

public class LeafFallManager : Singleton<LeafFallManager> {

	public float minInterval, maxInterval;
	public AnimationCurve leafFallOverYear;
	public int startMonth, startDay;
	public int stopMonth, stopDay; 
	public int minEmission, maxEmission;
	public int minEnergy, maxEnergy;
	public float nightDarkness;
	public float horizontalShift, verticalShift;
	public AnimationCurve windinessToShift;
	public float minSpeed, maxSpeed;
	public float minGravity, maxGravity;
	public AnimationCurve windToVelocity;
	public float minRandomization, maxRandomization;
	public AnimationCurve windToRandomization;

	void Start () {

		CreateLeafParticles ();
		UpdateLeafAmount ();
	}

	public float[] tintMultipliers;
	public GameObject particleBase;
	void CreateLeafParticles () {

		int copyCount = tintMultipliers.Length;
		foreach (float tintMultiplier in tintMultipliers) {
			GameObject particleCopy = Instantiate(particleBase) as GameObject;
			particleCopy.transform.parent = transform;
			particleCopy.transform.position = particleBase.transform.position;
			InitializeLeafParticles(particleCopy, copyCount, tintMultiplier);
		}
		Destroy (particleBase);
	}

	void InitializeLeafParticles (GameObject particleCopy, int copyCount, float tintMultiplier) {

		LeafParticles leafParticles = particleCopy.AddComponent<LeafParticles>();
		leafParticles.minInterval = minInterval;
		leafParticles.maxInterval = maxInterval;
		leafParticles.leafFallOverYear = leafFallOverYear;
		leafParticles.startMonth = startMonth;
		leafParticles.startDay = startDay;
		leafParticles.stopMonth = stopMonth;
		leafParticles.stopDay = stopDay;
		leafParticles.minEmission = minEmission / copyCount;
		leafParticles.maxEmission = maxEmission / copyCount;
		leafParticles.minEnergy = minEnergy;
		leafParticles.maxEnergy = maxEnergy;
		leafParticles.nightDarkness = nightDarkness;
		leafParticles.horizontalShift = horizontalShift;
		leafParticles.verticalShift = verticalShift;
		leafParticles.windinessToShift = windinessToShift;
		leafParticles.minSpeed = minSpeed;
		leafParticles.maxSpeed = maxSpeed;
		leafParticles.minGravity = minGravity;
		leafParticles.maxGravity = maxGravity;
		leafParticles.windToVelocity = windToVelocity;
		leafParticles.minRandomization = minRandomization;
		leafParticles.maxRandomization = maxRandomization;
		leafParticles.windToRandomization = windToRandomization;
		leafParticles.tintMultiplier = tintMultiplier;
		leafParticles.Init ();
	}

	void Update () {

		UpdateLeafAmount ();
	}

	public AnimationCurve leafAmountOverYear;
	[HideInInspector] public float leafAmount;
	public static bool thereAreLeaves;
	void UpdateLeafAmount () {

		leafAmount = leafAmountOverYear.Evaluate (SceneManager.curvePos);
		thereAreLeaves = leafAmount > 0;
	}


}
#endif