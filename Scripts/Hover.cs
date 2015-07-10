using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

	public bool waterLily;
	void Start () {

		originalPos = transform.position;
		originalPos.y = originalHeight != 0 ? originalHeight : originalPos.y;
		hoverPosition = Random.value;
		RandomizeRotation ();
		if (waterLily)
			renderer.material = WaterLilyManager.instance.GetRandomMaterial ();
	}

	void RandomizeRotation () {

		float randomY = Random.Range (0f, 360f);
		Vector3 newEuler = transform.eulerAngles;
		newEuler.y = randomY;
		transform.rotation = Quaternion.Euler (newEuler);
	}
	
	void Update () {
		
		UpdatePosition ();
	}

	private float hoverPosition;
	private Vector3 originalPos;
	public float originalHeight;
	public float speed, height;
	public AnimationCurve hoverAnimation;
	void UpdatePosition () {

		hoverPosition = (hoverPosition + speed * Time.deltaTime) % 1;
		float heightPosition = hoverAnimation.Evaluate (hoverPosition);
		float newHeight = Mathf.Lerp (originalPos.y, originalPos.y + height, heightPosition);
		Vector3 newPositoon = transform.position;
		newPositoon.y = newHeight;
		transform.position = newPositoon;
	}
	
	
}
