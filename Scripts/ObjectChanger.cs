using UnityEngine;
using System.Collections;
using System;

public class ObjectChanger : MonoBehaviour {

	public static void SetCurrentTexture (TextureInfo[] info, int matNumber, Renderer rendr) {

		int indx = ObjectChanger.GetIndex (info);
		Texture currentTexture = info[indx].texture; //if the bottom loop doesn't apply set to the remaining months
		rendr.materials[matNumber].SetTexture("_MainTex",currentTexture);
		
	}

	public static GameObject SetCurrentPrefab (PrefabInfo[] info, GameObject currentPrefab, Transform tranfrm, out int monthStart) {
		
		GameObject.Destroy (currentPrefab);
		int indx = ObjectChanger.GetIndex (info);
		PrefabInfo prefabInfo = info [indx];
		currentPrefab = prefabInfo.prefab; 
		currentPrefab = GameObject.Instantiate (currentPrefab, tranfrm.position, tranfrm.rotation) as GameObject;
		currentPrefab.transform.localScale = tranfrm.localScale;
		monthStart = prefabInfo.monthStart;
		return currentPrefab;
	}

	public static int GetIndex (MonthInfo[] info) {
		
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
