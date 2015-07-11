using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

	public enum LilyType {waterLily, lilyPad}
	public LilyType lilyType;
	void Start () {

		InitValues ();
		RandomizeValues ();
	}

	void InitValues () {

		if (lilyType == LilyType.waterLily) {
			WaterLilyProperties waterLilyProperties = LilyManager.instance.waterLilyProperties;
			originalHeight = waterLilyProperties.originalHeight;
			speed = waterLilyProperties.speed;
			height = waterLilyProperties.height;
			hoverAnimation = waterLilyProperties.hoverAnimation;
			transform.localScale = Vector3.one * waterLilyProperties.scale;
		}
		if (lilyType == LilyType.lilyPad) {
			LilyPadProperties lilyPadProperties = LilyManager.instance.lilyPadProperties;
			originalHeight = lilyPadProperties.originalHeight;
			speed = lilyPadProperties.speed;
			height = lilyPadProperties.height;
			hoverAnimation = lilyPadProperties.hoverAnimation;
		}
		originalPos = transform.position;
		originalPos.y = originalHeight;
	}

	void RandomizeValues() {

		if (lilyType == LilyType.waterLily) {
			WaterLilyProperties waterLilyProperties = LilyManager.instance.waterLilyProperties;
			RandomizeTilt(waterLilyProperties);
			renderer.material = LilyManager.instance.GetRandomMaterial ();
		}
		if (lilyType == LilyType.lilyPad) {
			LilyPadProperties lilyPadProperties = LilyManager.instance.lilyPadProperties;
			float scale = Mathf.Lerp (lilyPadProperties.minScale, lilyPadProperties.maxScale, Random.value);
			transform.localScale = Vector3.one * scale;
			Color tint = Random.value > 0.5f ? lilyPadProperties.tint1 : lilyPadProperties.tint2;
			renderer.material.color = tint;
		}
		hoverPosition = Random.value;
		RandomizeRotation ();
	}

	void RandomizeTilt (WaterLilyProperties waterLilyProperties) {

		float xTilt = Random.Range (-waterLilyProperties.maxTilt, waterLilyProperties.maxTilt);
		float zTilt = Random.Range (-waterLilyProperties.maxTilt, waterLilyProperties.maxTilt);
		Vector3 newEuler = transform.eulerAngles;
		newEuler.x += xTilt;
		newEuler.z += zTilt;
		transform.rotation = Quaternion.Euler (newEuler);
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
	private float originalHeight;
	private float speed, height;
	private AnimationCurve hoverAnimation;
	void UpdatePosition () {

		hoverPosition = (hoverPosition + speed * Time.deltaTime) % 1;
		float heightPosition = hoverAnimation.Evaluate (hoverPosition);
		float newHeight = Mathf.Lerp (originalPos.y, originalPos.y + height, heightPosition);
		Vector3 newPositoon = transform.position;
		newPositoon.y = newHeight;
		transform.position = newPositoon;
	}
	
	
}
