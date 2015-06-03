using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MapInfo {

	public Texture2D alphaMap;
	public bool black = true;
}

[System.Serializable]
public class GrassInfo {
	public int month;
	public int day = 1;
	public List<MapInfo> alphamaps;
}

public class GrassManager : Singleton<GrassManager> {
	
	private TerrainData terrainData;
	public List<GrassInfo> alphaMapsOverYear;
	private float[,,] outputBlock;
	public int blockWidth;

	void Start () {
		
		SceneManager.instance.OnNewDay += SetAlphaMaps; 
		terrainData = Terrain.activeTerrain.terrainData;
		alphaMapsOverYear = alphaMapsOverYear.OrderBy (grassInfo => grassInfo.month).ToList ();
		outputBlock = new float[blockWidth, blockWidth, terrainData.alphamapLayers];
		totalLength = blockWidth * blockWidth * terrainData.alphamapLayers;
		//setAlphaMaps ();
	}

	public void SetAlphaMaps () {

		if (GUIManager.instance.sliderUsed || SpeedThroughDay.instance.on) //avoid updating slowly every time the slider moves
			return;

		GrassInfo prevSet, nextSet;
		SetSets (out prevSet, out nextSet);
		float blend = GetBlend (prevSet, nextSet);
		StopAllCoroutines ();
		StartCoroutine (FinalizeAlphaMaps (prevSet, nextSet, blend));
	}
	
	void SetSets (out GrassInfo _prevSet, out GrassInfo _nextSet) {
		
		int prevIndex = -1;
		for (int i = alphaMapsOverYear.Count - 1; i >= 0; i--) {
			if (SceneManager.currentDate.Month >= alphaMapsOverYear[i].month) {
				
				prevIndex = i;
				break;
			}
		}
		if (prevIndex == -1)
			prevIndex = alphaMapsOverYear.Count -1;
		
		_prevSet = alphaMapsOverYear [prevIndex];
		_nextSet = alphaMapsOverYear [(prevIndex + 1) % alphaMapsOverYear.Count];
	}
	
	float GetBlend (GrassInfo _prevSet, GrassInfo _nextSet) { //days aren't taken into account like 12/4 vs 12/3
		
		int prevSetYear = SceneManager.currentDate.Year;
		DateTime prevSetDate = new DateTime (prevSetYear, _prevSet.month, _prevSet.day);
		int nextSetYear = _prevSet.month < _nextSet.month ? prevSetYear : prevSetYear + 1;
		DateTime nextSetDate = new DateTime (nextSetYear, _nextSet.month, _nextSet.day);
		
		double totalMinutes = (nextSetDate - prevSetDate).TotalMinutes;
		double currentMinutes = (SceneManager.currentDate - prevSetDate).TotalMinutes;
		float blend = (float)(currentMinutes / totalMinutes);
		return blend;
	}

	public Vector2 cropOrigin;
	public int cropWidth, cropHeight;
	private int totalLength;
	[HideInInspector] public float progress;
	IEnumerator FinalizeAlphaMaps (GrassInfo _prevSet, GrassInfo _nextSet, float blend) {

		float total = GetTotal ();
		float currentTotal = 0;
		for (int originY = 0; originY <= cropHeight - blockWidth; originY += blockWidth) {

			float currentCropWidth = cropWidth + originY;
			for (int originX = 0; originX <= currentCropWidth - blockWidth; originX += blockWidth) {

				int offsetOriginX = originX + (int)(cropOrigin.x - originY/2);
				int offsetOriginY = originY + (int)cropOrigin.y;
				for (int layer = 0; layer < terrainData.alphamapLayers; layer++) {

					currentTotal += blockWidth * blockWidth;
					progress = currentTotal/total * 100;
					if (_prevSet.alphamaps[layer].black && _nextSet.alphamaps[layer].black) 
						continue;

					Texture2D inputText1 = new Texture2D(blockWidth, blockWidth);
					Texture2D inputText2 = new Texture2D(blockWidth, blockWidth);
					Color[] inputText1Pixels = _prevSet.alphamaps[layer].alphaMap.GetPixels (offsetOriginY , offsetOriginX, blockWidth, blockWidth);
					Color[] inputText2Pixels = _nextSet.alphamaps[layer].alphaMap.GetPixels (offsetOriginY , offsetOriginX, blockWidth, blockWidth);
					inputText1.SetPixels(inputText1Pixels);
					inputText2.SetPixels(inputText2Pixels);
					inputText1.Apply();
					inputText2.Apply();
					GrassTextureCombiner.instance.inputTex1 = inputText1;
					GrassTextureCombiner.instance.inputTex2 = inputText2;
					GrassTextureCombiner.instance.blend = blend;
					GrassTextureCombiner.instance.activate = true;
					yield return new WaitForEndOfFrame();

					Texture2D outputTexture2D = GrassTextureCombiner.instance.outputTexture2D;
					Color[] outputPixels = outputTexture2D.GetPixels ();
					int index = 0;
					for (int y = 0; y < blockWidth; y ++) {
						for (int x = 0; x < blockWidth; x ++) {
							outputBlock[x, y, layer] = outputPixels[index].r; //it's grayscale so any one value will do
							index ++;
						}
					}
					Destroy(inputText1);
					Destroy(inputText2);
					Destroy(GrassTextureCombiner.instance.outputTexture2D);
				}
				terrainData.SetAlphamaps (offsetOriginX, offsetOriginY, outputBlock);
				Array.Clear (outputBlock, 0, totalLength);
			}
		}
	}

	float GetTotal () {

		float total = 0;
		for (int originY = 0; originY <= cropHeight - blockWidth; originY += blockWidth) {
			
			float currentCropWidth = cropWidth + originY;
			for (int originX = 0; originX <= currentCropWidth - blockWidth; originX += blockWidth) {
				
				int offsetOriginX = originX + (int)(cropOrigin.x - originY/2);
				int offsetOriginY = originY + (int)cropOrigin.y;
				for (int layer = 0; layer < terrainData.alphamapLayers; layer++) {
					total += blockWidth * blockWidth;
				}
			}
		}
		return total;
	}
}
