using UnityEngine;
using System.Collections;
using AmplifyColor;

[System.Serializable]
public class FilterInfo {
	
	public Texture LutTexture;
	public float blend;
	public AnimationCurve effectOverYear = AnimationCurve.Linear(0, 1, 1, 1);
}

public class FilterManager : Singleton<FilterManager> {

	void Start () {
		
		amplifyColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		stormAmplifyColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		darkAmplifyColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		SceneManager.instance.OnNewDay += RandomizeFilters;
		RandomizeFilters ();
		darkAmplifyColorEffect.LutTexture = darkFilter.LutTexture;
	}
	
	void RandomizeFilters () {

		RandomizeMainFilter ();
		RandomizeStormFilter ();
	}

	public FilterInfo[] filters;
	void RandomizeMainFilter () {

		filterIndex = Random.Range (0, filters.Length);
		filter = filters[filterIndex];
		amplifyColorEffect.LutTexture = filter.LutTexture;
		blend = 1 - filter.effectOverYear.Evaluate (SceneManager.curvePos);
	}

	public FilterInfo[] stormFilters;
	void RandomizeStormFilter () {

		stormFilterIndex = Random.Range (0, stormFilters.Length);
		stormFilter = stormFilters [stormFilterIndex];
		stormAmplifyColorEffect.LutTexture = stormFilter.LutTexture;
		stormBlend = 1;
	}
	
	void Update () {

		UpdateStormFilter ();
		UpdateMainFilter ();
		UpdateDarkFilter ();
	}
	
	private FilterInfo stormFilter;
	private AmplifyColorEffect stormAmplifyColorEffect;
	private float _stormBlend, _stormBlendNormalized;
	public float stormBlend {
		get { return _stormBlendNormalized; } 
		set {
			_stormBlendNormalized = value;
			_stormBlend = Mathf.Lerp (stormFilter.blend, 1, _stormBlendNormalized); 
		}
	}
	
	void UpdateStormFilter () {

		float newBlend = _stormBlend;
		if (WeatherControl.currentWeather != null) {
			newBlend = Mathf.Lerp (1, newBlend, WeatherControl.instance.totalTransition);
			newBlend = WeatherControl.currentWeather.usesFilter ? Mathf.Lerp (1, newBlend, WeatherControl.currentWeather.weather.severity) : 1;
		} else {
			newBlend = 1;
		}

		newBlend = Mathf.Lerp(1, newBlend, SkyManager.instance.nightDayLerp);
		stormAmplifyColorEffect.BlendAmount = newBlend;
	}

	private FilterInfo filter;
	private AmplifyColorEffect amplifyColorEffect;
	private float _blend, _blendNormalized;
	public float blend {
		get { return _blendNormalized; } 
		set { 
			_blendNormalized = value;
			_blend = Mathf.Lerp(filter.blend, 1, _blendNormalized);
		}
	}

	void UpdateMainFilter () {

		float newBlend = _blend;
		if (WeatherControl.currentWeather != null) {
			float stormInfluence = 0;
			if (WeatherControl.currentWeather.usesFilter) 
				stormInfluence = Mathf.Lerp(0, WeatherControl.currentWeather.weather.severity, WeatherControl.instance.totalTransition);	
			newBlend = Mathf.Lerp(1, newBlend, 1 - stormInfluence);
		}

		newBlend = Mathf.Lerp(1, newBlend, SkyManager.instance.nightDayLerp);
		amplifyColorEffect.BlendAmount = newBlend;
	}

	public FilterInfo darkFilter;
	private AmplifyColorEffect darkAmplifyColorEffect;
	void UpdateDarkFilter () {

		float newBlend = Mathf.Lerp (1, darkFilter.blend, 1 - SkyManager.instance.intensityLerp);
		darkAmplifyColorEffect.BlendAmount = newBlend;
	}

	private int filterIndex, stormFilterIndex;
	public void NextFilter (bool _filter = false, bool _stormFilter = false) {

		if (_filter) {
			filterIndex = (int)Mathf.Repeat(filterIndex + 1, filters.Length);
			filter = filters[filterIndex];
			amplifyColorEffect.LutTexture = filter.LutTexture;
			blend = Random.value;
		}

		if (_stormFilter) {
			stormFilterIndex = (int)Mathf.Repeat(stormFilterIndex + 1, stormFilters.Length);
			stormFilter = stormFilters [stormFilterIndex];
			stormAmplifyColorEffect.LutTexture = stormFilter.LutTexture;
			stormBlend = 1;
		}
	}

	private bool _on = true;
	public bool on {
		get { return _on; }
		set {
			_on = value;
			amplifyColorEffect.enabled = _on;
			stormAmplifyColorEffect.enabled = _on;
			darkAmplifyColorEffect.enabled = _on;
		}
	}
}
