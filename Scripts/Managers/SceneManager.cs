using UnityEngine;
using System;
using System.Collections;

public class SceneManager : Singleton<SceneManager> {
							
	void Awake () {

		InitializeCurrentDate ();
		InitializeTriggers ();
		InitializeValues ();
	}

	public static DateTime currentDate = DateTime.Now;	
	void InitializeCurrentDate () {
		
		currentDate = currentDate.AddMinutes(60 * 24 * (int)(365/4));
		startupTime = currentDate;
	}

	void InitializeTriggers () {

		GUIManager.instance.OnGuiSliderEvent += ChangeTime;
		OnNewYear += UpdateYear;
		OnNewMonth += UpdateSeason;
		OnNewDay += UpdateDay;
		OnNewHour += UpdateHour;
	}

	void InitializeValues () {

		UpdateYear ();
		UpdateDay ();
		UpdateCurvePosition ();
		currentSeason = Mathf.CeilToInt(currentDate.Month/3f);
		Application.runInBackground = true;
		//Screen.SetResolution (1920, 785, true);
	}

	void Update () {

		UpdateSecTimer ();
		UpdateMinTimer ();
		UpdateCurvePosition ();
		FireTriggers();
	}

	private float secTimer = 1;
	public event TimeTrigger OnNewSec;
	void UpdateSecTimer () {

		if (secTimer > 0) {
			secTimer -= Time.deltaTime;
		} else if (secTimer <= 0) {
			TriggerTimeEvent(OnNewSec);
			currentDate = currentDate.AddSeconds (1);
			secTimer = 1;
		}
	}

	private float minTimer = 60;
	public event TimeTrigger OnNewMin;
	void UpdateMinTimer () {

		if (minTimer > 0) {
			minTimer -= Time.deltaTime;
		} else if (minTimer <= 0) {
			TriggerTimeEvent(OnNewMin);
			minTimer = 60;
		}
	}

	public static double minsInYear;
	public static double currentMinutes;
	public static float curvePos, curvePosDay;
	void UpdateCurvePosition () {

		currentMinutes = (currentDate - yearStart).TotalMinutes;
		if (curvePos != 1 ) {
			curvePos = (float)(currentMinutes/minsInYear);
			curvePosDay = (float)((currentDate - dayStart).TotalMinutes / (24 * 60));
			if (curvePos > 1f ) 
				curvePos = curvePos - 1f;
			else if (curvePos > 1f) 
				curvePos = 1f;
		}
	}

	public delegate void TimeTrigger (); 	
	public event TimeTrigger OnNewFrame;
	public event TimeTrigger OnNewHour;
	public event TimeTrigger OnNewDay;
	public event TimeTrigger OnNewMonth;
	public event TimeTrigger OnNewSeason;
	public event TimeTrigger OnNewYear;
	public static int currentSeason; 
	void FireTriggers () {

		if (yearEnd.Year != currentDate.Year) 
			TriggerTimeEvent(OnNewYear);
		if (dayStart.Month != currentDate.Month) 
			TriggerTimeEvent(OnNewMonth);
		if (currentSeason != Mathf.FloorToInt(exactSeason)) {
			TriggerTimeEvent(OnNewSeason);
			currentSeason = Mathf.FloorToInt(exactSeason);
		}
		if (dayStart.Day != currentDate.Day) 
			TriggerTimeEvent(OnNewDay);
		if (currentHour!= currentDate.Hour)
			TriggerTimeEvent (OnNewHour);
		TriggerTimeEvent (OnNewFrame);
	}

	void TriggerTimeEvent (TimeTrigger timeTrigger) {

		if (timeTrigger != null)
			timeTrigger ();
	}

	public static DateTime yearStart, yearEnd;
	void UpdateYear ()  {

		yearStart = new DateTime (currentDate.Year, 1, 1);
		yearEnd = new DateTime (currentDate.Year, 12, 31, 23, 59, 59, 99);
		minsInYear = (yearEnd - yearStart).TotalMinutes;
	}

	public static float exactSeason;
	void UpdateSeason () {
			
		int monthShift = 2;
		exactSeason = (((currentDate.Month + monthShift) % 13)/3f) % 4;
	}

	public static DateTime dayStart;
	public static double minsAtDayStart;
	void UpdateDay () {

		dayStart = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day);
		minsAtDayStart = (dayStart - yearStart).TotalMinutes;
	}

	public static int currentHour;
	void UpdateHour ()  {

		currentHour = currentDate.Hour;
	}

	public static DateTime startupTime;
	public void ChangeTime (float from, float to, float val) {  

		DateTime newDate = startupTime;
		currentDate = newDate.AddMinutes (val);
	}

	public void ChangeTimeSeconds (float from, float to, float val) {   

		DateTime newDate = startupTime;
		currentDate = newDate.AddSeconds(val);
	}

	public float DateToPosition (DateTime date) {

		double minutesAtDate = (date - yearStart).TotalMinutes;
		float curvePosition = (float)(minutesAtDate/minsInYear);
		return curvePosition;
	}

	public static DateTime realDate {
		get { return currentDate.AddMinutes(-60 * 24 * (int)(365 / 4));}
	}
}
