using UnityEngine;
using System.Collections;
using UIWidgets;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIdataImporter : MonoBehaviour {
	[SerializeField]
	private ListView landInfoListView;
	private ListView CropInfoListView;
	private Text textShowingOnCropInfoTab;
	Dictionary<string,int> maxLevelDictionary;

//	[SerializeField]
//	Sprite landIcon;



	DataReader dataReaderScript;

	// Use this for initialization
	void Start () {
		landInfoListView = GameObject.Find ("LandListView").GetComponent<ListView> ();
		CropInfoListView = GameObject.Find ("CropListView").GetComponent<ListView> ();
		dataReaderScript = GameObject.Find ("ScriptContainer").GetComponent<DataReader> ();
		textShowingOnCropInfoTab = GameObject.FindGameObjectWithTag ("CropInfoTabText").GetComponent<Text> ();
		//maxLevelDictionary stores the max level of each kind of crop
		maxLevelDictionary=new Dictionary<string, int>();

		textShowingOnCropInfoTab.text="Welcome to the Fantacsy Farmer Simulator!";

		loadLandListToListView ();
		loadCropListToListView ();

	}



	void loadLandListToListView(){
		foreach (var f in dataReaderScript.farmLandList) {
			
			
			landInfoListView.Add ("Spot"+f.landSpot+"     cost: $"+f.costToPurchase);
			
			
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
	
	// Update is called once per frame
	void Update () {
	
//		Debug.Log ("SelectedIndex: " + CropInfoListView.SelectedIndex);
		trackCropInfoListView ();
//		var indicies = landInfoListView.SelectedIndicies;
//		Debug.Log("indicies: "+string.Join(", ", indicies.ConvertAll(x => x.ToString()).ToArray()));


	}



}
