using UnityEngine;
using System.Collections;
using System;

public class ModeledTreeManager : MonoBehaviour {

	public int barkMatNumber = 1;
	public int leafMatNumber = 0;
	public GameObject leafParticals;
	public AnimationCurve leafFallPercentage;
	public textureInfo[] bark;
	public textureInfo[] leaves;

	// Use this for initialization
	public void init () {

		SceneManager.instance.OnNewMonth += monthUpdate;
		ObjectChanger.sortArray (bark);
		ObjectChanger.sortArray (leaves);
		monthUpdate ();

	}

	void monthUpdate () {

		ObjectChanger.setTextureForMonth (leaves, leafMatNumber, renderer);
		ObjectChanger.setTextureForMonth (bark, barkMatNumber, renderer);
	}

}
