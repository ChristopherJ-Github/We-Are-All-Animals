using UnityEngine;
using System;
using System.Collections;

public class SaturationManager : Singleton <SaturationManager> {

	void Start () {

		saturationColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		saturationColorEffect.LutTexture = saturationFilter.LutTexture;
		SceneManager.instance.OnNewYear += CreateSaturationCurve;
		CreateSaturationCurve ();
	}

	[Tooltip("x = hour, y = minute")]
	public Vector2 transInStart, transInEnd;
	public Vector2 transOutStart, transOutEnd;
	void CreateSaturationCurve () {

		DateTime currentDate = SceneManager.currentDate;
		DateTime transInStartDate = new DateTime (currentDate.Year, 4, 1, (int)transInStart.x, (int)transInStart.y, 0);
		DateTime transInEndDate = new DateTime (currentDate.Year, 4, 1, (int)transInEnd.x, (int)transInEnd.y, 0);
		DateTime transOutStartDate = new DateTime (currentDate.Year, 11, 14, (int)transOutStart.x, (int)transOutStart.y, 0);
		DateTime transOutEndDate = new DateTime (currentDate.Year, 11, 14, (int)transOutEnd.x, (int)transOutEnd.y, 0);
		float transInStartPos = SceneManager.instance.DateToPosition (transInStartDate);
		float transInEndPos = SceneManager.instance.DateToPosition (transInEndDate);
		float transOutStartPos = SceneManager.instance.DateToPosition (transOutStartDate);
		float transOutEndPos = SceneManager.instance.DateToPosition (transOutEndDate);
		saturationOverYear = new AnimationCurve ();
		saturationOverYear.AddKey (new Keyframe (0, 0));
		saturationOverYear.AddKey (new Keyframe (transInStartPos, 0));
		saturationOverYear.AddKey (new Keyframe (transInEndPos, 1));
		saturationOverYear.AddKey (new Keyframe (transOutStartPos, 1));
		saturationOverYear.AddKey (new Keyframe (transOutEndPos, 0));
		saturationOverYear.AddKey (new Keyframe (1, 0));
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
