using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class NoiseCreator : MonoBehaviour {/*

	public bool createOnStart;
	int width, height;
	string path;

	void Start () {

		path = Application.dataPath + "/Terrain Detail Maps/Noise/";
		width = Terrain.activeTerrain.terrainData.detailWidth;
		height = Terrain.activeTerrain.terrainData.detailHeight;

		if (createOnStart) {
			Texture2D noise = createNoise();
			saveTexture (noise, "noise");
		}
	}
	
	Texture2D createNoise() {
		
		Texture2D alphaMap = new Texture2D (width, height);
		
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				
				float a = Random.value;
				Color color = new Color(a, a, a, 1f);
				alphaMap.SetPixel(x, y, color);
			}
		}
		alphaMap.Apply ();
		return alphaMap;
	}


	void saveTexture (Texture2D texture, string name) {
		
		byte[] bytes = texture.EncodeToPNG ();
		File.WriteAllBytes (path + name + ".png", bytes);
	}
	*/
}
