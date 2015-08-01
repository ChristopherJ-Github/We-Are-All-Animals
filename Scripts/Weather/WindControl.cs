using UnityEngine;
using System.Collections;

public class WindControl : Singleton<WindControl> {

	void OnEnable () {

#if !UNITY_WEBPLAYER
		WindZone = gameObject.AddComponent (typeof(ScriptableWindzoneInterface)) as ScriptableWindzoneInterface;
		WindZone.Init ();
#endif
		SceneManager.instance.OnNewMin += minUpdate; 
		SceneManager.instance.OnNewDay += RandomizeWindiness; 
		minUpdate ();
		RandomizeWindiness ();
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

	public AnimationCurve minWindOverYear, maxWindOverYear;
	public AnimationCurve likelyWindinessOverYear;
	public AnimationCurve likelyInfluence;
	public float maxDailyWindiness;
	void RandomizeWindiness () {
		
		float likelyWindiness = likelyWindinessOverYear.Evaluate (SceneManager.curvePos);
		float influence = likelyInfluence.Evaluate (Random.value);
		float minWindiness = minWindOverYear.Evaluate (SceneManager.curvePos);
		minWindiness = Mathf.Lerp (minWindiness, 0, SnowManager.instance.snowLevel);
		float maxWindiness = maxWindOverYear.Evaluate (SceneManager.curvePos);
		float randomWindiness = Mathf.Lerp (minWindiness, maxWindiness, Random.value);
		float windiness = Mathf.Lerp (randomWindiness, likelyWindiness, influence);
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

		float turbulence = Mathf.Lerp(minTurbulence, maxTurbulence, windiness);
		float mainWind = Mathf.Lerp(minMainWind, maxMainWind, windiness);
#if !UNITY_WEBPLAYER
		WindZone.WindMain = mainWind;
		WindZone.WindTurbulence = turbulence;
#endif
		TerrainData terrainData = terrain.terrainData;
		terrainData.wavingGrassAmount = Mathf.Lerp(minFlowerBending, maxFlowerBending, windiness);  //the variables names are off
		this.windiness = windiness;
	}
}