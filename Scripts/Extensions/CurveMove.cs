using UnityEngine;
using System.Collections;
//script to apply movement over time based on a curve
//Quentin Preik - 2012/10/22
//@fivearchers
public class CurveMove : MonoBehaviour
{
		public AnimationCurve moveCurve;        //Curve to use - use 1 second long, usually 1 unity high
		public Vector3 movement;                        //Amount of movement applied by this curve - so 5,25,0, would
		//move the object 5 right, 25 up when the curve is at 1
		public Vector3 randomization;
		public float moveRate;                          //How fast to go thru the curve:  1=1s, 2=0.5s, 0.5=2s etc
		public float curvePos;                          //Use this as an offset - so 0.5 would start halfway thru the curve
		Vector3 localstart;
		public bool loop = true;                          //Whether to loop or only run once
		public bool randomize = false;

		void Start ()
		{
				localstart = transform.localPosition;
		}
	
	
		void Update ()
		{
				if (curvePos != 1 || loop) {
						curvePos += (1 / moveRate) * Time.deltaTime;
						if (curvePos > 1f && loop) {
								curvePos = curvePos - 1f;
						} else if (curvePos > 1f && !loop) {
								curvePos = 1f;
						}
						if (randomize) {

								randomization = Random.insideUnitSphere;
								movement += randomization;
						}
						transform.localPosition = localstart + movement * moveCurve.Evaluate (curvePos);
				}
		}
}