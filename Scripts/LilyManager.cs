using UnityEngine;
using System.Collections;

[System.Serializable]
public class WaterLilyProperties {

	public float originalHeight;
	public float speed, height;
	public AnimationCurve hoverAnimation;
	public float maxTilt;
	public float scale;
}

[System.Serializable]
public class LilyPadProperties {

	public float originalHeight;
	public float speed, height;
	public AnimationCurve hoverAnimation;
	public float minScale, maxScale;
	public Color tint1, tint2;
}

public class LilyManager : Singleton<LilyManager> {

	public WaterLilyProperties waterLilyProperties;
	public LilyPadProperties lilyPadProperties;
	public Material[] materials;
	public Material GetRandomMaterial () {

		int materialIndex = Random.Range (0, materials.Length);
		return materials[materialIndex];
	}
}
