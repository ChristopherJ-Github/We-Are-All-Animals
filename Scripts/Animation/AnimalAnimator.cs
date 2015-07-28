using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Animates a gameobject along the spline at a specific speed. 
/// </summary>
[AddComponentMenu("SuperSplines/Animation/Regular Animator")]
public class AnimalAnimator: MonoBehaviour {
	
	public AnimationCurve spawnChance;
	public bool bird;
	private delegate void splineStateHandler();
	private splineStateHandler splineState;
	public GameObject animal;
	private Animation animation;
	public AnimationClip initAnimation;
	private Spline spline;
	[HideInInspector] public bool dontIdle;

	void Start () {
		
		AnimationSpawner.instance.currentAnimations.Add(this);
		speed = initSpeed;
		animation = animal.GetComponent<Animation> ();
		if (initAnimation != null)
			animation.Play(initAnimation.name);
		List<Spline> splines = GetSplines ();
		splineIndex = GetSplineIndex(splines);
		if (splineIndex != -1) {
			spline = splines[splineIndex];
			InitStopNodeArray();
			splineState = moving;
		} else {
			Remove();
		}
	}

	public void Remove () {
		
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
	
	public int splineIndex = -1;
	int GetSplineIndex (List<Spline> splines) {
		
		if (this.splineIndex != -1)
			return this.splineIndex;
		List<int> availableSplines = Enumerable.Range (0,  splines.Count).ToList ();
		RemoveDuplicateSplines (ref availableSplines);
		RemoveIncompatibleSplines (ref availableSplines, splines);
		if (availableSplines.Count > 0) 
			return availableSplines[Random.Range(0, availableSplines.Count)];
		else 
			return -1;
	}

	void RemoveDuplicateSplines (ref List<int> availableSplines) {

		foreach (AnimalAnimator animation in AnimationSpawner.instance.currentAnimations) {
			if (animal.name == animation.animal.name) 
				availableSplines.Remove(animation.splineIndex);
		}
	}

	void RemoveIncompatibleSplines (ref List<int> availableSplines, List<Spline> splines) {

		for (int splineIndex = 0; splineIndex < splines.Count; splineIndex ++) {
			SplineProperties splineProperties = splines[splineIndex].GetComponent<SplineProperties>();
			if (splineProperties.CanSpawn())
				continue;
			else
				availableSplines.Remove(splineIndex);
		}
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
	private float speed;
	public float initSpeed = 1f;
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

		stopNodePosition = -1f;
		if (dontIdle)
			return false;
		float currentPosition = passedTime % 1f;
		for (int nodeIndex = 0; nodeIndex < stopNodeArray.Count; nodeIndex ++) {
			stopNodePosition = stopNodeArray[nodeIndex].Parameters [spline].PosInSpline;
			if (StopNodeReached(currentPosition, stopNodePosition, nodeIndex))
				return true;
		}
		return false;
	}

	private Vector3 rotationBeforeIdle;
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
				randomAnimationInterval = Random.Range(stopNodeProperties.minInterval, stopNodeProperties.maxInterval);
				splineState = idle;
			}	
			if (stopNodeProperties.rotation != Vector3.zero) {
				rotationBeforeIdle = animal.transform.localEulerAngles;
				StartCoroutine(Rotate(stopNodeProperties.rotation));
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
	
	IEnumerator Rotate (Vector3 targetRotation) {
		
		Quaternion targetQuaternion = Quaternion.Euler (targetRotation);
		Quaternion initQuaternion = animal.transform.localRotation;
		float timePassed = 0;
		float timeLimit = 0.3f;
		while (timePassed < timeLimit) {
			timePassed += Time.deltaTime;
			float rotationAmount = Mathf.Clamp01(timePassed/timeLimit);
			Quaternion currentQuaternion = Quaternion.Lerp(initQuaternion, targetQuaternion, rotationAmount);
			animal.transform.localRotation = currentQuaternion;
			yield return null;
		}
	}
	
	private float randomAnimationInterval;
	void idle () {

		RandomAnimationCheck ();
		StormCheck ();
		if (stopIn <= 0) {
			StopAllCoroutines();//prevent random animation from playing after
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
			if (stopNodeProperties.rotation != Vector3.zero) 
				StartCoroutine(Rotate(rotationBeforeIdle));
		} 
	}

	void StormCheck () {

		if (WeatherControl.instance.storm)
			stopIn = 0;
	}

	void RandomAnimationCheck () {

		if (stopNodeProperties.showRandomAnimFields) {
			randomAnimationInterval -= Time.deltaTime;
			if (randomAnimationInterval <= 0) {
				animation.CrossFade(stopNodeProperties.randomAnimation.name);
				StartCoroutine(CrossFadeQueued (stopNodeProperties.idleAnimation.name, 
				                                stopNodeProperties.randomAnimation.length * 0.9f));
				randomAnimationInterval = Random.Range(stopNodeProperties.minInterval, stopNodeProperties.maxInterval);
			}
		}
	}

	IEnumerator CrossFadeQueued (string animationName, float delay) {

		yield return new WaitForSeconds (delay);
		animation.CrossFade (animationName);
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
