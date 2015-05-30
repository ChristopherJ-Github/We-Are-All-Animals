using UnityEngine;
using System.Collections;

public class SnowChanger : MonoBehaviour { 
	float snowLevel;
	public float rate = 0.01F;
	// Use this for initialization
	void Start () {	

	}
	
	// Update is called once per frame
	void Update () {
		if (snowLevel > 0.8 || snowLevel < 0.0) {
			rate *= -1;
			if (snowLevel < 0.0)
				renderer.material.SetVector 
					("_SnowDirection", new Vector4(Random.Range(0.0F,1.0F),
					 								Random.Range(0.0F,1.0F),
					 								Random.Range(0.0F,1.0F),
					 								1.0F));
		}
			

		snowLevel +=rate;
		//renderer.material.SetFloat("_Snow", 1.0F);
		renderer.material.SetFloat ("_Snow", snowLevel);
	}
}
