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
		
		int groupIndex = GetGroupIndex ();
		FilterGroup filterGroup = filterGroups [groupIndex];
		int filterIndex = GetFilterIndex (filterGroup);
		bool allowMainFilterCopying = Random.value >= 0.5f;
		SetFilter (groupIndex, filterIndex, allowMainFilterCopying);
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
	void SetFilter (int groupIndex, int filterIndex, bool allowMainFilterCopying) {
		
		this.groupIndex = groupIndex;
		filterGroup = filterGroups [groupIndex];
		this.filterIndex = filterIndex;
		if (filterIndex != -1)
			filter = filterGroup.filters[filterIndex];
		else
			filter = new FilterInfo ();
		amplifyColorEffect.LutTexture = filter.LutTexture;
		currentLut = filter.LutTexture;
		blend = 0;
	}
	
	void Update () {

		#if UNITY_EDITOR
			UpdateBlend ();
		#endif
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
	void UpdateFilter () {

		float newBlend = _blend;
		if (WeatherControl.currentWeather != null) {
			float weatherEffect = WeatherControl.instance.totalTransition * WeatherControl.instance.severity;
			float currentEffect = filterGroup.effectOverYear.Evaluate(SceneManager.curvePos);
			newBlend = Mathf.Lerp (1, newBlend, weatherEffect * currentEffect);
			newBlend = WeatherControl.currentWeather.usesFilter ? newBlend : 1;
		} else {
			newBlend = 1;
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
		SetFilter(groupIndex, filterIndex, false);
	}
}
