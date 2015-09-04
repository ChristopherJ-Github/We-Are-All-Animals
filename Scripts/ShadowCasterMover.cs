using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("SuperSplines/Animation/Regular Animator")]
public class ShadowCasterMover: MonoBehaviour {

	void Start () {

		initScaleVec = shadowCaster.transform.localScale;
	}

	public GameObject shadowCaster;
	public Spline spline;
	void Update() {

		float positionNorm = (float)SkyManager.instance.posInDay;
		float nodePosition = WrapValue (positionNorm, 0f, 1f, wrapMode);
		Move (nodePosition);
		Scale (nodePosition);
		ToggleRenderer ();
	}

	void Move (float nodePosition) {

		shadowCaster.transform.position = spline.GetPositionOnSpline(nodePosition);
		shadowCaster.transform.rotation = spline.GetOrientationOnSpline(nodePosition); 
	}

	private Vector3 initScaleVec;
	public float startScale, endScale;
	public AnimationCurve nodePosToScale;
	void Scale (float nodePosition) {

		Vector3 newScaleVec = initScaleVec;
		float scaleNormalized = nodePosToScale.Evaluate (nodePosition);
		float newScale = Mathf.Lerp (startScale, endScale, scaleNormalized);
		newScaleVec.x = newScale;
		shadowCaster.transform.localScale = newScaleVec;
	}

	public WrapMode wrapMode = WrapMode.Once;
	float WrapValue( float v, float start, float end, WrapMode wMode ) {
		
		switch( wMode ) {
		case WrapMode.Clamp:
		case WrapMode.ClampForever:
			return Mathf.Clamp(v, start, end);
		case WrapMode.Default:
		case WrapMode.Loop:
			return Mathf.Repeat(v, end - start) + start;
		case WrapMode.PingPong:
			return Mathf.PingPong(v, end - start) + start;
		default:
			return v;
		}
	}

	void ToggleRenderer () {

		shadowCaster.renderer.enabled = Tester.test;
	}
}
