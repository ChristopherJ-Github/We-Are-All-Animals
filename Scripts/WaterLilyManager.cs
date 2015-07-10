using UnityEngine;
using System.Collections;

public class WaterLilyManager : Singleton<WaterLilyManager> {

	public Material[] materials;
	public Material GetRandomMaterial () {

		int materialIndex = Random.Range (0, materials.Length);
		return materials[materialIndex];
	}
}
