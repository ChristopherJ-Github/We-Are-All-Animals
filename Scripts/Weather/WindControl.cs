using UnityEngine;
using System.Collections;

public class WindControl : Singleton<WindControl> {

	void OnEnable () {

#if !UNITY_WEBPLAYER
		WindZone = gameObject.AddComponent (typeof(ScriptableWindzoneInterface)) as ScriptableWindzoneInterface;
		WindZone.Init ();
#endif
		_particleEmitter = dust.GetComponent<ParticleEmitter> ();
		_particleAnimator = dust.GetComponent<ParticleAnimator> ();
		originalMatCol = dust.renderer.material.GetColor ("_TintColor");
		originalColors = _particleAnimator.colorAnimation;
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
	public void SetValues (float wnd) {
		
		float turbulence = Mathf.Lerp(minTurbulence, maxTurbulence , wnd);
		float mainWind = Mathf.Lerp(minMainWind, maxMainWind, wnd);
#if !UNITY_WEBPLAYER
		WindZone.WindMain = mainWind;
		WindZone.WindTurbulence = turbulence;
#endif
		TerrainData terrainData = terrain.terrainData;
		terrainData.wavingGrassAmount = Mathf.Lerp(minFlowerBending, maxFlowerBending, wnd);  //the variables names are off
		windiness = wnd;
	}

	void Update () {
		
		if (createDust) 
			CreateDust ();
	}

	private bool _createDust;
	public bool createDust {
		get { return _createDust; }
		set {
			_createDust = value;
			dust.SetActive(_createDust);
		}
	}
	public GameObject dust;
	private ParticleEmitter _particleEmitter;
	private ParticleAnimator _particleAnimator;
	public float minAlphaOffset, maxAlphaOffset;
	public float minSpeed, maxSpeed;
	public float minScale, maxScale;
	void CreateDust () {
		
		float speed = Mathf.Lerp(minSpeed, maxSpeed, windiness);
		_particleEmitter.worldVelocity = direction * speed;
		float scale = Mathf.Lerp (minScale, maxScale, windiness);
		_particleEmitter.minSize = Mathf.Clamp(scale - 1, 0, scale -1);
		_particleEmitter.maxSize = scale;
		SetDustColors ();
	}
	
	public float minSnowAlpha, maxSnowAlpha;
	private Color originalMatCol;
	private Color[] originalColors;
	void SetDustColors () {

		Color[] newColors = new Color[originalColors.Length];
		System.Array.Copy (originalColors, newColors, originalColors.Length);
		float alphaOffset = Mathf.Lerp (minAlphaOffset, maxAlphaOffset, windiness);
		for (int i = 0; i < newColors.Length; i++) {
			newColors[i] = AddSnowTint(newColors[i]);
			if (i == 0 || i == newColors.Length - 1)
				continue;
			newColors[i].a += alphaOffset;
			newColors[i].a *= WeatherControl.instance.totalTransition;
		}
		_particleAnimator.colorAnimation = newColors;
		Color newMatCol = AddSnowTint (originalMatCol);
		float snowAlpha = Mathf.Lerp (minSnowAlpha, maxSnowAlpha, WindControl.instance.windiness);
		newMatCol.a = Mathf.Lerp (newMatCol.a, snowAlpha, SnowManager.instance.snowLevel);
		dust.renderer.material.SetColor ("_TintColor", newMatCol);
	}

	Color AddSnowTint (Color toTint) {

		Color snowTint = Color.white;
		snowTint.a = toTint.a;
		return Color.Lerp(toTint, snowTint, SnowManager.instance.snowLevel);
	}
}