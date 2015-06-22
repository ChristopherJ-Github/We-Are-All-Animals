using UnityEngine;
using System.Collections;

public class TamiraBirthday : MonoBehaviour {

	public WeatherControl weatherControl;
	
	void OnEnable () {

		CloudControl.instance.SetOvercast (0);
		weatherControl.gameObject.SetActive (false);
	}

	void OnDisable () {

		weatherControl.gameObject.SetActive (true);
	}
}
