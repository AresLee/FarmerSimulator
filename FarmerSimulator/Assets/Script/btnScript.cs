using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class btnScript : MonoBehaviour {
	GameInformation gameInformationScript;
	Text textOnStartTimeBtn;
	UIdataImporter UIDataScript;
	Text textOnBuyLandBtn;
	DataReader dataReaderScript;
	InputField inputFiledAtComboBox;

	// Use this for initialization
	void Start () {
	
		gameInformationScript = GameObject.Find ("ScriptContainer").GetComponent<GameInformation> ();
		textOnStartTimeBtn = GameObject.FindGameObjectWithTag ("TimeStartButton").GetComponent<Text> ();
		UIDataScript = GameObject.Find ("ScriptContainer").GetComponent<UIdataImporter>();
		textOnBuyLandBtn = GameObject.FindGameObjectWithTag ("TextOnBuyLandBtn").GetComponent<Text> ();
		dataReaderScript = GameObject.Find ("ScriptContainer").GetComponent<DataReader> ();
		inputFiledAtComboBox = GameObject.FindGameObjectWithTag ("inputFiledAtComboBox").GetComponent<InputField> ();
			
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

		if (textOnBuyLandBtn.text=="Select") {


			//return the comboBox item that has the same string name as the selected item from LandInfoListView
			int counter=0;
			foreach (string s in UIDataScript.avilableLandComboBox.ListView.Strings) {
				if (s=="Spot"+(UIDataScript.landInfoListView.SelectedIndex+1)) {
					UIDataScript.avilableLandComboBox.ListView.SelectedIndex=counter;
				}
				counter+=1;

			}


		}



		//refreshing the LandInfolistView and AvilableComboBox
		UIDataScript.isLandInfoListViewUpdating = true;
		UIDataScript.isAvilableComboBoxUpdating = true;

	
	}

	public void buyCropBtn(){

		if (UIDataScript.avilableLandComboBox.ListView.SelectedIndex != -1) {
			//get the select crop from the cropInfoListview
			var selectedCrop = dataReaderScript.cropList [UIDataScript.CropInfoListView.SelectedIndex];

			gameInformationScript.moneyTotal -= selectedCrop.costToPlant;



			//		string testString="Spot6";
			//
			//		var result = Regex.Match (testString, @"\d+$").Value;
			//		Debug.Log ("number: " + result);

			string stringOfTargetFarmlandSpot=UIDataScript.avilableLandComboBox.ListView.Strings[UIDataScript.avilableLandComboBox.ListView.SelectedIndex];

			var targetFarmlandSpotNumber=Regex.Match(stringOfTargetFarmlandSpot, @"\d+$").Value;

			//add the crop selected to the target spot
			UIDataScript.currentFarmLandList[(int.Parse(targetFarmlandSpotNumber)-1)].cropInfoOfTheSpot=selectedCrop;
			UIDataScript.currentFarmLandList[(int.Parse(targetFarmlandSpotNumber)-1)].isTheSpotEmpty=false;
			UIDataScript.currentFarmLandList[(int.Parse(targetFarmlandSpotNumber)-1)].landStatus=selectedCrop.cropName+"(L."+selectedCrop.level+") is growing with $"+selectedCrop.cashOutputPerDay+" output per day.";

		//	UIDataScript.avilableLandComboBox.ListView.Items.Remove(stringOfTargetFarmlandSpot);
			inputFiledAtComboBox.text="";
		
		}

		//refreshing the LandInfolistView and AvilableComboBox
		UIDataScript.isLandInfoListViewUpdating = true;
		UIDataScript.isAvilableComboBoxUpdating = true;

	

	}
}


