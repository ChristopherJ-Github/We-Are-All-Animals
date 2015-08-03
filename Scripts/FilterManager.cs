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

		int groupIndex = Random.Range (0, filterGroups.Length);
		FilterGroup filterGroup = filterGroups [groupIndex];
		int filterIndex = Random.Range (0, filterGroup.filters.Length);
		SetFilter (groupIndex, filterIndex);
	}

	private FilterGroup filterGroup;
	void SetFilter (int groupIndex, int filterIndex) {

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
		int groupIndex = this.groupIndex;
		int maxIterations = 100;
		int iterations = 0;
		while (true) {
			filterIndex ++;
			if (filterIndex >= filterGroup.filters.Length) {
				groupIndex = (int)Mathf.Repeat(groupIndex + 1, filterGroups.Length);
				filterIndex = 0;
			} 
			if (filterGroups[groupIndex].effectOverYear.Evaluate(SceneManager.curvePos) != 0)
				break;
			iterations ++;
			if (iterations >= maxIterations)
				return;
		}
		SetFilter(groupIndex, filterIndex);
	}
}
