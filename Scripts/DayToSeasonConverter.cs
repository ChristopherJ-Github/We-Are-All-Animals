using UnityEngine;
using System;
using System.Collections;

public class DayToSeasonConverter : MonoBehaviour {
	
	void Start () {

		SetSeason ();
	}
	         
	void SetSeason () {

		int season = SceneManager.currentDate.Day % 4;
		DateTime currentDate = SceneManager.currentDate;
		switch (season) {
			case 0:
				SceneManager.currentDate = new DateTime (currentDate.Year, 1, 1, currentDate.Hour, currentDate.Minute, currentDate.Second);
				SnowManager.instance.snowLevel = 1;
				break;
			case 1:
				SceneManager.currentDate = new DateTime (currentDate.Year, 4, 8, currentDate.Hour, currentDate.Minute, currentDate.Second);
				SnowManager.instance.snowLevel = 0;
				break;
			case 2:
				SceneManager.currentDate = new DateTime (currentDate.Year, 6, 18, currentDate.Hour, currentDate.Minute, currentDate.Second);
				SnowManager.instance.snowLevel = 0;
				break;
			case 3:
				SceneManager.currentDate = new DateTime (currentDate.Year, 10, 28, currentDate.Hour, currentDate.Minute, currentDate.Second);
				SnowManager.instance.snowLevel = 0;
				break;
		}
		SceneManager.startupTime = SceneManager.currentDate;
	}
}
