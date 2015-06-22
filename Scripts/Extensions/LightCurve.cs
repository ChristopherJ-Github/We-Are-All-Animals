using UnityEngine;
using System.Collections;
//Light FX Script for Unity3D - using curves and a gradient your light will change over time
//  eg: Use a quick declining curve, and gradient from bright to dark for an explosion
//  eg: Make nice random curves for a torch flicker
 
// Quentin Preik 2013/10/22
// @fivearchers
// modified heavily by Benjamin Langerak, 2014/06/15
// @jammingames
 
public class LightCurve : MonoBehaviour
{
		public AnimationCurve intensityCurve;   //Used for intensity over time - cover 0-1 on both axis
		public float intensityMin;                              //Base intensity       
		public float intensityMax;                              //Max intensity - so using, 2 and 10 - at the bottom of a curve intensity
		//would be 2 and at the top it would be 10
       
		public Gradient colorCurve;                             //Gradient to color the light over time
		public AnimationCurve rangeCurve;               //Curve to change the range of the light over time
		public float rangeMin;                                  //Minimum range
		public float rangeMax;                                  //Maximum range
		public float curveSpeed;                                //How fast to go through the curves and gradient - 1=1s, 2=0.5s, 0.5=2s
		public float curvePosition;                             //Where to start on the curve 0-1, 0.5 would start halfway
		public bool realTime;
		public bool loop;                                               //Whether to loop through the curve
		public bool dieWhenDone;                                //If you're not looping should the object destroy itself when it's done
		private Light myLight;    
		
        
		void OnEnable ()
		{
				GUIManager.instance.OnGuiSliderEvent += ChangeCurvePosition;
		}

		void OnDisable ()
		{
				if (GUIManager.instance != null)
						GUIManager.instance.OnGuiSliderEvent -= ChangeCurvePosition;
		}


		void Start ()
		{

				myLight = gameObject.light;

		}
       
		public void ChangeCurvePosition (float from, float to, float value)
		{
				curvePosition = Mathf.InverseLerp (from, to, value);
		}

		void Update ()
		{
			
				if (realTime)
					//	curvePosition = Mathf.InverseLerp (0, 1080, GUIManager.instance.hSliderValue);
				if (curvePosition != 1f || !loop) {
						//curvePosition += curveSpeed * Time.deltaTime;
						//0.015
						if (!realTime)
								curvePosition += 1 / curveSpeed * Time.deltaTime;
					
						if (curvePosition > 1f) {
								if (dieWhenDone) {
										this.enabled = false;
										//Destroy (gameObject);
										return;
								}
								if (!loop) {
										curvePosition = 1f;      
								} else {
										curvePosition = 1f - curvePosition;
								}
						}
						light.intensity = intensityMin + (intensityMax - intensityMin) * intensityCurve.Evaluate (curvePosition);
						light.color = colorCurve.Evaluate (curvePosition);
						light.range = rangeMin + (rangeMax - rangeMin) * rangeCurve.Evaluate (curvePosition);
				}
		}
}