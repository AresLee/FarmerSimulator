using UnityEngine;
using System.Collections;
using UIWidgets;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIdataImporter : MonoBehaviour {
	[SerializeField]
	public ListView landInfoListView;
	public ListView CropInfoListView;
	private Text textShowingOnCropInfoTab;
	Dictionary<string,int> maxLevelDictionary;

	public List<FarmLandUnitOnTheList> currentFarmLandList;
	int maxLandQuantity;
	Text textOnBuyLandBtn;

	public Combobox avilableLandComboBox;
	List<string> comboBoxStringList;
	List<string> farmLandInfoStringList;

	public bool isLandInfoListViewUpdating;
	public bool isAvilableComboBoxUpdating;
//	[SerializeField]
//	Sprite landIcon;



	DataReader dataReaderScript;

	// Use this for initialization
	void Start () {
		landInfoListView = GameObject.Find ("LandListView").GetComponent<ListView> (); 
		//landInfoListView = GameObject.Find ("ListView").GetComponent<ListView> (); 
		CropInfoListView = GameObject.Find ("CropListView").GetComponent<ListView> ();
		dataReaderScript = GameObject.Find ("ScriptContainer").GetComponent<DataReader> ();
		textShowingOnCropInfoTab = GameObject.FindGameObjectWithTag ("CropInfoTabText").GetComponent<Text> ();
		textOnBuyLandBtn = GameObject.FindGameObjectWithTag ("TextOnBuyLandBtn").GetComponent<Text> ();
		//maxLevelDictionary stores the max level of each kind of crop
		maxLevelDictionary=new Dictionary<string, int>();
		//the current farmland information list
		currentFarmLandList = new List<FarmLandUnitOnTheList> ();
		maxLandQuantity = dataReaderScript.farmLandList.Count;//start from 1
		avilableLandComboBox = GameObject.Find ("Combobox").GetComponent<Combobox> ();
		comboBoxStringList = new List<string> ();
		farmLandInfoStringList = new List<string> ();


		textShowingOnCropInfoTab.text="Welcome to the Fantacsy Farmer Simulator!";


		loadInitialLandToListView ();
			//loadAllLandListToListView ();
		loadCropListToListView ();
		loadInitialComboBox ();
	

	}
	void loadInitialComboBox(){
		//add the initial spot to the avilableLandComboBox
		avilableLandComboBox.ListView.Add("Spot1");
	}

	void loadInitialLandToListView(){
		//add the 1st farmlandInfo from the whole farmlandInfo to the currentFarmlandList
		currentFarmLandList.Add(new FarmLandUnitOnTheList(true,0,dataReaderScript.farmLandList[0]));

		int initialFarmlandSpotNumber = currentFarmLandList [0].farmlandInfoOfTheSpot.landSpot;
		string initialFarmLandStatus = currentFarmLandList [0].landStatus;
		//add the first farmland spot info to the 1st item on the listview
		landInfoListView.Add ("Spot" + initialFarmlandSpotNumber +"   "+initialFarmLandStatus);
		//load the next spot locked
		loadTheNextLandOfListView (currentFarmLandList[0]);
	}

	//next farmLandSpot is the spot that can be unlocked 
	public void loadTheNextLandOfListView(FarmLandUnitOnTheList _previousFarmLandUnlocked){
		//add the next landspot info to the currentFarmLandList 
		currentFarmLandList.Add(new FarmLandUnitOnTheList(false,_previousFarmLandUnlocked.indexOfTheLandOnTheList+1,dataReaderScript.farmLandList[_previousFarmLandUnlocked.indexOfTheLandOnTheList+1]));
		int nextFarmlandSpotNumber = currentFarmLandList [_previousFarmLandUnlocked.indexOfTheLandOnTheList+1].farmlandInfoOfTheSpot.landSpot;
		string nextFarmLandStatus = currentFarmLandList [_previousFarmLandUnlocked.indexOfTheLandOnTheList+1].landStatus;
		//add this to be the next item on the listview below the one that has been unlocked
		landInfoListView.Add ("Spot" + nextFarmlandSpotNumber +"   "+nextFarmLandStatus);


	}

	void loadAllLandListToListView(){
		foreach (var f in dataReaderScript.farmLandList) {
			
			
			landInfoListView.Add ("Spot"+f.landSpot+"     cost: $"+f.costToPurchase);
			
			
		}
	
	}

	void loadPossibleLandListToLandListView(){
	
	}

	void trackLandInfoListView(){


		//for tracking selecting item of landInfoListView and update the function of the BuyLandBtn
		if (landInfoListView.SelectedIndex != -1) {
			var selectedFarmItem = currentFarmLandList [landInfoListView.SelectedIndex];

			//selected item not purchased
			if (!selectedFarmItem.isTheSpotPurchased) {
				textOnBuyLandBtn.text = "Purchase";

			//selected item purchased
			} else {
				//check if the land purchased has any crop and update the function of the BuyLandBtn
				if (selectedFarmItem.cropInfoOfTheSpot != null) {
					textOnBuyLandBtn.text = "Upgrade";



					//add specific output for specific crop
				} else {
					//the land will be empty once purchasing because there is no crop on it initially
					currentFarmLandList [landInfoListView.SelectedIndex].isTheSpotEmpty = true;
					currentFarmLandList [landInfoListView.SelectedIndex].landStatus="<Empty>";
					textOnBuyLandBtn.text = "Select";


				}

			
				updateFarmlandListView();
			}


			//for keep the items on the landList up-to-date
//			foreach (FarmLandUnitOnTheList c in currentFarmLandList) {
//		
//				Text itemString=GameObject.Find("farmLand"+c.indexOfTheLandOnTheList+"Text").GetComponent<Text>();
//
//				itemString.text="Spot" + c.farmlandInfoOfTheSpot.landSpot + "   " + c.landStatus;
//			
//			}
			//updateFarmlandListView();



		}
	}

	public void updateFarmlandListView(){


		if (isLandInfoListViewUpdating) {

		
			farmLandInfoStringList.Clear ();
			foreach (FarmLandUnitOnTheList f in currentFarmLandList) {
				farmLandInfoStringList.Add("Spot" + f.farmlandInfoOfTheSpot.landSpot + "   " + f.landStatus);
			}


			landInfoListView.Strings = farmLandInfoStringList;

			isLandInfoListViewUpdating = false;
		}


	}


	//this function load crop info to the listview and store the max value of each kind of crop
	void loadCropListToListView(){


		foreach (var c in dataReaderScript.cropList) {


			if (!maxLevelDictionary.ContainsKey(c.cropName)) {

				maxLevelDictionary.Add(c.cropName,c.level);
			}else{
				if (c.level>maxLevelDictionary[c.cropName]) {
					maxLevelDictionary[c.cropName]=c.level;
				}

			}

		
				CropInfoListView.Add (c.cropName+"    (L."+c.level+")");
		


		}

	}

	void trackCropInfoListView(){
		//for tracking selecting item of cropInfoListView and show the info on the tab
		if (CropInfoListView.SelectedIndex != -1) {
			var selectedCropItem = dataReaderScript.cropList [CropInfoListView.SelectedIndex];
			//var nextLevelOfSelectedCropItem;
				int IndexOfNextLevelOfSelectedCropItem;

				if(CropInfoListView.SelectedIndex+1<dataReaderScript.cropList.Count)
					 IndexOfNextLevelOfSelectedCropItem=CropInfoListView.SelectedIndex+1;
				else{
					IndexOfNextLevelOfSelectedCropItem=CropInfoListView.SelectedIndex;
				}

				var nextLevelOfSelectedCropItem=dataReaderScript.cropList [IndexOfNextLevelOfSelectedCropItem];
				string nextLevelTextPart;
			if (selectedCropItem.level!=maxLevelDictionary[selectedCropItem.cropName]) {
				 nextLevelTextPart = "\n\n\n\n\nNextLevel:"
					+"\nLevel: "+nextLevelOfSelectedCropItem.level
					+"\nCost To Plant: $"+nextLevelOfSelectedCropItem.costToPlant
					+"\nCash Output Per Day: $" + nextLevelOfSelectedCropItem.cashOutputPerDay;
			}else{

				nextLevelTextPart="\n\n\n\n\nThis is the maximum level of this crop.";
			}

				textShowingOnCropInfoTab.text = "Crop Name: " + selectedCropItem.cropName
					+ "\nLevel: " + selectedCropItem.level
						+ "\nCost To Plant: $" + selectedCropItem.costToPlant
						+ "\nCash Output Per Day: $" + selectedCropItem.cashOutputPerDay
						+nextLevelTextPart;




		
		}
	
	}

	void updateAvilableComboBoxList(){
	
		if (isAvilableComboBoxUpdating) {
			comboBoxStringList.Clear();
			foreach (FarmLandUnitOnTheList f in currentFarmLandList) {

				if (f.isTheSpotEmpty) {
					comboBoxStringList.Add("Spot" + f.farmlandInfoOfTheSpot.landSpot);
				

				}
			}


			avilableLandComboBox.ListView.Strings=comboBoxStringList;
			isAvilableComboBoxUpdating=false;
		}


	}


	
	// Update is called once per frame
	void Update () {
		Debug.Log ("isLandInfoListViewUpdating: " + isLandInfoListViewUpdating);
		Debug.Log ("isComboBoxUpdating: " + isAvilableComboBoxUpdating);



//		Debug.Log ("SelectedIndex: " + CropInfoListView.SelectedIndex);
		trackCropInfoListView ();
		trackLandInfoListView ();
		updateAvilableComboBoxList ();




	}



}
