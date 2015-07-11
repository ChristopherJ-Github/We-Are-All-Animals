using UnityEngine;
using System.Collections;

public class SaturationManager : Singleton <SaturationManager> {

	void Start () {

		saturationColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		saturationColorEffect.LutTexture = saturationFilter.LutTexture;
		SceneManager.instance.OnNewDay += UpdateSaturation;
	}

	public FilterInfo saturationFilter;
	private AmplifyColorEffect saturationColorEffect;
	public AnimationCurve saturationOverYear;
	void UpdateSaturation () {
		
		float saturationAmount = saturationOverYear.Evaluate (SceneManager.curvePos);
		float currentSaturation = Mathf.Lerp (saturationFilter.blend, 1, saturationAmount);
		float currentSatAfterSnow = Mathf.Lerp (currentSaturation, 1, SnowManager.instance.snowLevel);
		saturationColorEffect.BlendAmount = currentSatAfterSnow;
	}

	void Update () {

		UpdateSaturation ();//debug
	}
}
