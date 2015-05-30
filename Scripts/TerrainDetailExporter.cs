using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TerrainDetailExporter : Singleton<TerrainDetailExporter> {//comment out for web build
	
	public int spacing;
	private TerrainData terrainData;
	private string path;
	
	void Start () {
		
		terrainData = Terrain.activeTerrain.terrainData;
		path = Application.dataPath + "/Terrain Detail Maps/Exported/";
	}
	
	public void extractDetailMaps () {
		
		StopAllCoroutines ();
		SetupMap (0);
	}

	void SetupMap (int map) {

		int[,] detailMap = terrainData.GetDetailLayer (0, 0, terrainData.detailWidth, terrainData.detailHeight, map);
		StartCoroutine(extractDetailMap(detailMap, map));
	}
	
	IEnumerator extractDetailMap(int[,] detailMap, int map) {
		
		Texture2D detailTex = new Texture2D(terrainData.detailWidth,  terrainData.detailHeight);
		
		int x = 0;
		int i = 0;
		while (x < detailMap.GetLength(0)) {
			
			int y = 0;
			while (y < detailMap.GetLength(1)) {
				
				float a = detailMap[x,y];
				Color color = new Color(a, a, a, 1f);
				detailTex.SetPixel(x, y, color);
				y++;
				i++;
				
				if (i % spacing == 0) 
					yield return null;
			}
			GUIManager.instance.UpdateDetailSavePercentage(detailMap.GetLength(1));
			x++;
		}
		detailTex.Apply ();
		saveTexture (detailTex, "DetailMap" + map.ToString());

		if (map < terrainData.detailPrototypes.Length - 1)
			SetupMap (map + 1);
		Debug.Log ("Terrain detail map successfully extracted: " + path + "DetailMap" + map.ToString());
	}
	
	
	void saveTexture (Texture2D texture, string name) {
		
		byte[] bytes = texture.EncodeToPNG ();
		File.WriteAllBytes (path + name + ".png", bytes);
	}
	
}
