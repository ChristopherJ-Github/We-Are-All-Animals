using UnityEngine;
using System.Collections;

public class GeneralWeather : MonoBehaviour {
	
	[HideInInspector] public float severity;
	[HideInInspector] public float maxSeverity { 
		get{ return severityOverYear.Evaluate (SceneManager.curvePos); } 
	}
	public AnimationCurve severityOverYear;
}
