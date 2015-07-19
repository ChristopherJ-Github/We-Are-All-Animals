using UnityEngine;
using System.Collections;

public class SplineProperties : MonoBehaviour {

	public enum SplineType { River, FrozenRiver, Either }
	public SplineType splineType = SplineType.Either;

	public bool CanSpawn () {

		bool riverFrozen = SnowManager.instance.riverFrozen;
		if (splineType == SplineType.River && riverFrozen)
			return false;
		else if (splineType == SplineType.FrozenRiver && !riverFrozen)
			return false;
		else 
			return true;
	}
}
