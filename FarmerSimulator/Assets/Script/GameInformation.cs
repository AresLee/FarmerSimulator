using UnityEngine;
using System.Collections;

public class GameInformation : MonoBehaviour {
	float timer;
	float timeIncreasingSpeed;
	public bool isTimeRunning;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (isTimeRunning) {
			timer += timeIncreasingSpeed;
		}

	}
}
