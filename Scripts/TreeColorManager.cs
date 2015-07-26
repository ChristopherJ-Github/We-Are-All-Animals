using UnityEngine;
using System.Collections;

public class TreeColorManager : Singleton<TreeColorManager>{

	void Start () {
		
		SceneManager.instance.OnNewDay += DayUpdate;
		DayUpdate ();
	}
	
	void DayUpdate () {
		
		ChangeTreeColor ();
		ChangeFlowerColor ();
		UpdateBillboards ();
		TriggerColorChange (currentColor);
	}

	public Color fallTint;
	public AnimationCurve fallTintOverYear;
	[HideInInspector] public Color currentColor;
	void ChangeTreeColor () {

		currentColor = fallTint;
		float fallTintAmount = fallTintOverYear.Evaluate (SceneManager.curvePos);
		Shader.SetGlobalFloat ("_FallTintAmount", fallTintAmount);
		Shader.SetGlobalColor ("_FallTint", fallTint);
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		terrainData.wavingGrassTint = currentColor;
	}

	public bool updateBillboards;
	void UpdateBillboards () {
		
		if (updateBillboards)
			StartCoroutine (MoveCamera ());
	}
	
	IEnumerator MoveCamera () {
		
		Transform mainCamera = Camera.main.transform;
		Vector3 euler = mainCamera.transform.eulerAngles;
		float shift = 0.5f;
		euler.y += shift;
		mainCamera.transform.rotation = Quaternion.Euler (euler);
		yield return new WaitForEndOfFrame ();
		euler = mainCamera.transform.eulerAngles;
		euler.y -= shift;
		mainCamera.transform.rotation = Quaternion.Euler (euler);
	}

	public Gradient flowerColorOverYear;
	public AnimationCurve saturationOverYear;
	public float minSaturation;
	void ChangeFlowerColor () {
		
		Color currentColor = flowerColorOverYear.Evaluate (SceneManager.curvePos);
		Shader.SetGlobalColor ("_GrassTint", currentColor);
		float saturationAmount = saturationOverYear.Evaluate (SceneManager.curvePos);
		float saturation = Mathf.Lerp (minSaturation, 1, saturationAmount);
		Shader.SetGlobalFloat ("_Saturation", saturation);
	}

	public delegate void colorHandler (Color color);
	public event colorHandler OnColorChange;
	public void TriggerColorChange (Color color) {

		if (OnColorChange != null)
			OnColorChange(color);
	}

	void Update () {

		ChangeTreeColor ();//debug
		ChangeFlowerColor (); //debug
		SetBillboardLighting ();
		ForceBillboardUpdateCheck ();
	}

	public Gradient billboardLightingColor;
	void SetBillboardLighting () {

		Color billboardColor = billboardLightingColor.Evaluate (1 - SkyManager.instance.intensityLerp);
		billboardColor = billboardLightingColor.Evaluate(1 - SkyManager.instance.nightDayLerp);
		Shader.SetGlobalColor ("tree_color", billboardColor);
	}

	public bool forceBillboardUpdate;
	void ForceBillboardUpdateCheck () {
		
		if (forceBillboardUpdate) {
			forceBillboardUpdate = false;
			StartCoroutine(MoveCamera());
		}
	}
}
