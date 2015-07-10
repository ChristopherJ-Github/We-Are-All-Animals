using UnityEngine;
using System.Collections;

public class StopNodeProperties : MonoBehaviour {
	
	public AnimationClip idleAnimation;
	public float duration; 
	public AnimationClip randomAnimation;
	public float minInterval, maxInterval;
	public AnimationClip landAnimation;
	public float landDuration;
	public AnimationClip takeOffAnimation;
	public float takeOffDuration;
	public AnimationClip afterMovementAnimation;
	public float afterMovementSpeed;
	public bool showBirdFields;
	public bool showRandomAnimFields;
	public Vector3 rotation;
}
