using UnityEngine;
using System;
using System.Linq;
using System.Collections;


public class ModeledTreeManager : MonoBehaviour {

	public int barkMatNumber = 1;
	public int leafMatNumber = 0;
	public GameObject leafParticals;
	public AnimationCurve leafFallPercentage;
	public TextureInfo[] bark;
	public TextureInfo[] leaves;
	
	public void Init () {

		SceneManager.instance.OnNewDay += SetCurrentTextures;
		bark = bark.OrderBy (_textureInfo => _textureInfo.monthStart).ToArray();
		leaves = leaves.OrderBy (_textureInfo => _textureInfo.monthStart).ThenBy (_textureInfo => _textureInfo.day).ToArray ();
		SetCurrentTextures ();

	}

	void SetCurrentTextures () {

		ObjectChanger.SetCurrentTexture(leaves, leafMatNumber, renderer);
		ObjectChanger.SetCurrentTexture (bark, barkMatNumber, renderer);
	}

}
