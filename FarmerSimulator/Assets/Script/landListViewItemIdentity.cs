using UnityEngine;
using System.Collections;

public class landListViewItemIdentity : MonoBehaviour {
	public static int FarmLandCount;
	// Use this for initialization
	void Start () {

		gameObject.name="farmLand"+FarmLandCount+"Text";
		FarmLandCount += 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
