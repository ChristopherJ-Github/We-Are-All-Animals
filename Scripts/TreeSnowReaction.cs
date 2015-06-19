using UnityEngine;
using System.Collections;

public class TreeSnowReaction : MonoBehaviour {
	
	public float snowShininess = 0.15f;
	public float SnowStartHeight = 100.0f;
	public Texture2D SnowTexture;
	public float SnowTile = 10f;
	
	void Start () {								
		
		SnowManager.instance.OnSnowChange += updateSnow;
		
		Shader.SetGlobalFloat("_snowShininess", snowShininess);
		Shader.SetGlobalFloat("_SnowTile", SnowTile);
		if (SnowTexture)
			Shader.SetGlobalTexture("_SnowTexture", SnowTexture);
	}
	
	void updateSnow (float snowLevel) {
		
		Shader.SetGlobalFloat("_SnowAmount", snowLevel);
		Shader.SetGlobalFloat("_SnowStartHeight", SnowStartHeight);
	}
	
}
