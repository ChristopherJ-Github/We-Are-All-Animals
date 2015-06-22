using UnityEngine;
using System.Collections;
 
//use an animation curve to rotate an object on its Y axis
//someday will mod to include an axis selection
 
public class CurveRotate : MonoBehaviour
{
		public AnimationCurve rotateCurve;      //curve to use for the angle (usually keeping the range 0-1 on both axis)
		public float offset;                            //offset angle (start angle)
		public float rotateAmount;                      //degrees to rotate
		public float rotateRate;                        //speed to go thru the rotation 1=1s, 0.5=2s, 2=0.5s
		public float curvePos;                          //use if you want to start at a different point on the curve 0-1, so 0.5 is halfway
		public bool loop = true;                          //should we keep looping?
		Vector3 eul;                                            //this stores your rotation: Note right now it would make your angles x & z=0
		public enum Axis
		{
				x,
				y,
				z
		}
		public Axis axis;
		void Start ()
		{
				eul = Vector3.zero;
		}
       
 
		void Update ()
		{
				if (curvePos != 1 || loop) {
						curvePos += (1 / rotateRate) * Time.deltaTime;
						if (curvePos > 1f && loop) {
								curvePos = curvePos - 1f;
						} else if (curvePos > 1f && !loop) {
								curvePos = 1f;
						}
						if (axis == Axis.x)
								eul.x = offset + rotateAmount * rotateCurve.Evaluate (curvePos);
						else if (axis == Axis.y)
								eul.y = offset + rotateAmount * rotateCurve.Evaluate (curvePos);
						else if (axis == Axis.z)
								eul.z = offset + rotateAmount * rotateCurve.Evaluate (curvePos);
						transform.localEulerAngles = eul;
				}
		}
}