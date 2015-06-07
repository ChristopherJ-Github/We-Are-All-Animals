using UnityEngine;
using System.Collections;

public class StopNodeProperties : MonoBehaviour {

	public float landDuration;
	public float takeOffDuration;
	public float duration; 
	public AnimationClip idleAnimation;
	public AnimationClip randomAnimation;
	public AnimationClip landAnimation;
	public AnimationClip takeOffAnimation;
	public AnimationClip afterMovementAnimation;
	public float afterMovementSpeed;
	public bool showBirdFields;
}
