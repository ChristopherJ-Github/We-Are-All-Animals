using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class SunControl : Singleton<SunControl> { 

	void OnEnable () { 

		SceneManager.instance.OnNewYear += SetSunCurves;
		SeasonToDay ();
		SetSunCurves ();
		SetDayCurve ();
		SetDayLengths ();
	}

	public static AnimationCurve sunriseAstroCurve, sunriseCurve, sunsetCurve, sunsetAstroCurve; 
	public float[] sunriseAstro; 
	public float[] sunrise;
	public float[] sunset;
	public float[] sunsetAstro;
	/// <summary>
	/// Initialize curves for each sun phase
	/// </summary>
	void SetSunCurves () { 

		sunriseAstroCurve = new AnimationCurve ();
		sunriseCurve = new AnimationCurve ();
		sunsetCurve = new AnimationCurve ();
		sunsetAstroCurve = new AnimationCurve ();
		for (int month = 1; month <= 12; month++) {
			DateTime monthStart = new DateTime (SceneManager.currentDate.Year,month,1);
			double monthStartMinutes = (monthStart- SceneManager.yearStart).TotalMinutes;
			float monthPos = (float)(monthStartMinutes/SceneManager.minsInYear);
			AddKeys(month, monthPos);
		}
	}

	void AddKeys (int month, float monthPos) {

		float sunriseAstroValue = sunriseAstro[month-1]/24; 
		float sunriseValue = sunrise[month-1]/24; 
		float sunsetValue = sunset[month-1]/24; 
		float sunsetAstroValue = sunsetAstro[month-1]/24; 
		sunriseAstroCurve.AddKey(monthPos, sunriseAstroValue);
		sunriseCurve.AddKey(monthPos,sunriseValue);
		sunsetCurve.AddKey(monthPos, sunsetValue);
		sunsetAstroCurve.AddKey(monthPos,sunsetAstroValue);
	}
	
	[HideInInspector] public AnimationCurve dayCurve;
	/// <summary>
	/// Set curve used in regular daily mode
	/// </summary>
	public void SetDayCurve() {
		
		dayCurve = new AnimationCurve ();
		float minsAtSuniseAstro = (float)(sunriseAstroTime * 60 + SceneManager.minsAtDayStart);
		float minsAtSunsetAstro = (float)(sunsetAstroTime * 60 + SceneManager.minsAtDayStart); 
		dayCurve.AddKey (minsAtSuniseAstro / (float)SceneManager.minsInYear, 0.0F);
		dayCurve.AddKey (minsAtSunsetAstro / (float)SceneManager.minsInYear, 1.0F);
	}
	
	public float dayLength { get { return sunsetTime - sunriseTime; } }
	public static float minDayLength, maxDayLength;
	void SetDayLengths () {

		minDayLength = 24;
		maxDayLength = 0;
		for (int month = 0; month < 12; month ++) {
			float dayLength = sunset[month] - sunrise[month];
			minDayLength = dayLength < minDayLength ? dayLength : minDayLength;
			maxDayLength = dayLength > maxDayLength ? dayLength : maxDayLength;
		}
	}

	/// <summary>
	/// Set curve used in seasonal mode for christmas demo
	/// </summary>
	public void SetSeasonCurve() {
		
		dayCurve = new AnimationCurve ();
		float minsAtSunise = (float)(SceneManager.minsAtDayStart);
		float minsAtSunset = (float)(SceneManager.minsAtDayStart + (60 * 24 * (365f/4f))); 
		dayCurve.AddKey (minsAtSunise / (float)SceneManager.minsInYear, 0.0F);
		dayCurve.AddKey (minsAtSunset / (float)SceneManager.minsInYear, 1.0F);	
	}

	/// <summary>
	/// Set to daily sunsets
	/// </summary>
	public void SeasonToDay () {

		SceneManager.instance.OnNewSeason -= SetSeasonCurve;
		SceneManager.instance.OnNewDay += SetDayCurve;
	}

	/// <summary>
	/// set to season based sunsets
	/// </summary>
	public void DayToSeason () {
		
		SceneManager.instance.OnNewSeason += SetSeasonCurve;
		SceneManager.instance.OnNewDay -= SetDayCurve;
	}

	public float sunriseAstroTime { get { return sunriseAstroCurve.Evaluate(SceneManager.curvePos) * 24; } }
	public float sunriseTime { get { return sunriseCurve.Evaluate(SceneManager.curvePos) * 24; } }
	public float sunsetTime { get { return sunsetCurve.Evaluate(SceneManager.curvePos) * 24; } }
	public float sunsetAstroTime { get { return sunsetAstroCurve.Evaluate(SceneManager.curvePos) * 24; } }

	public float posInNight {
		get {
			if (SceneManager.currentHour < 24 && SceneManager.currentHour > sunriseTime) {
				float minsAtSunset = (float)(sunsetTime * 60 + SceneManager.minsAtDayStart);
				float minsAtNextSunise = (float)(sunriseTime * 60 + SceneManager.minsAtDayStart + (24 * 60));
				return Tools.Math.Convert((float)SceneManager.currentMinutes, minsAtSunset, minsAtNextSunise, 0, 1);
			} else {
				float minsAtLastSunset = (float)(sunsetTime * 60 + SceneManager.minsAtDayStart - (24 * 60));
				float minsAtSunise = (float)(sunriseTime * 60 + SceneManager.minsAtDayStart);
				return Tools.Math.Convert((float)SceneManager.currentMinutes, minsAtLastSunset, minsAtSunise, 0, 1);
			}
		}
	}
	
	public float posInDay { 
		get { 
			float minsAtSuniseAstro = (float)(sunriseAstroTime * 60 + SceneManager.minsAtDayStart);
			float minsAtSunsetAstro = (float)(sunsetAstroTime * 60 + SceneManager.minsAtDayStart);
			return Tools.Math.Convert((float)SceneManager.currentMinutes, minsAtSuniseAstro, minsAtSunsetAstro, 0, 1);
		}
	}
}