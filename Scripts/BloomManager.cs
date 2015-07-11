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
	void SetGlowValues () {

		float tintAmount = tintOverYear.Evaluate (SceneManager.curvePos);
		float currentTint = Mathf.Lerp (0, maxTint, tintAmount);
		float tintAfterSnow = Mathf.Lerp (currentTint, 0, SnowManager.instance.snowLevel);
		glowEffect.glowTint = Color.Lerp (Color.black, Color.white, tintAfterSnow);
	}
}
