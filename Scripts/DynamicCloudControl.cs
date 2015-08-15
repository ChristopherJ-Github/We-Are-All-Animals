using UnityEngine;
using System.Collections;

public class DynamicCloudControl : Singleton<DynamicCloudControl> {

	void OnEnable () {

		StartCoroutine(ChangeExtraOvercast());
	}

	public float minDelay, maxDelay;
	public float minExtraOvercast, maxExtraOvercast;
	public float snowInfluence;
	IEnumerator ChangeExtraOvercast () {
		
		float timePassed = 0;
		float currentDelay = Mathf.Lerp (maxDelay, minDelay, WindControl.instance.windiness);
		while (timePassed <= currentDelay) {	
			currentDelay = Mathf.Lerp (maxDelay, minDelay, WindControl.instance.windiness);
			timePassed += Time.deltaTime;
			yield return null;
		}
		float extraOvercastRange = Mathf.Lerp (maxExtraOvercast, minExtraOvercast, CloudControl.instance.overcast);
		float snowEffect = 1 - SnowManager.instance.snowLevel * snowInfluence;
		float extraOvercastGoal = Random.Range (-extraOvercastRange, extraOvercastRange) * snowEffect;
		StartCoroutine (SetExtraOvercast (extraOvercastGoal));
	}
	
	[HideInInspector] public float extraOvercast;
	public float changeSpeed;
	IEnumerator SetExtraOvercast (float extraOvercastGoal) {
		
		while (extraOvercast != extraOvercastGoal) {
			extraOvercast = Mathf.MoveTowards(extraOvercast, extraOvercastGoal, Time.deltaTime * changeSpeed);
			yield return null;
		}
		StartCoroutine(ChangeExtraOvercast());
	}
}
