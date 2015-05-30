using UnityEngine;
using System.Collections;

public class FogControl : Singleton<FogControl> {
	
	public Gradient nightToDusk;
	public Gradient _midday;
	public Color middayCloudy;
	public Color middayFullCloud;
	[HideInInspector] 
	public Color midday;
	public GameObject globalFogActivator;
	
	public AnimationCurve minFogOverYear, maxFogOverYear;
	public float maxDensity;
	public float minDesnity;
	public AnimationCurve overcastInfluence;
	
	void Start () {
		
		RenderSettings.fog = true;
		SceneManager.instance.OnNewDay += dayUpdate;
		dayUpdate ();
	}
	
	void dayUpdate () {
		
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
		
		Color initMidday = _midday.Evaluate (CloudControl.instance.middayLerp);
		float _overcastInfluence = overcastInfluence.Evaluate(CloudControl.instance.overcast); 
		Color middayAfterCloud = Color.Lerp (initMidday, middayCloudy, _overcastInfluence);
		/*
		Color middayGrayscale = new Color (middayAfterCloud.grayscale, middayAfterCloud.grayscale, middayAfterCloud.grayscale);
		Color middayAfterGray = Color.Lerp (middayAfterCloud, middayGrayscale, CloudControl.instance.grayAmount);
		*/
		Color middayAfterFullCloud = Color.Lerp (middayAfterCloud, middayFullCloud, CloudControl.instance.grayAmount);

		midday = middayAfterFullCloud;
	}
	
	public Color NightToDusk (float lerp) {
		
		Color initColor = nightToDusk.Evaluate (lerp);
		Color grayscale = new Color (initColor.grayscale, initColor.grayscale, initColor.grayscale);
		Color afterGray = Color.Lerp (initColor, grayscale, CloudControl.instance.grayAmount);
		return afterGray;
	}
	
	public void SetGlobalFog (bool active) {
		
		globalFogActivator.SetActive (active);
	}
}
