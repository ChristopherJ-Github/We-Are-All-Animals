using UnityEngine;
using System.Collections;
using AmplifyColor;

public class FilterTester : MonoBehaviour {
	
	void Start () {
		
		amplifyColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		SetFilter ();
	}

	private AmplifyColorEffect amplifyColorEffect;
	void SetFilter () {

		amplifyColorEffect.LutTexture = filter.LutTexture;
	}
	
	void Update () {

		UpdateBlend ();
		if (Application.isEditor)
			UpdateLut ();
	}

	public float _blend;
	public float blendNormalized;
	public float blend {
		get { return blendNormalized; } 
		set { 
			blendNormalized = value;
			_blend = Mathf.Lerp(filter.blend, 1, blendNormalized);
		}
	}
	
	public FilterInfo filter;
	public bool testMode = true;
	void UpdateBlend () {

		if (testMode)
			blendNormalized = 1 - Tester.instance.testValue01;
		blend = blendNormalized;
		amplifyColorEffect.BlendAmount = _blend;
	}

	void UpdateLut () {

		amplifyColorEffect.LutTexture = filter.LutTexture;
	}
}
