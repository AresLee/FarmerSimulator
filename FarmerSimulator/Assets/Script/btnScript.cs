using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class btnScript : MonoBehaviour {
	GameInformation gameInformationScript;
	Text textOnStartTimeBtn;
	// Use this for initialization
	void Start () {
	
		gameInformationScript = GameObject.Find ("ScriptContainer").GetComponent<GameInformation> ();
		textOnStartTimeBtn = GameObject.FindGameObjectWithTag ("TimeStartButton").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (gameInformationScript.isTimeRunning);
	}

	public void timeStartBtnFunc(){

	

		if (gameInformationScript.isTimeRunning) {

			gameInformationScript.isTimeRunning = false;
			textOnStartTimeBtn.text="Start Time";


		} else {
			gameInformationScript.isTimeRunning=true;
			textOnStartTimeBtn.text="Pause Time";
		}

	}
}
