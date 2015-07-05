using UnityEngine;
using System.Collections;
using UIWidgets;
using UnityEngine.UI;

public class UIdataImporter : MonoBehaviour {
	[SerializeField]
	ListView landInfoListView;
	ListView CropInfoListView;

//	[SerializeField]
//	Sprite landIcon;



	DataReader dataReaderScript;

	// Use this for initialization
	void Start () {
		landInfoListView = GameObject.Find ("LandListView").GetComponent<ListView> ();
		CropInfoListView = GameObject.Find ("CropListView").GetComponent<ListView> ();
		dataReaderScript = GameObject.Find ("ScriptContainer").GetComponent<DataReader> ();

		loadLandListToListView ();
		loadCropListToListView ();
	}



	void loadLandListToListView(){
		foreach (var f in dataReaderScript.farmLandList) {
			
			
			landInfoListView.Add ("Spot"+f.landSpot+"     cost: $"+f.costToPurchase);
			
			
		}
	
	}

	void loadCropListToListView(){
		foreach (var c in dataReaderScript.cropList) {
			
			
			CropInfoListView.Add (c.cropName+"    (L."+c.level+")");
			
			
		}

	}
	
	// Update is called once per frame
	void Update () {
	
		Debug.Log ("lastSelectedIndex: " + landInfoListView.SelectedIndex);

//		var indicies = landInfoListView.SelectedIndicies;
//		Debug.Log("indicies: "+string.Join(", ", indicies.ConvertAll(x => x.ToString()).ToArray()));


	}
}
