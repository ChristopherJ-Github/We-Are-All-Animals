using UnityEngine;
using System.Collections;

public class NightLight : MonoBehaviour {

	public AnimationCurve brightnessCurve;
	public float maxBrightness;
	public SunProperties Sun;

	void Update () {

		light.intensity = brightnessCurve.Evaluate (1 - SkyManager.instance.intensityLerp * maxBrightness);
	}
}
