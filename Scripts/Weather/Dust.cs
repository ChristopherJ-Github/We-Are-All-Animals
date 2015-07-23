using UnityEngine;
using System.Collections;

public class Dust : Singleton<Dust> {

	void OnEnable () {

		_particleEmitter = dust.GetComponent<ParticleEmitter> ();
		_particleAnimator = dust.GetComponent<ParticleAnimator> ();
		originalMatCol = dust.renderer.material.GetColor ("_TintColor");
		originalColors = _particleAnimator.colorAnimation;
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
	void CreateDust () {
		
		SetDustColors ();
		SetDustProperties ();
	}
	
	public float minSnowAlpha, maxSnowAlpha;
	public float minWhiteAlpha, maxWhiteAlpha;
	public float nightAlpha;
	public AnimationCurve datetimeToNightAlpha;
	private Color originalMatCol;
	private Color[] originalColors;
	public float minAlphaOffset, maxAlphaOffset;
	public float nightDarkness;
	public Color testColor;
	void SetDustColors () {
		
		Color[] newColors = new Color[originalColors.Length];
		System.Array.Copy (originalColors, newColors, originalColors.Length);
		float alphaOffset = Mathf.Lerp (minAlphaOffset, maxAlphaOffset, WindControl.instance.windiness);
		for (int i = 0; i < newColors.Length; i++) {
			newColors[i] = AddSnowTint(newColors[i]);
			if (i == 0 || i == newColors.Length - 1)
				continue;
			newColors[i].a += alphaOffset;
			newColors[i].a *= WeatherControl.instance.totalTransition;
		}
		_particleAnimator.colorAnimation = newColors;
		Color matColAfterSnow = AddSnowTint (originalMatCol);
		float particleWhiteAmount = (FogControl.instance.brightnessAmount + CloudControl.instance.overcast) / 2.0f;
		Color matColorAferWhite = Color.Lerp (matColAfterSnow, Color.white, particleWhiteAmount);
		Color matColNight = Color.Lerp (matColorAferWhite, Color.black, nightDarkness);
		float particleBrightness = AmbientLightingChanger.instance.GetParticleBrightness ();
		Color matColDarkened = Color.Lerp (matColNight, matColorAferWhite, particleBrightness);
		
		float snowAlpha = Mathf.Lerp (minSnowAlpha, maxSnowAlpha, WindControl.instance.windiness);
		//float whiteAlpha = Mathf.Lerp (minWhiteAlpha, maxWhiteAlpha, 
		matColDarkened.a = Mathf.Lerp (matColDarkened.a, snowAlpha, SnowManager.instance.snowLevel);
		dust.renderer.material.SetColor ("_TintColor", matColDarkened);
	}
	
	public float minSpeed, maxSpeed;
	public float normalScale, snowScale;
	public float normalEmission, snowEmission;
	void SetDustProperties () {
		
		float snowLevel = SnowManager.instance.snowLevel;
		float speed = Mathf.Lerp(minSpeed, maxSpeed, WindControl.instance.windiness);
		_particleEmitter.worldVelocity = WindControl.instance.direction * speed;
		float scale = Mathf.Lerp (normalScale, snowScale, snowLevel);
		_particleEmitter.minSize = Mathf.Clamp(scale - 1, 0, scale -1);
		_particleEmitter.maxSize = scale;
		float emission = Mathf.Lerp (normalEmission, snowEmission, snowLevel);
		_particleEmitter.minEmission = emission;
		_particleEmitter.maxEmission = emission;
	}
	
	public float snowTintOffset;
	Color AddSnowTint (Color toTint) {
		
		Color snowTint = Color.white;
		snowTint.a = toTint.a;
		float tintAmount = Mathf.InverseLerp (snowTintOffset, 1, SnowManager.instance.snowLevel);
		return Color.Lerp(toTint, snowTint, tintAmount);
	}
}
