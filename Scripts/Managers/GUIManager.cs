using UnityEngine;
using System.Collections;

public class GUIManager : Singleton<GUIManager>
{
	public GUISkin gSkin;
	public GUIStyle gStyle;
	public GUIStyle sidebarStyle;
	public Vector2 scrollPosition = Vector2.zero;
	[HideInInspector] public bool toggleStats = false;
	public bool sliderUsed;
	float delay;
	float delayVal = 1f;
	
	void Start ()
	{
		delay = delayVal;
	}
	
	float yearSliderValue = 0.0F;
	float daySliderValue = 0.0F;
	float severitySliderValue = 0.0F;
	void OnGUI ()
	{
		if (!SceneManager.instance.doTick)
			return;
		float yearMin = 0;
		float yearMax = 525600; 
		float dayMin = 0;
		float dayMax = 1440;
		GUI.skin = gSkin;
		float tempVal = yearSliderValue;
		float tempVal2 = daySliderValue;
		float tempVal3 = severitySliderValue;
		
		delay -= Time.deltaTime;
		if (delay <= 0) {
			delay = 0;
			if (Input.GetKeyDown (KeyCode.Space)) {
				toggleStats = !toggleStats;
				delay = delayVal;
			}
		}
		
		if (toggleStats) {
			//horizontal slider bar for purpose of scrubbing through timeline
			yearSliderValue = GUI.HorizontalSlider (new Rect ((Screen.width / 2) - 175, Screen.height - Screen.height / 6.5f, 350, 120), yearSliderValue, yearMin, yearMax);
			daySliderValue = GUI.HorizontalSlider (new Rect ((Screen.width / 2) - 175, Screen.height - Screen.height / 3.25f, 350, 120), daySliderValue, dayMin, dayMax);
			severitySliderValue = GUI.VerticalSlider (new Rect (Screen.width * 0.98f, Screen.height * 1f/10f, 10, Screen.height * 9f/10), severitySliderValue, 1f, 0f);
			
			
			int labelHeight = 200;
			int labelWidth = 200;
			float weatherTransition;
			if (WeatherControl.currentWeather != null) 
				weatherTransition = WeatherControl.currentWeather.changesClouds ? (WeatherControl.instance.cloudTransition + WeatherControl.instance.transition)/2: WeatherControl.instance.transition;
			else 
				weatherTransition = 0;
			string text = "Weather Transition: " + Mathf.FloorToInt((weatherTransition) * 100).ToString() + "%";
			GUI.Label(new Rect(Screen.width - labelWidth, 0, labelWidth,labelHeight), text, gStyle);
			
			buttons();
			WeatherStats();
			DisplayCurvePosition();
		}
		// if there is a change in the slider Value, send an event with the info
		sliderUsed = false;
		if (yearSliderValue != tempVal || daySliderValue != tempVal2) {
			TriggerGuiEvent (yearMin, yearMax, yearSliderValue + daySliderValue);
			sliderUsed = true;
		}
		if (severitySliderValue != tempVal3)
			TriggerGuiEvent (severitySliderValue);
	}

	public float alphaSavePercentage, detailSavePercentage;
	public float alphaSaveTotal, detailSaveTotal;
	public float alphaSaveCurrent, detailSaveCurrent;
	void buttons () {

		if (WeatherControl.instance.safeToPress) {
			GUI.backgroundColor = Color.grey;
			if (GUI.Button (new Rect(Screen.width * 0.8f, Screen.height * 0.11f, Screen.width * 0.17f, Screen.height * 0.15f), "Spawn Weather")) 
				WeatherControl.instance.RandomlySpawn();
		} else {
			GUI.backgroundColor = Color.grey;
			if (GUI.Button (new Rect(Screen.width * 0.8f, Screen.height * 0.11f, Screen.width * 0.17f, Screen.height * 0.15f), "Stop Weather")) {
				WeatherControl.instance.TurnOff();
			}
		}

		GUI.backgroundColor = Color.grey;
		if (GUI.Button (new Rect (Screen.width * 0.8f, Screen.height * 0.59f, Screen.width * 0.17f, Screen.height * 0.1f), "Update Greenery " + (int)GrassManager.instance.progress +"%")) {

			FlowerManager.instance.SetDetailMaps();
			GrassManager.instance.SetAlphaMaps();
		}
		
		if (GUI.Button (new Rect (Screen.width * 0.75f, Screen.height * 0.59f, Screen.width * 0.05f, Screen.height * 0.1f), "Stop")) {
			
			FlowerManager.instance.StopAllCoroutines();
			GrassManager.instance.StopAllCoroutines();
		}
		
		if (GUI.Button (new Rect(Screen.width * 0.8f, Screen.height * 0.48f, Screen.width * 0.17f, Screen.height * 0.1f), "Export Alpha Maps " + (int)alphaSavePercentage +"%")) {
			
			TerrainData terrainData = Terrain.activeTerrain.terrainData;
			alphaSaveTotal = terrainData.alphamapLayers * terrainData.alphamapWidth * terrainData.alphamapHeight;
			alphaSaveCurrent = 0;
#if !UNITY_WEBPLAYER
			TerrainAlphaExporter.instance.extractAlphaMaps();
#endif
		}
		
		if (GUI.Button (new Rect(Screen.width * 0.63f, Screen.height * 0.48f, Screen.width * 0.17f, Screen.height * 0.1f), "Export Detail Maps " + (int)detailSavePercentage +"%")) {
			
			TerrainData terrainData = Terrain.activeTerrain.terrainData;
			detailSaveTotal = terrainData.detailPrototypes.Length * terrainData.detailWidth * terrainData.detailHeight;
			detailSaveCurrent = 0;
#if !UNITY_WEBPLAYER
			TerrainDetailExporter.instance.extractDetailMaps();
#endif
		}
	}
	
	void WeatherStats () {

		int y = 0;
		int width = 200;
		int height = 25;

		int virtualHeight = WeatherControl.instance.weatherTypes.Length * height;
		virtualHeight = Mathf.Clamp (virtualHeight, Screen.height + 1, virtualHeight); //keep the scrollbar visible
		GUI.backgroundColor = Color.red;
		scrollPosition = GUI.BeginScrollView(new Rect(0, 0, width, Screen.height), scrollPosition, new Rect(0, 0, width - 20, virtualHeight));
		foreach (WeatherInfo weatherType in WeatherControl.instance.weatherTypes) {
			CreateWeatherLabel (weatherType, y, width, height);
			y += height;
		}
		GUI.EndScrollView ();
	}

	void CreateWeatherLabel (WeatherInfo weatherType, int y, int width, int height) {

		float alpha = weatherType.weather.activeSelf ? 1f : 0.45f;
		sidebarStyle.normal.textColor = new Color(1.0f, 1.0f, 1.0f, alpha);
		string name = weatherType.weather.name;
		int percentage = Mathf.FloorToInt(weatherType.spawnChance * 100);
		GUI.Label(new Rect(0, y, width, height), name + ": " + percentage + "%", sidebarStyle);
		percentage = Mathf.FloorToInt (WeatherControl.instance.GetMaxSeverity (weatherType) * 100);
		name = "Str";
		GUI.Label(new Rect(100, y, width, height), name + ": " + percentage + "%", sidebarStyle);
	}

	void DisplayCurvePosition () {

		int originaFontSize = sidebarStyle.fontSize;
		sidebarStyle.fontSize = 0;
		GUI.Label(new Rect(Screen.width * 0.892f, Screen.height * 0.41f, Screen.width * 200, Screen.height * 25), 
		          "curve position",
		          sidebarStyle);
		sidebarStyle.fontSize = originaFontSize;
		GUI.Label(new Rect(Screen.width * 0.883f, Screen.height * 0.437f, Screen.width * 200, Screen.height * 25), 
		          SceneManager.curvePos.ToString(), 
		          sidebarStyle);
	}
	
	public void UpdateAlphaSavePercentage (int toAdd) {
		
		alphaSaveCurrent += toAdd;
		alphaSavePercentage = alphaSaveCurrent/alphaSaveTotal * 100;
	}
	
	public void UpdateDetailSavePercentage (int toAdd) {
		
		detailSaveCurrent += toAdd;
		detailSavePercentage = detailSaveCurrent/detailSaveTotal * 100;
	}
	
	/// <summary>
	/// GuiEvents to fire information to other scripts when needed
	/// </summary>
	public delegate void GuiSliderEvent (float from,float to,float val);
	public event GuiSliderEvent OnGuiSliderEvent;
	public delegate void GuiEvent (float val);
	public event GuiEvent OnGuiEvent;
	
	public void TriggerGuiEvent (float from, float to, float val)
	{
		if (OnGuiSliderEvent != null) {
			OnGuiSliderEvent (from, to, val);
		}
	}
	
	public void TriggerGuiEvent (float val)
	{
		if (OnGuiEvent != null) {
			OnGuiEvent (val);
		}
	}
}
