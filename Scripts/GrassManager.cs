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
public class Date {
	
	public int month;
	public int day = 1;
}

[System.Serializable]
public class GrassInfo {
	
	public Date[] dates;
	[HideInInspector] public Date date;
	public List<MapInfo> alphamaps;
}

public class GrassManager : Singleton<GrassManager> {
	
	private TerrainData terrainData;
	public List<GrassInfo> alphaMapsOverYear;
	private List<GrassInfo> _alphaMapsOverYear;
	private float[,,] outputBlock;
	public int blockWidth;
	
	void Start () {
		
		terrainData = Terrain.activeTerrain.terrainData;
		PopulateAlphaMapsOverYear ();
		_alphaMapsOverYear = _alphaMapsOverYear.OrderBy (grassInfo => grassInfo.date.month).
			ThenBy(grassInfo => grassInfo.date.day).ToList ();
		outputBlock = new float[blockWidth, blockWidth, terrainData.alphamapLayers];
		totalLength = blockWidth * blockWidth * terrainData.alphamapLayers;
		#if !UNITY_EDITOR
		SceneManager.instance.OnNewDay += SetAlphaMaps;
		SetAlphaMaps ();
		#endif
	}
	
	void PopulateAlphaMapsOverYear () {
		
		_alphaMapsOverYear = new List<GrassInfo> ();
		foreach (GrassInfo sourceGrassInfo in alphaMapsOverYear) {
			foreach (Date date in sourceGrassInfo.dates) {
				GrassInfo newGrassInfo = new GrassInfo();
				newGrassInfo.alphamaps = sourceGrassInfo.alphamaps;
				newGrassInfo.date = date;
				_alphaMapsOverYear.Add(newGrassInfo);
			}
		}
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
		
		int prevIndex = GetPreviousIndex ();
		_prevSet = _alphaMapsOverYear [prevIndex];
		_nextSet = _alphaMapsOverYear [(prevIndex + 1) % _alphaMapsOverYear.Count];
	}
	
	int GetPreviousIndex () {
		
		for (int i = _alphaMapsOverYear.Count - 1 ; i >= 0 ; i--) {
			GrassInfo grassInfo = _alphaMapsOverYear[i];
			if (SceneManager.currentDate.Month == _alphaMapsOverYear[i].date.month) {
				if (SceneManager.currentDate.Day >= _alphaMapsOverYear[i].date.day)
					return i;
			}
			else if (SceneManager.currentDate.Month > _alphaMapsOverYear[i].date.month) 
				return i;
		}
		return _alphaMapsOverYear.Count - 1;
	}
	
	float GetBlend (GrassInfo prevSet, GrassInfo nextSet) {
		
		int prevSetYear = GetPrevSetYear (prevSet);
		int nextSetYear = GetNextSetYear (nextSet);
		DateTime prevSetDate = new DateTime (prevSetYear, prevSet.date.month, prevSet.date.day);
		DateTime nextSetDate = new DateTime (nextSetYear, nextSet.date.month, nextSet.date.day);
		double totalMinutes = (nextSetDate - prevSetDate).TotalMinutes;
		double currentMinutes = (SceneManager.currentDate - prevSetDate).TotalMinutes;
		float blend = (float)(currentMinutes / totalMinutes);
		return blend;
	}
	
	bool AreDatesInOrder (int firstMonth, int firstDay, int secondMonth, int secondDay) {
		
		if (firstMonth < secondMonth) {
			return true;
		} else if (secondMonth < firstMonth) {
			return false;
		} else {
			if (firstDay <= secondDay)
				return true;
			else
				return false;
		}
	}
	
	int GetPrevSetYear (GrassInfo prevSet) {
		
		DateTime currentDate = SceneManager.currentDate;
		Date prevDate = prevSet.date;
		if (AreDatesInOrder(prevDate.month, prevDate.day, currentDate.Month, currentDate.Day))
			return currentDate.Year;
		else 
			return currentDate.Year - 1;
	}
	
	int GetNextSetYear (GrassInfo nextSet) {
		
		DateTime currentDate = SceneManager.currentDate;
		Date nextDate = nextSet.date;
		if (AreDatesInOrder(currentDate.Month, currentDate.Day, nextDate.month, nextDate.day))
			return currentDate.Year;
		else 
			return currentDate.Year + 1;
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
