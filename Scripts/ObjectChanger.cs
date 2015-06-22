using UnityEngine;
using System.Collections;
using System;

public class ObjectChanger : MonoBehaviour {

	public static void SetCurrentTexture (textureInfo[] info, int matNumber, Renderer rendr) {

		int indx = ObjectChanger.GetIndex (info);
		Texture currentTexture = info[indx].texture; //if the bottom loop doesn't apply set to the remaining months
		rendr.materials[matNumber].SetTexture("_MainTex",currentTexture);
		
	}

	public static GameObject SetCurrentPrefab (prefabInfo[] info, GameObject currentPrefab, Transform tranfrm) {
		
		GameObject.Destroy (currentPrefab);
		int indx = ObjectChanger.GetIndex (info);
		currentPrefab = info[indx].prefab; 
		currentPrefab = GameObject.Instantiate (currentPrefab, tranfrm.position, tranfrm.rotation) as GameObject;
		currentPrefab.transform.localScale = tranfrm.localScale;
		return currentPrefab;
	}

	public static int GetIndex (monthInfo[] info) {
		
		for (int i = info.Length-1 ; i >= 0 ; i--) {
			if (SceneManager.currentDate.Month == info[i].monthStart) {
				if (SceneManager.currentDate.Day >= info[i].day)
					return i;
			}
			else if (SceneManager.currentDate.Month > info[i].monthStart) 
				return i;
		}
		return info.Length - 1;
	}
}
