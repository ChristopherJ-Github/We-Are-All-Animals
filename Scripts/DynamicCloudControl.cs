using UnityEngine;
using System.Collections;

public class DynamicCloudControl : Singleton<DynamicCloudControl> {

	void OnEnable () {

		StartCoroutine(ChangeExtraOvercast());
	}

	public float minDelay, maxDelay;
	public float minExtraOvercast, maxExtraOvercast;
	public float snowInfluence;
	[HideInInspector] public float timePassed;
	[HideInInspector] public float currentDelay;
	IEnumerator ChangeExtraOvercast () {
		
		timePassed = 0;
		currentDelay = Mathf.Lerp (maxDelay, minDelay, WindControl.instance.windiness);
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
	[HideInInspector] public float extraOvercastGoal;
	IEnumerator SetExtraOvercast (float extraOvercastGoal) {

		this.extraOvercastGoal = extraOvercastGoal;
		while (extraOvercast != this.extraOvercastGoal) {
			extraOvercast = Mathf.MoveTowards(extraOvercast, this.extraOvercastGoal, Time.deltaTime * changeSpeed);
			yield return null;
		}
		StartCoroutine(ChangeExtraOvercast());
	}
}
