using UnityEngine;
using System.Collections;

public class AnimationSpeedChanger : MonoBehaviour {

	public AnimationClip animationClip;
	public float speed = 1;

	void Awake () {

		animation [animationClip.name].speed = speed;
	}
}
