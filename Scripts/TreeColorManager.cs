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
		TriggerColorChange (currentColor);
	}

	public Gradient treeColorOverYear;
	[HideInInspector] public Color currentColor;
	void ChangeTreeColor () {

		currentColor = treeColorOverYear.Evaluate (SceneManager.curvePos);
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		terrainData.wavingGrassTint = currentColor;
	}

	public Gradient flowerColorOverYear;
	void ChangeFlowerColor () {
		
		Color currentColor = flowerColorOverYear.Evaluate (SceneManager.curvePos);
		Shader.SetGlobalColor ("_GrassTint", currentColor);
	}

	public delegate void colorHandler (Color color);
	public event colorHandler OnColorChange;
	public void TriggerColorChange (Color color) {

		if (OnColorChange != null)
			OnColorChange(color);
	}
	
	void Update () {

		SetBillboardLighting ();
	}

	public Gradient billboardLightingColor;
	void SetBillboardLighting () {

		Color billboardColor = billboardLightingColor.Evaluate (1 - SkyManager.instance.intensityLerp);
		billboardColor = billboardLightingColor.Evaluate(1 - SkyManager.instance.nightDayLerp);
		Vector3 colorVector = new Vector3 (billboardColor.r, billboardColor.b, billboardColor.g);
		Shader.SetGlobalVector("tree_color", colorVector);
	}
}
