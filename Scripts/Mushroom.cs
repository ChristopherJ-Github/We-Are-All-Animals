using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour {

	void Awake () {

		initScale = transform.localScale.x;
	}

	void Update () {

		UpdateScale ();
	}

	private float initScale;
	public float minScale;
	public float maxScale = 1;
	void UpdateScale () {

		float scaleAmount = Mathf.InverseLerp(0.24f, 0.85f, (float)SkyManager.instance.posInDay);
		float minScale = this.minScale != 0 ? this.minScale/100 : initScale;
		float currentScalar = Mathf.Lerp (minScale, maxScale/100, scaleAmount);
		transform.localScale = Vector3.one * currentScalar;
	}

}
