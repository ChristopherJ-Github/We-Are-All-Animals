#if !UNITY_WEBPLAYER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TerrainAlphaExporter : Singleton<TerrainAlphaExporter>{
	
	public int spacing;
	private TerrainData terrainData;
	private string path;
	float[,,] alphaMaps;
	Texture2D alphaMap;
	
	void Start () {
		
		terrainData = Terrain.activeTerrain.terrainData;
		path = Application.dataPath + "/Terrain Alpha Maps/Exported/";
	}
	
	public void extractAlphaMaps () {
		
		StopAllCoroutines ();
		alphaMap = new Texture2D(terrainData.alphamapWidth, terrainData.alphamapHeight);
		alphaMaps = terrainData.GetAlphamaps (0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
		SetupMap (0, alphaMaps);
	}

	void SetupMap(int map, float[,,] alphaMaps) {

		StartCoroutine(extractAlphaMap (alphaMaps, map));
	}
	
	IEnumerator extractAlphaMap (float[,,] alphaMaps, int mapIndx) {
		
		int x = 0;
		int i = 0;
		while (x < alphaMaps.GetLength(0)) {
			
			int y = 0;
			while (y < alphaMaps.GetLength(1)) {
				
				float a = alphaMaps[x,y,mapIndx];
				Color color = new Color(a, a, a, 1f);
				alphaMap.SetPixel(x, y, color);
				y++;
				i++;
				if (i % spacing == 0)
					yield return null;
			}
			GUIManager.instance.UpdateAlphaSavePercentage(alphaMaps.GetLength(1));
			x++;
		}
		alphaMap.Apply ();
		saveTexture (alphaMap, "AlphaMap" + mapIndx.ToString());

		if (mapIndx < alphaMaps.GetLength(2) - 1) {
			SetupMap(mapIndx + 1, alphaMaps);
		} else {
			alphaMaps = null;
			Destroy (alphaMap);
		}
		Debug.Log ("Terrain alpha map successfully extracted: " + path + "AlphaMap" + mapIndx.ToString());
	}
	
	void saveTexture (Texture2D texture, string name) {
		
		byte[] bytes = texture.EncodeToPNG (); 
		File.WriteAllBytes (path + name + ".png", bytes);
	}
	
}
#endif