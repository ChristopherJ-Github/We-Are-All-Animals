using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[System.Serializable]
public class GrassInfo {
	public int month;
	public int day = 1;
	public List<Texture2D> alphamaps;
}

public unsafe class GrassManager : Singleton<GrassManager> {
	
	private TerrainData terrainData;
	public List<GrassInfo> alphaMapsOverYear;
	
	void Start () {
		
		SceneManager.instance.OnNewDay += SetAlphaMaps; 
		terrainData = Terrain.activeTerrain.terrainData;
		alphaMapsOverYear = alphaMapsOverYear.OrderBy (grassInfo => grassInfo.month).ToList ();
		AllocArray ();
		//setAlphaMaps ();
	}

	private int elemSize;
	private IntPtr finalResultPointer;
	void AllocArray () {

		float[,,] finalResult = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
		finalResult [0, 0, 0] = 1;
		finalResult [0, 0, 1] = 2;
		finalResult [0, 0, 2] = 3;

		int totalLength = terrainData.alphamapWidth * terrainData.alphamapHeight * terrainData.alphamapLayers;
		elemSize = sizeof(float);
		int size = elemSize* totalLength;
		finalResultPointer = Marshal.AllocHGlobal (size);

		int i = 0;
		for (int width = 0; width < terrainData.alphamapWidth; width++) {
			for (int height = 0; height < terrainData.alphamapHeight; height++) {
				for (int layer = 0; layer < terrainData.alphamapLayers; layer++) {
					IntPtr pointer = new IntPtr (finalResultPointer.ToInt32() + elemSize * i);
					Marshal.StructureToPtr(finalResult[width, height, layer], pointer, false);
					if (i < 10) {
						float value = (float)Marshal.PtrToStructure(pointer, typeof(float));
						Debug.Log(i + ": pointer: " + pointer.ToInt32() + ", value: " + value);
					}
					i++;
				}
			}
		}
	}

	private bool pressed;
	IEnumerator FreeMemory () {

		int i = 0;
		for (int width = 0; width < terrainData.alphamapWidth; width++) {
			for (int height = 0; height < terrainData.alphamapHeight; height++) {
				for (int layer = 0; layer < terrainData.alphamapLayers; layer++) {
					IntPtr pointer = new IntPtr (finalResultPointer.ToInt32() + elemSize * i); 
					if (i < 10) {
						float value = (float)Marshal.PtrToStructure(pointer, typeof(float));
						Debug.Log(i + ": pointer: " + pointer.ToInt32() + ", value: " + value);
					}
					//Marshal.FreeHGlobal(pointer);
					yield return null;
					i++;
				}
			}
		}

		/*
		Marshal.FreeHGlobal(finalResultPointer);
		Debug.Log ("finalResult 1: " + finalResult[0,0,0]);
		Debug.Log ("finalResult 2: " + finalResult[0,0,1]);
		Debug.Log ("finalResult 3: " + finalResult[0,0,2]);
		*/
		finalResultPointer = IntPtr.Zero;
		Debug.Log("freed aparently");
		yield return null;
	}

	void Update () {

		if (Input.GetKey(KeyCode.H) && !pressed) {
			pressed = true;
			StartCoroutine(FreeMemory());
		}
	}

	public void SetAlphaMaps () {
		
		if (GUIManager.instance.sliderUsed || SpeedThroughDay.instance.on) //avoid updating slowly every time the slider moves
			return;

		GrassInfo prevSet, nextSet;
		SetSets (out prevSet, out nextSet);
		float blend = GetBlend (prevSet, nextSet);
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
	
	IEnumerator FinalizeAlphaMaps (GrassInfo _prevSet, GrassInfo _nextSet, float blend) {
		/*
		Color[] outputPixels;
		//float[] outputValues;
		
		for (int i = 0; i < terrainData.alphamapLayers; i++) {
			
			GrassTextureCombiner.instance.inputTex1 = _prevSet.alphamaps[i];
			GrassTextureCombiner.instance.inputTex2 = _nextSet.alphamaps[i];
			GrassTextureCombiner.instance.blend = blend;
			GrassTextureCombiner.instance.activate = true;
			yield return new WaitForEndOfFrame();
			
			Texture2D outputTexture2D = GrassTextureCombiner.instance.outputTexture2D;
			outputPixels = outputTexture2D.GetPixels();
			float[] outputValues = outputPixels.Select(pixel => pixel.r).ToArray(); //it's grayscale so any one value will do
			int index = 0;
			for (int y = 0; y < finalResult.GetLength(1); y ++) {
				for (int x = 0; x < finalResult.GetLength(0); x ++) {
					finalResult[x, y, i] = outputValues[index];
					index ++;
				}
			}
			Destroy(GrassTextureCombiner.instance.outputTexture2D);
			outputPixels = null;
			outputValues = null;
			yield return null;
		}
		terrainData.SetAlphamaps (0, 0, finalResult);
		System.GC.Collect();
		*/
		yield return null;
	}

	void OnDestroy () {

		Marshal.FreeHGlobal (finalResultPointer);
	}
}
