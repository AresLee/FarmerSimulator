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
	Button buyLandButton;


	// Use this for initialization
	void Start () {
	
		gameInformationScript = GameObject.Find ("ScriptContainer").GetComponent<GameInformation> ();
		textOnStartTimeBtn = GameObject.FindGameObjectWithTag ("TimeStartButton").GetComponent<Text> ();
		UIDataScript = GameObject.Find ("ScriptContainer").GetComponent<UIdataImporter>();
		textOnBuyLandBtn = GameObject.FindGameObjectWithTag ("TextOnBuyLandBtn").GetComponent<Text> ();
		dataReaderScript = GameObject.Find ("ScriptContainer").GetComponent<DataReader> ();
		inputFiledAtComboBox = GameObject.FindGameObjectWithTag ("inputFiledAtComboBox").GetComponent<InputField> ();
		buyLandButton = GameObject.Find ("BuyLandBtn").GetComponent<Button> ();

	}
	
	// Update is called once per frame
	void Update () {

		//the buyLandButton will be disappear if nothing is selected on the landInfoListView
		if (UIDataScript.landInfoListView.SelectedIndex == -1) {
			buyLandButton.gameObject.SetActive (false);
		} else {
			buyLandButton.gameObject.SetActive (true);
		}

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


			//make sure user has enough money to purhcase
			if (gameInformationScript.moneyTotal>=UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].farmlandInfoOfTheSpot.costToPurchase) {
				UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].isTheSpotPurchased=true;
				//load the next farmland that is avalable to purchase
				UIDataScript.loadTheNextLandOfListView(UIDataScript.currentFarmLandList[UIDataScript.currentFarmLandList.Count-1]);
				//cost money
				gameInformationScript.moneyTotal-=UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].farmlandInfoOfTheSpot.costToPurchase;
			}else{
				UIDataScript.showNotification("Sorry, you don't have enough money to buy this. See you next time!");

			}

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

		if (textOnBuyLandBtn.text=="Upgrade") {
			// to do...

			var selectedCrop=UIDataScript.currentFarmLandList[UIDataScript.landInfoListView.SelectedIndex].cropInfoOfTheSpot;

			int maxLevelOfSelectedCrop=UIDataScript.maxLevelDictionary[selectedCrop.cropName];
			if (selectedCrop.level<maxLevelOfSelectedCrop) {
				int counter=0;
				foreach (Crop c in dataReaderScript.cropList) {

					if (c.cropName==selectedCrop.cropName&&c.level==selectedCrop.level) {
						UIDataScript.currentFarmLandList[UIDataScript.landInfoListView.SelectedIndex].cropInfoOfTheSpot=dataReaderScript.cropList[counter+1];

						UIDataScript.currentFarmLandList[UIDataScript.landInfoListView.SelectedIndex].landStatus = selectedCrop.cropName + "(L." + dataReaderScript.cropList[counter+1].level + ") is growing with $" + dataReaderScript.cropList[counter+1].cashOutputPerDay + " output per day.";
						gameInformationScript.isCalculatingMoneyCanEarnPerDay=true;
					}
					counter+=1;
				}
			}
			else{

				UIDataScript.showNotification("Sorry, Level."+selectedCrop.level+" is the max level for "+selectedCrop.cropName);
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

			//Only level 1 crop can be planted to an empty farmland
			if (!(selectedCrop.level>1)) {

				if (selectedCrop.costToPlant<=gameInformationScript.moneyTotal) {

					gameInformationScript.moneyTotal -= selectedCrop.costToPlant;
					
					string stringOfTargetFarmlandSpot = UIDataScript.avilableLandComboBox.ListView.Strings [UIDataScript.avilableLandComboBox.ListView.SelectedIndex];
					
					var targetFarmlandSpotNumber = Regex.Match (stringOfTargetFarmlandSpot, @"\d+$").Value;
					
					//add the crop selected to the target spot
					UIDataScript.currentFarmLandList [(int.Parse (targetFarmlandSpotNumber) - 1)].cropInfoOfTheSpot = selectedCrop;
					UIDataScript.currentFarmLandList [(int.Parse (targetFarmlandSpotNumber) - 1)].isTheSpotEmpty = false;
					UIDataScript.currentFarmLandList [(int.Parse (targetFarmlandSpotNumber) - 1)].landStatus = selectedCrop.cropName + "(L." + selectedCrop.level + ") is growing with $" + selectedCrop.cashOutputPerDay + " output per day.";
					
					//	UIDataScript.avilableLandComboBox.ListView.Items.Remove(stringOfTargetFarmlandSpot);
					inputFiledAtComboBox.text = "";
					
					//refreshing the LandInfolistView and AvilableComboBox
					UIDataScript.isLandInfoListViewUpdating = true;
					UIDataScript.isAvilableComboBoxUpdating = true;
					//refresing moneyCanEarnPerDay
					gameInformationScript.isCalculatingMoneyCanEarnPerDay=true;
				}

				else{
					UIDataScript.showNotification("Sorry, you don't have enough money to buy this. See you next time!");
				}
			}else{
				UIDataScript.showNotification("Only level 1 crop can be planted to an empty farmland!");

			}


		
		} else {

			UIDataScript.showNotification("Please select an available empty farmland from the combo box!");
		}




	

	}

	public void sellBtnFunc(){

		gameInformationScript.moneyTotal += UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].cropInfoOfTheSpot.costToPlant;
		gameInformationScript.moneyCanEarnPerDay -= UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].cropInfoOfTheSpot.cashOutputPerDay;

		UIDataScript.currentFarmLandList[UIDataScript.landInfoListView.SelectedIndex].cropInfoOfTheSpot=null;
		UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].isTheSpotEmpty = true;
		UIDataScript.currentFarmLandList [UIDataScript.landInfoListView.SelectedIndex].landStatus = "<Empty>";
		gameInformationScript.isCalculatingMoneyCanEarnPerDay = true;
		//refreshing the LandInfolistView and AvilableComboBox
		UIDataScript.isLandInfoListViewUpdating = true;
		UIDataScript.isAvilableComboBoxUpdating = true;
	}
}


