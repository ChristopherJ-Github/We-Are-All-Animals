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
	public int spacing;
	private Texture2D detailMap;
	private int[,] outputMap;
	
	void Start () {
		
		SceneManager.instance.OnNewDay += setDetailMaps; 
		terrainData = Terrain.activeTerrain.terrainData;
		if (flowerAlphaMaps.Count != terrainData.detailPrototypes.Length) { 
			Debug.LogError("not enough or too many detail maps");
			return;
		}
		outputMap = new int[terrainData.detailWidth, terrainData.detailHeight];
		//setDetailMaps ();  
	}
	
	public void setDetailMaps () {
		
		
		if (GUIManager.instance.sliderUsed || SpeedThroughDay.instance.on) //avoid updating slowly every time the slider moves
			return;
		
		StopAllCoroutines ();
		setDetailMap (0);
	}
	
	void setDetailMap (int map) {
		
		detailMap = flowerAlphaMaps[map].detailMap;
		AnimationCurve growthOverYear = flowerAlphaMaps[map].useMainCurve ? mainGrowthOverYear : flowerAlphaMaps[map].growthOverYear;
		float growth = getCurrentGrowth(growthOverYear);
		StartCoroutine(applyDetail (outputMap, detailMap, growth, map));
		detailMap = null;
		Destroy(detailMap);
	}
	
	float getCurrentGrowth(AnimationCurve growthOverYear) {
		
		float growth = growthOverYear.Evaluate (SceneManager.curvePos);
		return growth;
	}
	
	IEnumerator applyDetail (int[,] _outputMap, Texture2D detail, float lerp, int map) { 
		
		int x = 0;
		int i = 0;
		float total = detail.width * detail.height;
		while(x < detail.width) {
			
			int y = 0;
			while(y < detail.height) {
				
				float a = detail.GetPixel(x,y).r; //r g or b works
				float noiseA = noise.GetPixel(x,y).r;
				a = noiseA + lerp >= 1f && a == 1 ? 1f : 0f;
				outputMap[x, y] = (int)a;
				y ++;
				i ++;
				GUIManager.instance.growthPercentage = i/total * 100;
				if (i % spacing == 0)
					yield return null;
			} 
			x++;
		}
		terrainData.SetDetailLayer(0, 0, map, outputMap);
		_outputMap = null;
		detail = null;
		
		if (map < terrainData.detailPrototypes.Length -1)
			setDetailMap(map + 1);
		else
			System.GC.Collect ();
	}
}
