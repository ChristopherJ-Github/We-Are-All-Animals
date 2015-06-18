using UnityEngine;
using System.Collections;

public class SnowReactions : MonoBehaviour {
	
	public float minSnowLevel;
	public float maxSnowLevel;
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
			minSnowLevel = -1;
			maxSnowLevel = 0;
		}
	}
	
	void updateSnow (float snowLevel) {

		snowLevel = Mathf.Lerp (minSnowLevel, maxSnowLevel, snowLevel);
		objectMat.SetFloat ("_Snow", snowLevel);
		Color newSnowTint = snowTint.Evaluate (snowLevel);
		objectMat.SetColor ("_SnowColor", newSnowTint);
	}
	
}
