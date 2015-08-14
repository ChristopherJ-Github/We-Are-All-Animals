using UnityEngine;
using System.Collections;
using AmplifyColor;

[System.Serializable]
public class FilterInfo {
	
	public Texture LutTexture;
	public float blend;
}

[System.Serializable]
public class FilterGroup {

	public AnimationCurve effectOverYear = AnimationCurve.Linear(0, 1, 1, 1);
	public FilterInfo[] filters;
}

public class FilterManager : Singleton<FilterManager> {

	void Start () {
		
		amplifyColorEffect = Camera.main.gameObject.AddComponent<AmplifyColorEffect> ();
		SceneManager.instance.OnNewDay += RandomizeFilter;
		RandomizeFilter();
	}

	[Tooltip("Leave blank")] 
	public Texture currentLut;
	public FilterGroup[] filterGroups;
	void RandomizeFilter () {

		int groupIndex = GetGroupIndex ();
		FilterGroup filterGroup = filterGroups [groupIndex];
		int filterIndex = GetFilterIndex (filterGroup);
		SetFilter (groupIndex, filterIndex);
	}

	int GetGroupIndex () {

		float largestEffect = 0;
		int outputIndex = 0;
		for (int index = 0; index < filterGroups.Length; index ++) {
			float effect = filterGroups[index].effectOverYear.Evaluate(SceneManager.curvePos);
			if (effect > largestEffect) {
				largestEffect = effect;
				outputIndex = index;
			}
		}
		return outputIndex;
	}

	int GetFilterIndex (FilterGroup filterGroup) {

		if (0 == Random.Range(0, filterGroup.filters.Length + 1)) 
			return -1;
		else
			return Random.Range (0, filterGroup.filters.Length);
	}

	private FilterGroup filterGroup;
	void SetFilter (int groupIndex, int filterIndex) {

		this.groupIndex = groupIndex;
		filterGroup = filterGroups [groupIndex];
		this.filterIndex = filterIndex;
		if (filterIndex != -1)
			filter = filterGroup.filters[filterIndex];
		else
			filter = new FilterInfo ();
		amplifyColorEffect.LutTexture = filter.LutTexture;
		currentLut = filter.LutTexture;
		float currentEffect = filterGroup.effectOverYear.Evaluate (SceneManager.curvePos);
		currentEffect = Mathf.Clamp01 (currentEffect);
		blend = 1 - currentEffect;
	}

	void Update () {

		#if UNITY_EDITOR
			UpdateBlend ();
		#endif
		UpdateMainFilter ();
	}
	
	private float _blend, _blendNormalized;
	public float blend {
		get { return _blendNormalized; } 
		set { 
			_blendNormalized = value;
			_blend = Mathf.Lerp(filter.blend, 1, _blendNormalized);
		}
	}

	public static FilterInfo filter;
	public static AmplifyColorEffect amplifyColorEffect;
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

	void UpdateBlend () {

		float currentEffect = filterGroup.effectOverYear.Evaluate (SceneManager.curvePos);
		currentEffect = Mathf.Clamp01 (currentEffect);
		blend = 1 - currentEffect;
	}
	
	private int groupIndex, filterIndex;
	public void NextFilter () {

		int filterIndex = this.filterIndex;
		filterIndex = (int)Mathf.Repeat (filterIndex + 1, filterGroup.filters.Length);
		SetFilter(groupIndex, filterIndex);
	}
}
