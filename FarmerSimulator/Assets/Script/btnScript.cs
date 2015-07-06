using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class btnScript : MonoBehaviour {
	GameInformation gameInformationScript;
	Text textOnStartTimeBtn;
	UIdataImporter UIDataScript;
	Text textOnBuyLandBtn;
	// Use this for initialization
	void Start () {
	
		gameInformationScript = GameObject.Find ("ScriptContainer").GetComponent<GameInformation> ();
		textOnStartTimeBtn = GameObject.FindGameObjectWithTag ("TimeStartButton").GetComponent<Text> ();
		UIDataScript = GameObject.Find ("ScriptContainer").GetComponent<UIdataImporter>();
		textOnBuyLandBtn = GameObject.FindGameObjectWithTag ("TextOnBuyLandBtn").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	//	Debug.Log (gameInformationScript.isTimeRunning);
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

	public void buyLandBtn(){

	//	UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].isTheSpotPurchased;

		//check if the land is purchased and the button is on the state of "purchase"
		if ((!(UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].isTheSpotPurchased))&&textOnBuyLandBtn.text=="Purchase") {
			UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].isTheSpotPurchased=true;

		}

		//UIDataScript.landInfoListView.SelectedIndex
	}
}


