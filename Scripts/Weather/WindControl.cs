using UnityEngine;
using System.Collections;

public class WindControl : Singleton<WindControl> {

	void OnEnable () {

#if !UNITY_WEBPLAYER
		WindZone = gameObject.AddComponent (typeof(ScriptableWindzoneInterface)) as ScriptableWindzoneInterface;
		WindZone.Init ();
#endif
		SceneManager.instance.OnNewMin += minUpdate; 
		SceneManager.instance.OnNewDay += dayUpdate; 
		minUpdate ();
		dayUpdate ();
	}
	
	void minUpdate () {
		
		ChangeDirection ();
	}

	public void ChangeDirection (Vector3 _direction = default(Vector3)) {
		
		if (_direction == default(Vector3)) {
			_direction = Random.insideUnitSphere;
			_direction.y = 0;
			_direction = _direction.normalized;
		}
		StopAllCoroutines ();
		StartCoroutine (ChangeDirectionRoutine (_direction));
	}

	[HideInInspector] public Vector3 direction;
	public float directionChangeSpeed;
	IEnumerator ChangeDirectionRoutine (Vector3 _direction) {
		
		Quaternion initRotation = transform.rotation;
		Quaternion goalRotation = Quaternion.LookRotation (_direction);
		while (initRotation != goalRotation) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, directionChangeSpeed * Time.deltaTime);
			direction = transform.forward;
			yield return null;
		}
	}

	public AnimationCurve likelyWindinessOverYear;
	public AnimationCurve likelyInfluence;
	public float maxDailyWindiness;
	void dayUpdate () {
		
		float likelyWindiness = likelyWindinessOverYear.Evaluate (SceneManager.curvePos);
		float influence = likelyInfluence.Evaluate (Random.value);
		windiness = Mathf.Lerp (Random.value, likelyWindiness, influence) * maxDailyWindiness;
		SetValues (windiness);
	}

	private ScriptableWindzoneInterface WindZone;
	public Terrain terrain;
	public float minMainWind, maxMainWind;
	public float minTurbulence, maxTurbulence;
	public float minFlowerBending, maxFlowerBending;
	[HideInInspector] public float windiness; 
	public AnimationCurve windMultiplierOveryear;
	public float minWindMutliplier;
	public void SetValues (float windiness, float weatherSeverity = 0) {

		float multiplierAmount = windMultiplierOveryear.Evaluate (SceneManager.curvePos);
		multiplierAmount = Mathf.Lerp (multiplierAmount, 1, weatherSeverity);
		float amountAfterSnow = Mathf.Lerp (multiplierAmount, 1, SnowManager.instance.snowLevel);
		float windMultiplier = Mathf.Lerp (minWindMutliplier, 1, amountAfterSnow);
		float turbulence = Mathf.Lerp(minTurbulence, maxTurbulence, windiness);
		float mainWind = Mathf.Lerp(minMainWind, maxMainWind, windiness);
		turbulence *= windMultiplier;
		mainWind *= windMultiplier;
#if !UNITY_WEBPLAYER
		WindZone.WindMain = mainWind;
		WindZone.WindTurbulence = turbulence;
#endif
		TerrainData terrainData = terrain.terrainData;
		terrainData.wavingGrassAmount = Mathf.Lerp(minFlowerBending, maxFlowerBending, windiness) * windMultiplier;  //the variables names are off
		this.windiness = windiness;
	}
}