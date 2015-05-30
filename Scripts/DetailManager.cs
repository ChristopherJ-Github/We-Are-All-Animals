using UnityEngine;
using System.Collections;

public class DetailManager : MonoBehaviour {

	TerrainData terrainData;

	void Start () {

		terrainData = Terrain.activeTerrain.terrainData;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
