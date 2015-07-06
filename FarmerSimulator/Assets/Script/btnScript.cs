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



	//	check if the land is purchased and the button is on the state of "purchase"
		if ((!(UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].isTheSpotPurchased))&&textOnBuyLandBtn.text=="Purchase") {
			UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].isTheSpotPurchased=true;


			//load the next farmland that is avalable to purchase
			UIDataScript.loadTheNextLandOfListView(UIDataScript.currentFarmLandList[UIDataScript.currentFarmLandList.Count-1]);
		
		}


		UIDataScript.isLandInfoListViewUpdating = true;
		UIDataScript.isAvilableComboBoxUpdating = true;

		//UIDataScript.landInfoListView.Add("test");

		//need to find a way to walk around the limited listview items issue. might initialized all the farmland first and change the strings
	}

	public void buyCropBtn(){


	}
}


