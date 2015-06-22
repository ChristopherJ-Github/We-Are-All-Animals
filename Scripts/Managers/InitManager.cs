using UnityEngine;
using System.Collections;

public class InitManager : MonoBehaviour
{
		//this class is used to properly instantiate and assign member references 
		//to all singletons via a call to the instance property
		
		public GameObject mainScene;
		public GUISkin gSkin;
		
		void Awake ()
		{
				SceneManager.mainScene = mainScene;
				GUIManager.instance.gSkin = gSkin;
		}

}
