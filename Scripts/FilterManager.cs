using UnityEngine;
using System.Collections;
using AmplifyColor;

[System.Serializable]
public class FilterInfo {
	
	public Texture LutTexture;
	public float blend;
}

[System.Serializable]
public class StormFilterInfo : FilterInfo {
	
	public AnimationCurve effectOverYear = AnimationCurve.Linear(0, 1, 1, 1);

	public StormFilterInfo (FilterInfo filterInfo) {

		LutTexture = filterInfo.LutTexture;
		blend = filterInfo.blend;
	}
}

[System.Serializable]
public class FilterGroup {

	public AnimationCurve effectOverYear = AnimationCurve.Linear(0, 1, 1, 1);
	public FilterInfo[] filters;
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

	[Tooltip("Leave blank")] 
	public Texture currentLut;
	public FilterGroup[] filterGroups;
	void RandomizeMainFilter () {

		int groupIndex = Random.Range (0, filterGroups.Length);
		FilterGroup filterGroup = filterGroups [groupIndex];
		int filterIndex = Random.Range (0, filterGroup.filters.Length);
		SetMainFilter (groupIndex, filterIndex);
	}

	private FilterGroup filterGroup;
	void SetMainFilter (int groupIndex, int filterIndex) {

		this.groupIndex = groupIndex;
		filterGroup = filterGroups [groupIndex];
		this.filterIndex = filterIndex;
		filter = filterGroup.filters[filterIndex];
		amplifyColorEffect.LutTexture = filter.LutTexture;
		currentLut = filter.LutTexture;
		float currentEffect = filterGroup.effectOverYear.Evaluate (SceneManager.curvePos);
		currentEffect = Mathf.Clamp01 (currentEffect);
		blend = 1 - currentEffect;
	}

	[Tooltip("Leave blank")] 
	public Texture currentStormLut;
	public StormFilterInfo[] stormFilters;
	void RandomizeStormFilter () {

		int filterIndex = Random.Range (0, stormFilters.Length);
		bool allowMainFilterCopying = Random.value >= 0.5f;
		SetStormFilter (filterIndex, allowMainFilterCopying);
	}

	void SetStormFilter (int filterIndex, bool allowMainFilterCopying) {

		stormFilterIndex = filterIndex;
		stormFilter = stormFilters [filterIndex];
		if (WeatherControl.currentWeather != null && allowMainFilterCopying) 
			if (WeatherControl.currentWeather.weather.name == "Fog") 
				stormFilter = new StormFilterInfo (filter);	
		stormAmplifyColorEffect.LutTexture = stormFilter.LutTexture;
		currentStormLut = stormFilter.LutTexture;
		stormBlend = 0;
	}

	void Update () {

		UpdateMainFilter ();
		UpdateStormFilter ();
		UpdateDarkFilter ();
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
				stormInfluence = Mathf.Lerp(0, WeatherControl.instance.severity, WeatherControl.instance.totalTransition);	
			newBlend = Mathf.Lerp(1, newBlend, 1 - stormInfluence);
		}
		newBlend = Mathf.Lerp(1, newBlend, SkyManager.instance.nightDayLerp);
		amplifyColorEffect.BlendAmount = newBlend;
	}

	private StormFilterInfo stormFilter;
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
			float weatherEffect = WeatherControl.instance.totalTransition * WeatherControl.instance.severity;
			float currentEffect = stormFilter.effectOverYear.Evaluate(SceneManager.curvePos);
			newBlend = Mathf.Lerp (1, newBlend, weatherEffect * currentEffect);
			newBlend = WeatherControl.currentWeather.usesFilter ? newBlend : 1;
		} else {
			newBlend = 1;
		}
		newBlend = Mathf.Lerp(1, newBlend, SkyManager.instance.nightDayLerp);
		stormAmplifyColorEffect.BlendAmount = newBlend;
	}

	public FilterInfo darkFilter;
	private AmplifyColorEffect darkAmplifyColorEffect;
	void UpdateDarkFilter () {

		float newBlend = Mathf.Lerp (1, darkFilter.blend, 1 - SkyManager.instance.intensityLerp);
		darkAmplifyColorEffect.BlendAmount = newBlend;
	}

	private int groupIndex, filterIndex, stormFilterIndex;
	public void NextFilter (bool changeMainFilter = false, bool changeStormFilter = false) {

		if (changeMainFilter) {
			int filterIndex = this.filterIndex + 1;
			int groupIndex = this.groupIndex;
			if (filterIndex >= filterGroup.filters.Length) {
				groupIndex = (int)Mathf.Repeat(groupIndex + 1, filterGroups.Length);
				filterIndex = 0;
			} 
			SetMainFilter(groupIndex, filterIndex);
		}
		if (changeStormFilter) {
			int stormFilterIndex = (int)Mathf.Repeat(this.stormFilterIndex + 1, stormFilters.Length);
			SetStormFilter(stormFilterIndex, false);
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
