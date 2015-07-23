using UnityEngine;
using System.Collections;
using System;

public class TemperatureManager : Singleton<TemperatureManager> {

	public AnimationCurve tempCurve;         
	public static float temperature;                      
	
	void Update () {

		UpdateTemperature ();
	}

	void UpdateTemperature () {

		temperature = tempCurve.Evaluate(SceneManager.curvePos);
	}
}