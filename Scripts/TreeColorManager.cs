using UnityEngine;
using System.Collections;

public class TreeColorManager : Singleton<TreeColorManager>{
	
	public Gradient colorOverYear;
	[HideInInspector] public Color currentColor;
	public Gradient billboardLightingColor;
	
	void Start () {
		
		SceneManager.instance.OnNewDay += dayUpdate;
		dayUpdate ();
	}
	
	void dayUpdate () {
		
		currentColor = colorOverYear.Evaluate (SceneManager.curvePos);
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		terrainData.wavingGrassTint = currentColor;
	}
	
	void Update () {
		
		Color billboardColor = billboardLightingColor.Evaluate (1 - SkyManager.instance.intensityLerp);
		//Debug.Log (SunControl.instance.sun.intensityLerp);
		billboardColor = billboardLightingColor.Evaluate(1 - SkyManager.instance.nightDayLerp);
		Vector3 colorVector = new Vector3 (billboardColor.r, billboardColor.b, billboardColor.g);
		Shader.SetGlobalVector("tree_color", colorVector);
	}
}
