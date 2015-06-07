using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Animates a gameobject along the spline at a specific speed. 
/// </summary>
[AddComponentMenu("SuperSplines/Animation/Regular Animator")]
public class AnimalAnimator: MonoBehaviour{
	
	public AnimationCurve spawnChance;
	public bool bird;
	private delegate void splineStateHandler();
	private splineStateHandler splineState;
	
	private float speed;
	public float initSpeed = 1f;
	public GameObject animal;
	private Animation animation;
	public AnimationClip initAnimation;
	private Spline spline;
	void Start () {
		
		AnimationSpawner.instance.currentAnimations.Add(this);
		speed = initSpeed;
		animation = animal.GetComponent<Animation> ();
		animation.Play(initAnimation.name);
		List<Spline> splines = GetSplines ();
		splineIndex = GetSplineIndex(splines.Count);
		if (splineIndex != -1) {
			spline = splines[splineIndex];
			InitStopNodeArray();
			splineState = moving;
		} else {
			Remove();
		}
	}

	void Remove () {
		
		AnimationSpawner.instance.currentAnimations.Remove(this);
		Destroy (gameObject);
	}
	
	List<Spline> GetSplines () {
		
		List<Spline> splines = new List<Spline> ();
		foreach (Transform child in transform) {
			Spline _spline = child.GetComponent<Spline>();
			if (_spline != null) {
				splines.Add(_spline);
			}
		}
		return splines;
	}
	
	[HideInInspector] public int splineIndex;
	int GetSplineIndex (int splineCount) {
		
		if (splineIndex != null)
			return splineIndex;
		
		List<int> availableSplines = Enumerable.Range (0, splineCount).ToList ();
		foreach (AnimalAnimator animation in AnimationSpawner.instance.currentAnimations) {
			if (animal.name == animation.animal.name) 
				availableSplines.Remove(animation.splineIndex);
		}
		if (availableSplines.Count > 0) 
			return availableSplines[Random.Range(0, availableSplines.Count)];
		else 
			return -1;
	}

	private List<SplineNode> stopNodeArray; 
	private List<bool> nodePassed;
	void InitStopNodeArray () {
		
		stopNodeArray = new List<SplineNode>();
		nodePassed = new List<bool> ();
		foreach (SplineNode node in spline.SplineNodes) { 
			StopNodeProperties isAStopNode = node.GetComponent<StopNodeProperties>();
			if (isAStopNode) { 
				stopNodeArray.Add(node); 
				nodePassed.Add(false);
			}
		}
	}
	
	private double stopIn;
	void Update() {
		
		stopIn -= Time.deltaTime;
		splineState ();
	}
	
	private float passedTime = 0f;
	void moving () {
		
		passedTime += (Time.deltaTime * speed) / spline.Length;
		float nodePosition = GetCurrentNodePosition ();
		SetPosition (nodePosition);
		SetRotation (nodePosition);
		SplineEndCheck (nodePosition);
	}
	
	float GetCurrentNodePosition () {

		float stopNodePosition;
		if (Stop(out stopNodePosition))
			return stopNodePosition;
		else
			return WrapValue(passedTime, 0f, 1f, wrapMode);
	}

	public WrapMode wrapMode = WrapMode.Once;
	float WrapValue( float v, float start, float end, WrapMode wMode ) {
		
		switch( wMode ) {
		case WrapMode.Clamp:
		case WrapMode.ClampForever:
			return Mathf.Clamp(v, start, end);
		case WrapMode.Default:
		case WrapMode.Loop:
			return Mathf.Repeat(v, end - start) + start;
		case WrapMode.PingPong:
			return Mathf.PingPong(v, end - start) + start;
		default:
			return v;
		}
	}
	
	void SetPosition (float nodePosition) {
		
		animal.transform.position = spline.GetPositionOnSpline(nodePosition);
		if (!bird) {
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (animal.transform.position, -Vector3.up, out hit,1000.0f)) {
				float distanceToGround = hit.distance;
				Vector3 down = new Vector3(0,hit.distance,0);
				animal.transform.position -= down;		
			}
		}
	}
	
	void SetRotation (float nodePosition) {
		
		Quaternion newRot = spline.GetOrientationOnSpline(nodePosition);
		if (bird) { //reverse roll
			Quaternion reversedRoll = new Quaternion();
			reversedRoll.eulerAngles = new Vector3 (newRot.eulerAngles.x, newRot.eulerAngles.y, -newRot.eulerAngles.z);
			newRot = reversedRoll;	
		} else { //remove roll
			Quaternion onlyY = new Quaternion();
			onlyY.eulerAngles = new Vector3 (0,newRot.eulerAngles.y, 0);
			newRot = Quaternion.RotateTowards(newRot,onlyY,360f);
		}
		animal.transform.rotation = newRot; 
	}

	public int amountOfLoops;
	private int loopCount;
	private float lastParam;
	void SplineEndCheck (float nodePosition) {
		
		if (wrapMode == WrapMode.Loop) {
			if (nodePosition < lastParam) {
				loopCount++;
				ResetStopNodes();
			}
			if (loopCount >= amountOfLoops)
				Remove();
		} else {
			if (nodePosition == 1f) 
				Remove();
		}
		lastParam = nodePosition;
	}

	void ResetStopNodes () {
		
		for (int i = 0; i < nodePassed.Count; i++)
			nodePassed[i] = false;
	}
	
	/// <summary>
	/// Checks to see if the animal has reached a stop node
	/// </summary>
	private StopNodeProperties stopNodeProperties;
	bool Stop(out float stopNodePosition) { 
		
		float currentPosition = passedTime % 1f;
		for (int nodeIndex = 0; nodeIndex < stopNodeArray.Count; nodeIndex ++) {
			stopNodePosition = stopNodeArray[nodeIndex].Parameters [spline].PosInSpline;
			if (StopNodeReached(currentPosition, stopNodePosition, nodeIndex))
				return true;
		}
		stopNodePosition = -1f;
		return false;
	}
	
	bool StopNodeReached (float currentPosition, float nodePosition, int nodeIndex) {
		
		stopNodeProperties = stopNodeArray[nodeIndex].gameObject.GetComponent<StopNodeProperties>();
		if (currentPosition >= nodePosition && !nodePassed[nodeIndex]) {
			if (bird) {
				AnimationClip landAnimation = stopNodeProperties.landAnimation;
				stopIn = stopNodeProperties.landDuration;
				animation.CrossFade(landAnimation.name);
				splineState = landing;
			} else {
				AnimationClip idleAnimation = stopNodeProperties.idleAnimation;
				stopIn = stopNodeProperties.duration;
				animation.CrossFade(idleAnimation.name);
				splineState = idle;
			}	
			nodePassed[nodeIndex] = true;
			return true;
		}
		return false;
	}
	
	void landing () {
		
		if (stopIn <= 0) {
			splineState = idle;
			AnimationClip idleAnimation = stopNodeProperties.idleAnimation;
			stopIn = stopNodeProperties.duration;
			animation.CrossFade(idleAnimation.name);		
		} 
	}
	
	void idle () {
		
		if (stopIn <= 0) {
			if (bird) {
				AnimationClip takeOffAnimation = stopNodeProperties.takeOffAnimation;
				stopIn = stopNodeProperties.takeOffDuration;
				animation.CrossFade(takeOffAnimation.name);
				splineState = takingOff;
			} else {
				AnimationClip moveAnimation = stopNodeProperties.afterMovementAnimation;
				animation.CrossFade(moveAnimation.name);
				splineState = moving;
				if (stopNodeProperties.afterMovementSpeed > 0) 
					speed = stopNodeProperties.afterMovementSpeed;	
			}
		} 
	}

	void takingOff () {
		
		if (stopIn <= 0) {
			AnimationClip moveAnimation = stopNodeProperties.afterMovementAnimation;
			animation.CrossFade (moveAnimation.name);
			splineState = moving;
			if (stopNodeProperties.afterMovementSpeed > 0) 
				speed = stopNodeProperties.afterMovementSpeed;	
		}
	}
	
	/// <summary>
	/// Forces to idle state based on a selected node
	/// </summary>
	/// <returns><c>true</c>, if idle was forced, <c>false</c> otherwise.</returns>
	/// <param name="nodeIndex">Node index.</param>
	public bool ForceIdle (int stopNodeIndex, bool permaIdle) {
		
		if (stopNodeIndex >= stopNodeArray.Count)
			return false;
		for (int i = 0; i < stopNodeIndex - 1; i ++) 
			nodePassed[i] = true;
		
		passedTime = stopNodeArray[stopNodeIndex].Parameters [spline].PosInSpline;
		stopIn = 0; //force the state back to moving if it's not already
		splineState = moving;
		splineState ();
		return true;
	}
}
