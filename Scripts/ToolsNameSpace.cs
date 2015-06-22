using UnityEngine;
using System.Collections;

namespace Tools {

	public class Math {

		/// <summary>
		/// Used instead of the usual InverseLerp and Lerp combo to allow for unclamped conversion. 
		/// Entering -1 in an original range from 0 to 1 for example will return a value outside of the new range
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="oldMin">Old minimum.</param>
		/// <param name="oldMax">Old max.</param>
		/// <param name="newMin">New minimum.</param>
		/// <param name="newMax">New max.</param>
		public static float Convert (float value, float oldMin, float oldMax, float newMin, float newMax) {
			
			float oldRange = oldMax - oldMin;
			float newRange = newMax - newMin;
			float newValue = (((value - oldMin) * newRange) / oldRange) + newMin;
			return newValue;
		}
	}
}