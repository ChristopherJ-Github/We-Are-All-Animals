using UnityEngine;
using System.Collections;
using CurveExtended;

public class SunProperties : MonoBehaviour {
	
	private SunControl SunCtrl;
	private bool isSun;
	void Start () {
		
		SunCtrl = SunControl.instance;
		if (this == SkyManager.instance.sun) 
			isSun = true;
	}
	
	void Update () {
		
		UpdateIntensity ();
		UpdateShadowStrength ();
	}
	
	[HideInInspector] public float weatherDarkness;
	public float minIntensity, maxIntensity;
	[HideInInspector] public float currentIntesity;
	void UpdateIntensity () {

		float curvePos = SceneManager.curvePos;
		float posInDay = SunCtrl.dayCurve.Evaluate (curvePos);
		float darknessAmount = weatherDarkness + CloudControl.instance.extraOvercast + SnowManager.instance.snowLevel;
		float currentDarkness = Mathf.Lerp (0, maxIntensity, darknessAmount);
		currentIntesity = Mathf.Clamp (maxIntensity - currentDarkness, minIntensity, maxIntensity);
	}

	public float minShadowStrength = 0.81f;
	public float maxShadowStrength = 1.0f;
	void UpdateShadowStrength () {

		light.shadowStrength = Mathf.Lerp (maxShadowStrength, minShadowStrength, SnowManager.instance.snowLevel);
	}
	
	public AnimationCurve overcastInfluence;
	public Color GetMiddayColor () {
		
		Color initMidday = SkyManager.instance.sunMiddayTint.Evaluate (CloudControl.instance.middayValue);
		float divider = Mathf.Max (new float[]{initMidday.r, initMidday.g, initMidday.b});
		initMidday = initMidday/divider;
		initMidday = Color.Lerp (initMidday, Color.white, 0.4f);
		float _overcastInfluence = overcastInfluence.Evaluate(CloudControl.instance.overcast); 
		_overcastInfluence *= 1 - SnowManager.instance.snowLevel;
		Color middayAfterCloud = Color.Lerp (initMidday, Color.white, _overcastInfluence);
		Color middayGrayscale = new Color (middayAfterCloud.grayscale, middayAfterCloud.grayscale, middayAfterCloud.grayscale);
		Color middayAfterGray = Color.Lerp (middayAfterCloud, middayGrayscale, CloudControl.instance.grayAmount);
		return middayAfterGray;
	}
	
}
