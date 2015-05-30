using UnityEngine;
using System.Collections;
using System;

public class ObjectChanger : MonoBehaviour {

	public static void setTextureForMonth (textureInfo[] info, int matNumber, Renderer rendr) {

		int indx = ObjectChanger.monthIndex (info);
		Texture currentTexture = info[indx].texture; //if the bottom loop doesn't apply set to the remaining months
		rendr.materials[matNumber].SetTexture("_MainTex",currentTexture);
		
	}

	public static GameObject setPrefabForMonth (prefabInfo[] info, GameObject currentPrefab, Transform tranfrm) {
		
		GameObject.Destroy (currentPrefab);
		int indx = ObjectChanger.monthIndex (info);
		currentPrefab = info[indx].prefab; 
		
		currentPrefab = GameObject.Instantiate (currentPrefab, tranfrm.position, tranfrm.rotation) as GameObject;
		currentPrefab.transform.localScale = tranfrm.localScale;
		
		return currentPrefab;
	}

	public static int monthIndex (monthInfo[] info) {

		for (int i = info.Length-1 ; i >= 0 ; i--) {
			if (SceneManager.currentDate.Month >= info[i].monthStart) {
				return i;
			}
		}

		return info.Length - 1;
	}

	public static void sortArray(monthInfo[] array) { 
		
		monthInfo[] arrayCopy = (monthInfo[])array.Clone ();
		int[] sortedArray = new int[array.Length];
		int[] index = new int[array.Length];
		
		
		for (int i = 0; i < array.Length; i++) 
			sortedArray[i] = array[i].monthStart;
		Array.Sort(sortedArray);
		
		for (int i = 0; i < array.Length; i++) {
			index[i] = Array.IndexOf(sortedArray, array[i].monthStart);
		}
		
		for (int i = 0; i < array.Length; i++) 
			array[index[i]] = arrayCopy[i];
		
	}
}
