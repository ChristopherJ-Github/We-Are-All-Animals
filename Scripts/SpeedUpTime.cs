using UnityEngine;
using System.Collections;

public class SpeedUpTime : MonoBehaviour
{
		/*
	 * small helper class to increase or decrease the speed of time in game
	  */
		
		void Update ()
		{
				if (Input.GetKey (KeyCode.A) && Time.timeScale > 0) {
						Time.timeScale -= 0.25f;
						
				}
				
				if (Input.GetKey (KeyCode.D) && Time.timeScale < 100) {
						Time.timeScale = Mathf.Clamp (Time.timeScale + 1, 0, 100);
						
				}
				if (Input.GetKeyDown (KeyCode.S))
						Time.timeScale = 1;
		}
}
