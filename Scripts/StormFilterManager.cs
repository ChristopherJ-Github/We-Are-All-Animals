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
		SetFilter(groupIndex, filterIndex, false);
	}
}
