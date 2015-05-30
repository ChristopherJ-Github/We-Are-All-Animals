using UnityEngine;
using System.Collections;
using CurveExtended;

public class SunProperties : MonoBehaviour {
	
	public AnimationCurve IntensityOverDay;
	[HideInInspector]
	public float currentIntesity;
	public float minIntensity;
	public float maxIntensity;
	SunControl SunCtrl;
	private bool isSun;
	public AnimationCurve overcastInfluence;
	
	void Start () {
		
		SunCtrl = SunControl.instance;
		if (this == SkyManager.instance.sun) 
			isSun = true;
	}
	
	void Update () {
		
		float curvePos = SceneManager.curvePos;
		float posInDay = SunCtrl.dayCurve.Evaluate (curvePos);
		float darkness = Mathf.Lerp (0, maxIntensity, CloudControl.instance.overcast + CloudControl.instance.extraOvercast);
		currentIntesity = Mathf.Clamp (maxIntensity - darkness, minIntensity, maxIntensity);
	}
	
	public Color GetMiddayColor () {
		
		Color initMidday = SkyManager.instance.sunMiddayTint.Evaluate (CloudControl.instance.middayLerp);
		float divider = Mathf.Max (new float[]{initMidday.r, initMidday.g, initMidday.b});
		initMidday = initMidday/divider;
		initMidday = Color.Lerp (initMidday, Color.white, 0.4f);
		
		float _overcastInfluence = overcastInfluence.Evaluate(CloudControl.instance.overcast); 
		Color middayAfterCloud = Color.Lerp (initMidday, Color.white, _overcastInfluence);
		Color middayGrayscale = new Color (middayAfterCloud.grayscale, middayAfterCloud.grayscale, middayAfterCloud.grayscale);
		Color middayAfterGray = Color.Lerp (middayAfterCloud, middayGrayscale, CloudControl.instance.grayAmount);
		return middayAfterGray;
	}
	
}
