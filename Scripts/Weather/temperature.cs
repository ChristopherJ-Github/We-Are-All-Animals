using UnityEngine;
using System.Collections;
using System;
//script to apply temperature over time based on a curve
//Quentin Preik - 2012/10/22
//@fivearchers
public class temperature : Singleton<temperature>
{
	public AnimationCurve tempCurve; //Curve to use - use 1 second long, usually 1 unity high               
	public static float tempPercentage;
	float curvePos; //Use this as an offset - so 0.5 would start halfway thru the curv
	Vector3 localstart;                        
	
	void Update () { //can turn to minUpdate

		tempPercentage = tempCurve.Evaluate(SceneManager.curvePos);
		tempPercentage = Mathf.Clamp(tempPercentage,0,1);

	}
}