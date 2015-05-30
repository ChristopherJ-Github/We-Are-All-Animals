using UnityEngine;
using System.Collections;

public class LeafFaller : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tree treeObject = gameObject.GetComponent<Tree> ();



		foreach ( Transform treeNode in treeObject.transform) {
			Debug.Log (treeNode);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
