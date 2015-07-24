using UnityEngine;
using System;
using System.Collections;

public class SaturationManager : Singleton <SaturationManager> {

	void Start () {

		saturationColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		saturationColorEffect.LutTexture = saturationFilter.LutTexture;
		CreateSaturationCurve ();
	}

	public Vector2 transInStart, transInEnd;
	public Vector2 transOutStart, transOutEnd;
	void CreateSaturationCurve () {

		DateTime currentDate = SceneManager.currentDate;
		DateTime transInStartDate = new DateTime (currentDate.Year, 4, 1, (int)transInStart.x, (int)transInStart.y, 0);
		DateTime transInEndDate = new DateTime (currentDate.Year, 4, 1, (int)transInEnd.x, (int)transInEnd.y, 0);
		saturationOverYear = new AnimationCurve ();

		//float springTransitionStart = 
		//saturationOverYear.AddKey(new Keyframe(
	}

	public FilterInfo saturationFilter;
	private AmplifyColorEffect saturationColorEffect;
	private AnimationCurve saturationOverYear;
	void UpdateSaturation () {
		
		float saturationAmount = saturationOverYear.Evaluate (SceneManager.curvePos);
		float currentSaturation = Mathf.Lerp (saturationFilter.blend, 1, saturationAmount);
		float currentSatAfterSnow = Mathf.Lerp (currentSaturation, 1, SnowManager.instance.snowLevel);
		saturationColorEffect.BlendAmount = currentSatAfterSnow;
	}

	void Update () {

		UpdateSaturation ();
	}
}
