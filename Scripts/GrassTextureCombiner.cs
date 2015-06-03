using UnityEngine;
using System.Collections;

public class GrassTextureCombiner : Singleton<GrassTextureCombiner> {

	[HideInInspector]
	public float blend;
	[HideInInspector]
	public Texture inputTex1, inputTex2;
	[HideInInspector]
	public bool activate;
	public Shader combiner;
	private Material combinerMat;
	private RenderTexture outputTexture;
	[HideInInspector]
	public Texture2D outputTexture2D;

	void Start () {
		
		Application.runInBackground = true;
		combinerMat = new Material (combiner);
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		outputTexture = new RenderTexture (GrassManager.instance.blockWidth, GrassManager.instance.blockWidth, 0);
	}
	
	void OnPostRender () {

		if (!activate)
			return;

		combinerMat.SetTexture ("_InputTex1", inputTex1);
		combinerMat.SetTexture ("_InputTex2", inputTex2);
		combinerMat.SetFloat ("_Blend", blend);
		outputTexture2D = new Texture2D (inputTex1.width, inputTex1.height);
		Graphics.Blit (inputTex1, outputTexture, combinerMat);
		RenderTexture.active = outputTexture;
		outputTexture2D.ReadPixels (new Rect (0, 0, outputTexture2D.width, outputTexture2D.height), 0, 0);
		outputTexture2D.Apply ();
		activate = false;
		//make sure to destroy outputTexture2D once it's retrieved
	}
}
