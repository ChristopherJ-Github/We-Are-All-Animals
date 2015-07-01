using UnityEngine;
using System.Collections;

public class TreeColorManager : Singleton<TreeColorManager>{

	void Start () {
		
		SceneManager.instance.OnNewDay += dayUpdate;
		dayUpdate ();
	}
	
	void dayUpdate () {
		
		ChangeTreeColor ();
		ChangeFlowerColor ();
		UpdateBillboards ();
		TriggerColorChange (currentColor);
	}

	public Gradient treeColorOverYear;
	[HideInInspector] public Color currentColor;
	void ChangeTreeColor () {

		currentColor = treeColorOverYear.Evaluate (SceneManager.curvePos);
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
	void ChangeFlowerColor () {
		
		Color currentColor = flowerColorOverYear.Evaluate (SceneManager.curvePos);
		Shader.SetGlobalColor ("_GrassTint", currentColor);
		float saturation = saturationOverYear.Evaluate (SceneManager.curvePos);
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
		Vector3 colorVector = new Vector3 (billboardColor.r, billboardColor.b, billboardColor.g);
		Shader.SetGlobalVector("tree_color", colorVector);
	}

	public bool forceBillboardUpdate;
	void ForceBillboardUpdateCheck () {
		
		if (forceBillboardUpdate) {
			forceBillboardUpdate = false;
			StartCoroutine(MoveCamera());
		}
	}
}
