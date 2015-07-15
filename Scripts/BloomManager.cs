using UnityEngine;
using System.Collections;

public class BloomManager : MonoBehaviour {

	void Start () {

		glowEffect = Camera.main.GetComponent<GlowEffect> ();
	}

	void Update () {

		SetGlowValues ();
	}

	public AnimationCurve tintOverYear;
	public float maxTint;
	private GlowEffect glowEffect;
	public AnimationCurve daytimeToBloom;
	void SetGlowValues () {

		float tintAmount = tintOverYear.Evaluate (SceneManager.curvePos);
		float darkening = 1 - Mathf.Clamp01(daytimeToBloom.Evaluate (SunControl.instance.posInDay));
		float tintAmountDarkened = Mathf.Lerp(tintAmount, 0, darkening);
		float currentTint = Mathf.Lerp (0, maxTint, tintAmountDarkened);
		float tintAfterSnow = Mathf.Lerp (currentTint, 0, SnowManager.instance.snowLevel);
		glowEffect.glowTint = Color.Lerp (Color.black, Color.white, tintAfterSnow);
	}
}
