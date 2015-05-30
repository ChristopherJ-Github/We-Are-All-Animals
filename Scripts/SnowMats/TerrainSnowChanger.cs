using UnityEngine;
using System.Collections;

public class TerrainSnowChanger : MonoBehaviour { 
	float snowLevel;
	float amount;
	// Use this for initialization
	void Start () {	
		amount = 0.01F;
	}
	
	// Update is called once per frame
	void Update () {
		if (snowLevel > 0.5 || snowLevel < 0.0) {
			amount *= -1;
			if (snowLevel < 0.0)
				Terrain.activeTerrain.materialTemplate.SetVector 
					("_SnowDirection", new Vector4(Random.Range(0.0F,1.0F),
					 								Random.Range(0.0F,1.0F),
					 								Random.Range(0.0F,1.0F),
					 								1.0F));
		}
			

		snowLevel +=amount;
		//renderer.material.SetFloat("_Snow", 1.0F);
		Terrain.activeTerrain.materialTemplate.SetFloat ("_Snow", snowLevel);
	}
}
