using UnityEngine;
using System.Collections;
using System;

public class SceneManager : Singleton<SceneManager>
{

		public static GameObject mainScene;	 						//an array of all scenes to instantiate/destroy on timing					
		public static GameObject currentScene;							//reference to current scene
		public static DateTime currentDate = DateTime.Now;			//public for accesibility from GUI
		public static DateTime startupTime;
		public static DateTime realDate {
			get {
				return currentDate.AddMinutes(-60 * 24 * (int)(365 / 4));
			}
		}

		public static DateTime dayStart;
		public static DateTime yearStart;
		public static DateTime yearEnd;
		public static double minsInYear;
		public static double minsAtDayStart;
		public static double currentMinutes;
		public static int currentHour;
		public static int currentSeason; 
		public static float season; //exact season

		public delegate void SpawnObject (); 		//delegate/event to spawn random objects at random times depending on time of day/scene
		public event SpawnObject OnNewFrame;
		public event SpawnObject OnNewSec;
		public event SpawnObject OnNewMin;
		public event SpawnObject OnNewHour;
		public event SpawnObject OnNewDay;
		public event SpawnObject OnNewMonth;
		public event SpawnObject OnNewSeason;
		public event SpawnObject OnNewYear;
		
		public static float curvePos;
		public static float curvePosDay;

		public void TriggerNewFrame () {

				if (OnNewFrame != null)
						OnNewFrame ();
		}
		
		public void TriggerNewSec ()				
		{
				if (OnNewSec != null) 
						OnNewSec ();
		}

		public void TriggerNewMin ()				
		{
				if (OnNewMin != null) 
						OnNewMin ();
		}
		
		public void TriggerNewHour ()				
		{
				if (OnNewHour != null) 
					OnNewHour ();
		}
		
		public void TriggerNewDay ()					
		{
				if (OnNewDay != null) 
					OnNewDay ();
		}	

		public void TriggerNewMonth ()				
		{
				if (OnNewMonth != null) 
						OnNewMonth ();
		}

		public void TriggerNewSeason ()				
		{
				if (OnNewSeason != null) 
						OnNewSeason ();
		}

		public void TriggerNewYear ()				
		{
				if (OnNewYear != null) 
					OnNewYear ();
		}
	
		void Awake ()//hook in and remove event methods in OnEnable and OnDisable to prevent weak links
		{
				GUIManager.instance.OnGuiSliderEvent += ChangeTime;
				this.AfterFadeEvent += InitAfterFade;
				this.StartFadeEvent += PauseBeforeFade;
				this.OnNewYear += UpdateYear;
				this.OnNewMonth += UpdateSeason;
				this.OnNewDay += UpdateDay;
				this.OnNewHour += UpdateHour;
				
				currentDate = currentDate.AddMinutes(60 * 24 * (int)(365/4));
				startupTime = currentDate;
				//initialize year and day
				UpdateYear ();
				UpdateDay ();
				
				currentSeason = Mathf.CeilToInt(currentDate.Month/3f);
				currentMinutes = (currentDate - yearStart).TotalMinutes;
				curvePos = (float)(currentMinutes/minsInYear);
				Application.runInBackground = true;
		}

		void OnDisable ()
		{
				if (GUIManager.instance != null)
						GUIManager.instance.OnGuiSliderEvent -= ChangeTime;
				this.AfterFadeEvent -= InitAfterFade;
				this.StartFadeEvent -= PauseBeforeFade;
				this.OnNewYear -= UpdateYear;
				this.OnNewDay -= UpdateDay;
		}
		
		
		IEnumerator Start ()
		{
				//endDate is created 18 hours in the future from startupTime (length of program running)
				startupTime = currentDate;
				var hour = currentDate.AddHours (18).Hour;
				TriggerNewMin ();
				//DateTime endDate = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day, hour, currentDate.Minute, currentDate.Second);
				//just to ensure manager is initialized from InitManager
				yield return Auto.Wait (1);

				currentScene = mainScene;
				currentScene.SetActive (true);
		}
		
		float timer = 1;
		float minTimer = 60;
		public bool doTick = false;
		void Update ()
		{
				if (!doTick)
						return;
				//Timer used to add seconds to Realtime, affected by Time.timeScale
				if (timer > 0)
						timer -= Time.deltaTime;
				else if (timer <= 0) {
						TriggerNewSec ();
						currentDate = currentDate.AddSeconds (1);
						timer = 1;
				}

				//Timer for checking current scene and attempting to spawn an object once per minute
				if (minTimer > 0)
						minTimer -= Time.deltaTime;
				else if (minTimer <= 0) {
						Debug.Log ("TRIGGER SPAWNED ----------------------------------");
						TriggerNewMin ();
						minTimer = 60;
				}
				currentMinutes = (currentDate - yearStart).TotalMinutes;
				if (curvePos != 1 ) {
					curvePos = (float)(currentMinutes/minsInYear);
					curvePosDay = (float)((currentDate - dayStart).TotalMinutes / (24 * 60));
					
					if (curvePos > 1f ) {
						curvePos = curvePos - 1f;
					} else if (curvePos > 1f) {
						curvePos = 1f;
					}
				}
				FireTriggers();
				TriggerNewFrame();
		}

		void FireTriggers ()
		{
			if (yearEnd.Year != currentDate.Year) 
				TriggerNewYear ();
			if (dayStart.Month != currentDate.Month) 
				TriggerNewMonth ();
			if (currentSeason != Mathf.FloorToInt(season)) {
				TriggerNewSeason();
				currentSeason = Mathf.FloorToInt(season);
			}
			if (dayStart.Day != currentDate.Day) 
				TriggerNewDay ();
			if (currentHour!= currentDate.Hour)
				TriggerNewHour ();
		}

		void UpdateYear () 
		{
			yearStart = new DateTime (currentDate.Year, 1, 1);
			yearEnd = new DateTime (currentDate.Year, 12, 31, 23, 59, 59, 99);
			minsInYear = (yearEnd - yearStart).TotalMinutes;
		}
		
		void UpdateSeason () {
				
			int monthShift = 2;
			season = (((currentDate.Month + monthShift) % 13)/3f) % 4;
		}

		void UpdateDay ()
		{
			dayStart = new DateTime (currentDate.Year, currentDate.Month, currentDate.Day);
			minsAtDayStart = (dayStart - yearStart).TotalMinutes;
		}

		void UpdateHour () 
		{
			currentHour = currentDate.Hour;
		}

		//changes time by minutes and calls checkTime 
		public void ChangeTime (float from, float to, float val)
		{   
				DateTime newDate = startupTime;
				currentDate = newDate.AddMinutes (val);
		}

		public void ChangeTimeSeconds (float from, float to, float val)
		{   
			DateTime newDate = startupTime;
			currentDate = newDate.AddSeconds(val);
		}
	
	
	//logic to load up various scenes and destroy old ones based on current hour
		// note, this can easily be extended into a generic system for scenes/subscenes, with generic actions on particular hours.


		//quick helper class
		public static GameObject DestroyAndSpawn (GameObject gObj)
		{	
				currentScene.SetActive (false);
				Debug.Log(currentScene.activeSelf);
				gObj.SetActive (true);
				return gObj;
		}

		void InitAfterFade (int fadeTimes)
		{
				//if (fadeTimes < 1)
				doTick = true;
		}

		void PauseBeforeFade (int fadeTimes)
		{
				//if (fadeTimes > 0)
				doTick = false;
		}



		public delegate void AfterFade (int hour); 		
		public event AfterFade AfterFadeEvent, StartFadeEvent;

	
		public void TriggerAfterFade (int hour)
		{
				if (AfterFadeEvent != null) {
						AfterFadeEvent (hour);
				}
		}
	
		public void TriggerStartFade (int hour)
		{
				if (StartFadeEvent != null) {
						StartFadeEvent (hour);
				}
		}


}
