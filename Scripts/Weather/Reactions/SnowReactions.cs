using UnityEngine;
using System.Collections;

public class SnowReactions : MonoBehaviour {
	
	public float minSnowLevel = -1;
	public float maxSnowLevel = 0;
	private Gradient snowTint;
	public enum objectType {Model, Terrain};
	public objectType type;
	Material objectMat;
	
	void Start () {								
		
		SnowManager.instance.OnSnowChange += updateSnow;
		if (type == objectType.Terrain) {
			objectMat = Terrain.activeTerrain.materialTemplate;
			snowTint = SnowManager.instance.terrainSnowTint;
		}
		if (type == objectType.Model) {
			objectMat = renderer.material;
			snowTint = SnowManager.instance.objectSnowTint;
		}
	}
	
	void updateSnow (float snowLevel) {

		float newNewLevel = Mathf.Lerp (minSnowLevel, maxSnowLevel, snowLevel);
		objectMat.SetFloat ("_Snow", newNewLevel);
		Color newSnowTint = snowTint.Evaluate (newNewLevel);
		objectMat.SetColor ("_SnowColor", newSnowTint);
	}
	
}
