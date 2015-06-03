using UnityEngine;
using System.Collections;

public class TreeColorManager : Singleton<TreeColorManager>{

	void Start () {
		
		SceneManager.instance.OnNewDay += dayUpdate;
		dayUpdate ();
	}

	public Gradient colorOverYear;
	[HideInInspector] public Color currentColor;
	void dayUpdate () {
		
		currentColor = colorOverYear.Evaluate (SceneManager.curvePos);
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		terrainData.wavingGrassTint = currentColor;
#if !UNITY_WEBPLAYER
		LeafFallManager.instance.ChangeColor (currentColor);
#endif
	}

	public Gradient billboardLightingColor;
	void Update () {
		
		Color billboardColor = billboardLightingColor.Evaluate (1 - SkyManager.instance.intensityLerp);
		//Debug.Log (SunControl.instance.sun.intensityLerp);
		billboardColor = billboardLightingColor.Evaluate(1 - SkyManager.instance.nightDayLerp);
		Vector3 colorVector = new Vector3 (billboardColor.r, billboardColor.b, billboardColor.g);
		Shader.SetGlobalVector("tree_color", colorVector);
	}
}
