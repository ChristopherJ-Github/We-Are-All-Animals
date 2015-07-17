using UnityEngine;
using System.Collections;
using AmplifyColor;

public class DarkFilterManager : Singleton<DarkFilterManager> {
	
	void Start () {

		amplifyColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		amplifyColorEffect.LutTexture = filter.LutTexture;
	}
	void Update () {

		UpdateFilter ();
	}
	
	public FilterInfo filter;
	public static AmplifyColorEffect amplifyColorEffect;
	void UpdateFilter () {
		
		float newBlend = Mathf.Lerp (1, filter.blend, 1 - SkyManager.instance.intensityLerp);
		amplifyColorEffect.BlendAmount = newBlend;
	}
}
