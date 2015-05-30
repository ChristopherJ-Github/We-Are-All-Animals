using UnityEngine;
using System.Collections;

public class WindControl : Singleton<WindControl> {
	
	public AnimationCurve likelyWindinessOverYear;
	public AnimationCurve likelyInfluence;
	public float minMainWind, maxMainWind;
	public float minTurbulence, maxTurbulence;
	public float minFlowerBending, maxFlowerBending;
	public float directionChangeSpeed;
	public float maxDailyWindiness;

	[HideInInspector] public Vector3 direction; 
	[HideInInspector] public float windiness;
	private ScriptableWindzoneInterface WindZone; 

	public Terrain terrain;
	public GameObject dust;
	private ParticleEmitter _particleEmitter;
	private ParticleAnimator _particleAnimator;
	public float minAlphaOffset, maxAlphaOffset;
	public float minSpeed, maxSpeed;
	public float minScale, maxScale;
	private Color[] originalColors;
	private bool _createDust;
	public bool createDust {
		get {
			return _createDust;
		}
		set {
			_createDust = value;
			dust.SetActive(_createDust);
		}
	}
	
	void OnEnable () {
		
		WindZone = gameObject.AddComponent (typeof(ScriptableWindzoneInterface)) as ScriptableWindzoneInterface;//comment out for webbuild
		WindZone.Init ();//comment out for webbuild
		_particleEmitter = dust.GetComponent<ParticleEmitter> ();
		_particleAnimator = dust.GetComponent<ParticleAnimator> ();
		originalColors = _particleAnimator.colorAnimation;
		SceneManager.instance.OnNewMin += minUpdate; 
		SceneManager.instance.OnNewDay += dayUpdate; 
		minUpdate ();
		dayUpdate ();
	}
	
	void minUpdate () {
		
		ChangeDirection ();
	}
	
	void dayUpdate () {
		
		float likelyWindiness = likelyWindinessOverYear.Evaluate (SceneManager.curvePos);
		float influence = likelyInfluence.Evaluate (Random.value);
		
		windiness = Mathf.Lerp (Random.value, likelyWindiness, influence) * maxDailyWindiness;
		SetValues (windiness);
	}

	void Update () {

		if (createDust)
			CreateDust ();
	}

	void CreateDust () {
		
		float speed = Mathf.Lerp(minSpeed, maxSpeed, windiness);
		_particleEmitter.worldVelocity = WindControl.instance.direction * speed;

		float scale = Mathf.Lerp (minScale, maxScale, windiness);
		_particleEmitter.minSize = Mathf.Clamp(scale - 1, 0, scale -1);
		_particleEmitter.maxSize = scale;
		
		Color[] newColors = new Color[originalColors.Length];
		System.Array.Copy (originalColors, newColors, originalColors.Length);
		
		float alphaOffset = Mathf.Lerp (minAlphaOffset, maxAlphaOffset, windiness);
		for (int i = 0; i < newColors.Length; i++) {
			if (i == 0 || i == newColors.Length - 1)
				continue;
			newColors[i].a += alphaOffset;
			newColors[i].a *= WeatherControl.instance.totalTransition;
		}
		_particleAnimator.colorAnimation = newColors;
	}
	
	public void SetValues (float wnd) {
		
		float turbulence = Mathf.Lerp(minTurbulence, maxTurbulence , wnd);
		float mainWind = Mathf.Lerp(minMainWind, maxMainWind, wnd);
		
		WindZone.WindMain = mainWind;//comment out for webbuild
		WindZone.WindTurbulence = turbulence;//comment out for webbuild
		TerrainData terrainData = terrain.terrainData;
		terrainData.wavingGrassAmount = Mathf.Lerp(minFlowerBending, maxFlowerBending, wnd);  //the variables names are off
		windiness = wnd;
	}

	public void ChangeDirection (Vector3 _direction = default(Vector3)) {
		
		if (_direction == default(Vector3)) {
			_direction = Random.insideUnitSphere;
			_direction.y = 0;
			_direction = _direction.normalized;
		}
		direction = _direction;
		StopAllCoroutines ();
		StartCoroutine (ChangeDirectionRoutine (_direction));
	}
	
	IEnumerator ChangeDirectionRoutine (Vector3 _direction) {
		
		Quaternion initRotation = transform.rotation;
		Quaternion goalRotation = Quaternion.LookRotation (_direction);
		while (initRotation != goalRotation) {
			
			transform.rotation = Quaternion.RotateTowards(transform.rotation, goalRotation, directionChangeSpeed * Time.deltaTime);
			yield return null;
		}
	}
}
