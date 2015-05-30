using UnityEngine;
using System.Collections;

public class LiliBirthday : MonoBehaviour {

	public Texture2D detailMap;
	Texture2D originalDetailMap;
	public int index;

	void OnEnable () {

		originalDetailMap = FlowerManager.instance.flowerAlphaMaps [index].detailMap;
		FlowerManager.instance.flowerAlphaMaps [index].detailMap = detailMap;
		FlowerManager.instance.setDetailMaps ();
	}
	
	void OnDisable () {

		FlowerManager.instance.flowerAlphaMaps [index].detailMap = originalDetailMap;
		FlowerManager.instance.setDetailMaps ();
	}
}
