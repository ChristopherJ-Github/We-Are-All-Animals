using UnityEngine;
using System.Collections;
using AmplifyColor;

public class StormFilterManager : Singleton<StormFilterManager> {
	
	void Start () {

		amplifyColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		SceneManager.instance.OnNewDay += RandomizeFilter;
		RandomizeFilter ();
	}

	[Tooltip("Leave blank")] 
	public Texture currentLut;
	public FilterGroup[] filterGroups;
	void RandomizeFilter () {
		
		int groupIndex = Random.Range (0, filterGroups.Length);
		FilterGroup filterGroup = filterGroups [groupIndex];
		int filterIndex = Random.Range (0, filterGroup.filters.Length);
		bool allowMainFilterCopying = Random.value >= 0.5f;
		SetFilter (groupIndex, filterIndex, allowMainFilterCopying);
	}
	
	private FilterGroup filterGroup;
	void SetFilter (int groupIndex, int filterIndex, bool allowMainFilterCopying) {
		
		this.groupIndex = groupIndex;
		filterGroup = filterGroups [groupIndex];
		this.filterIndex = filterIndex;
		filter = filterGroup.filters [filterIndex];
		if (WeatherControl.currentWeather != null && allowMainFilterCopying) 
			if (WeatherControl.currentWeather.weather.name == "Fog") 
				filter = FilterManager.filter;	
		amplifyColorEffect.LutTexture = filter.LutTexture;
		currentLut = filter.LutTexture;
		blend = 0;
	}
	
	void Update () {

		UpdateFilter ();
	}
	
	private float _blend, _blendNormalized;
	public float blend {
		get { return _blendNormalized; } 
		set {
			_blendNormalized = value;
			_blend = Mathf.Lerp (filter.blend, 1, _blendNormalized); 
		}
	}
	
	private FilterInfo filter;
	public static AmplifyColorEffect amplifyColorEffect;
	public AnimationCurve fogEffectOverYear;
	void UpdateFilter () {
		
		float newBlend = _blend;
		if (WeatherControl.currentWeather != null) {
			float weatherEffect = WeatherControl.instance.totalTransition * WeatherControl.instance.severity;
			float currentEffect = filterGroup.effectOverYear.Evaluate(SceneManager.curvePos);
			float fogEffect = currentEffect;
			if (WeatherControl.currentWeather.weather.name == "Fog") 
				fogEffect *= fogEffectOverYear.Evaluate(SceneManager.curvePos);
			float leafEffect = fogEffect * LeafFallManager.instance.leafAmount;
			newBlend = Mathf.Lerp (1, newBlend, weatherEffect * leafEffect);
			newBlend = WeatherControl.currentWeather.usesFilter ? newBlend : 1;
		} else {
			newBlend = 1;
		}
		newBlend = Mathf.Lerp(1, newBlend, SkyManager.instance.nightDayLerp);
		amplifyColorEffect.BlendAmount = newBlend;
	}

	private int groupIndex, filterIndex;
	public void NextFilter () {

		int filterIndex = this.filterIndex + 1;
		int groupIndex = this.groupIndex;
		if (filterIndex >= filterGroup.filters.Length) {
			groupIndex = (int)Mathf.Repeat(groupIndex + 1, filterGroups.Length);
			filterIndex = 0;
		} 
		SetFilter(groupIndex, filterIndex, false);
	}
}
