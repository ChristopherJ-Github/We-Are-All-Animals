using UnityEngine;
using System.Collections;

public class SnowReactions : MonoBehaviour {
	
	public float startOffset;
	private float minSnowLevel;
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
			minSnowLevel = -10;
		}
		if (type == objectType.Model) {
			objectMat = renderer.material;
			snowTint = SnowManager.instance.objectSnowTint;
			minSnowLevel = -7;
			maxSnowLevel = 1;
		}
	}
	
	void updateSnow (float snowLevel) {
		
		snowLevel = Mathf.Lerp (startOffset, maxSnowLevel, snowLevel);
		//snowLevel = Mathf.Clamp (startOffset + snowLevel, 0, maxSnowLevel);
		float newSnowLevel = Mathf.Lerp (minSnowLevel, maxSnowLevel, snowLevel);
		objectMat.SetFloat ("_Snow", newSnowLevel);
		Color newSnowTint = snowTint.Evaluate (snowLevel);
		objectMat.SetColor ("_SnowColor", newSnowTint);
	}
	
}
