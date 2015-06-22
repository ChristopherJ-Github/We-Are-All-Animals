using UnityEngine;
using System.Collections;

public class BackgroundTinter : MonoBehaviour {

	public AnimationCurve IntensityOverDay;
	public Color minIntensity;
	public Color maxIntensity;
	SunControl SunCtrl;
	float currentIntesity;

	// Use this for initialization
	void Start () {

		SunCtrl = SunControl.instance;

	}
	
	// Update is called once per frame
	void Update () {

		float curvePos = SceneManager.curvePos;
		currentIntesity = IntensityOverDay.Evaluate (SunCtrl.dayCurve.Evaluate (curvePos));
		currentIntesity = Mathf.Clamp (currentIntesity - CloudControl.instance.overcast, minIntensity.grayscale, maxIntensity.grayscale);
		Color col = Color.Lerp (minIntensity, maxIntensity, currentIntesity);
		renderer.material.SetColor ("_Color", col);

	}
}
