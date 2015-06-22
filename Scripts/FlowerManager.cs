using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FlowerInfo {
	public Texture2D detailMap;
	public bool useMainCurve = true;
	public AnimationCurve growthOverYear;
}

public class FlowerManager : Singleton<FlowerManager> {
	
	TerrainData terrainData;
	public Texture2D noise;
	public AnimationCurve mainGrowthOverYear;
	public List<FlowerInfo> flowerAlphaMaps;
	private Texture2D detailMap;
	private int[,] outputBlock;
	public int blockWidth;

	void Start () {
		
		SceneManager.instance.OnNewDay += SetDetailMaps; 
		terrainData = Terrain.activeTerrain.terrainData;
		if (flowerAlphaMaps.Count != terrainData.detailPrototypes.Length) { 
			Debug.LogError("not enough or too many detail maps");
			return;
		}
		outputBlock = new int[blockWidth, blockWidth];
		//setDetailMaps ();  
	}
	
	public void SetDetailMaps () {

		if (GUIManager.instance.sliderUsed || SpeedThroughDay.instance.on) //avoid updating slowly every time the slider moves
			return;

		DetailMapRoutine ();
	}

	public Vector2 cropOrigin;
	public int cropWidth, cropHeight;
	public Texture2D safeZone;
	void DetailMapRoutine () {

		for (int originY = 0; originY <= cropHeight - blockWidth; originY += blockWidth) {

			float currentCropWidth = cropWidth + originY;
			for (int originX = 0; originX <= currentCropWidth - blockWidth; originX += blockWidth) {

				int offsetOriginX = originX + (int)(cropOrigin.x - originY/2);
				int offsetOriginY = originY + (int)cropOrigin.y;
				for (int map = 0; map < terrainData.detailPrototypes.Length; map ++) {

					detailMap = flowerAlphaMaps[map].detailMap;
					AnimationCurve growthOverYear = flowerAlphaMaps[map].useMainCurve ? mainGrowthOverYear : flowerAlphaMaps[map].growthOverYear;
					float growth = growthOverYear.Evaluate (SceneManager.curvePos);
					Color[] detailMapPixels = detailMap.GetPixels(offsetOriginY, offsetOriginX, blockWidth, blockWidth);
					Color[] safeZonePixels = safeZone.GetPixels(offsetOriginY, offsetOriginX, blockWidth, blockWidth);

					int index = 0;
					for (int y = 0; y < blockWidth; y++) {
						for (int x = 0; x < blockWidth; x++) {
							float a = detailMapPixels[index].r; //it's grayscale so any one value will do
							float noiseA = noise.GetPixel(x,y).r;
							a = noiseA + growth >= 1f && a == 1 ? 1f : 0f;
							int safeZoneProtection = (int)(safeZonePixels[index].r * detailMapPixels[index].r);
							outputBlock[x, y] = safeZoneProtection != 1 ? (int)a : 1;
							index ++;
						}
					} 
					terrainData.SetDetailLayer(offsetOriginX, offsetOriginY, map, outputBlock);
				}
			}
		}
	}
}
