using UnityEngine;
using System.Collections;

public class FogControl : Singleton<FogControl> {

	void Start () {
		
		RenderSettings.fog = true;
		SceneManager.instance.OnNewDay += RandomizeFog;
		RandomizeFog ();
	}

	public AnimationCurve minFogOverYear, maxFogOverYear;
	public float minDesnity, maxDensity;
	void RandomizeFog () {
		
		float minFog = minFogOverYear.Evaluate (SceneManager.curvePos);
		float maxFog = maxFogOverYear.Evaluate (SceneManager.curvePos);
		float fogDensity = Random.Range (minFog, maxFog);
		SetFogDesnity(Mathf.Lerp (minDesnity, maxDensity, fogDensity));
		SetGlobalFog (false);
	}
	
	public void SetFogDesnity (float density) {
		
		RenderSettings.fogDensity = density;
	}

	void Update () {
		
		SetMidayColor ();
	}

	public Gradient _midday;
	public AnimationCurve overcastInfluence;
	public Color middayCloudy;
	public Color middayStorm;
	public Color middaySnow;
	[HideInInspector] public Color midday;
	void SetMidayColor () {

		Color initMidday = _midday.Evaluate (CloudControl.instance.middayValue);
		float _overcastInfluence = overcastInfluence.Evaluate(CloudControl.instance.overcast); 
		Color middayAfterCloud = Color.Lerp (initMidday, middayCloudy, _overcastInfluence);
		Color middayAfterStorm = Color.Lerp (middayAfterCloud, middayStorm, CloudControl.instance.grayAmount);
		Color middayAfterSnow = Color.Lerp (middayAfterStorm, middaySnow, SnowManager.instance.snowLevel * _overcastInfluence);
		midday = middayAfterSnow;
	}

	public Gradient nightToDusk;
	public Color NightToDusk (float lerp) {
		
		Color initColor = nightToDusk.Evaluate (lerp);
		Color grayscale = new Color (initColor.grayscale, initColor.grayscale, initColor.grayscale);
		Color afterGray = Color.Lerp (initColor, grayscale, CloudControl.instance.grayAmount);
		return afterGray;
	}

	public GameObject globalFogActivator;
	public void SetGlobalFog (bool active) {
		
		globalFogActivator.SetActive (active);
	}
}
