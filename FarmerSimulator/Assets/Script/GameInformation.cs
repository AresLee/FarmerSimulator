using UnityEngine;
using System.Collections;
using UIWidgets;
using UnityEngine.UI;

public class GameInformation : MonoBehaviour {
	float timer;
	float timeIncreasingSpeed=5;
	public bool isTimeRunning;
	int daysCounter=0;
	float percentageOfProgressBar;
	Text textOnProgressBar;
	Text textBelowProgressBar;


	private Progressbar timeProgressbar;

	// Use this for initialization
	void Start () {
		timeProgressbar = GameObject.Find ("TimeProgressbar").GetComponent<Progressbar> ();
		timeProgressbar.Max = 100;
		textOnProgressBar = GameObject.FindGameObjectWithTag ("TextOnProgressBar").GetComponent<Text> ();
		textBelowProgressBar=GameObject.FindGameObjectWithTag ("TextbelowProgressBar").GetComponent<Text> ();
	//	timeProgressbar.Value = 10;
	}
	
	// Update is called once per frame
	void Update () {
		timerFunc ();
		processBarForTimeFunc ();
		Debug.Log("timer: "+(int)timer+ " day: "+daysCounter+" percentage: "+percentageOfProgressBar);
	}

	void timerFunc(){
		if (isTimeRunning) {
			//1 second in the real life = 1 hour in the game by defult
			timer += timeIncreasingSpeed*Time.deltaTime;
		}
		//when reach every 24 hours in the game world, dayCounter will be added one
		daysCounter = (int)(timer / 24);

		percentageOfProgressBar = (timer % 24) / 24;
	}

	void processBarForTimeFunc(){

		timeProgressbar.Value = (int)(percentageOfProgressBar*100);
		int currentHour = (int)(timer % 24);

		textOnProgressBar.text="Time: "+currentHour+":00";
		textBelowProgressBar.text = "Day "+daysCounter;

	}

}

public class FarmLandUnitOnTheList{
	public Farmland farmlandInfoOfTheSpot;
	public Crop cropInfoOfTheSpot;
	bool isTheSpotEmpty;
	bool isTheSpotPurchased;
	int indexOfTheLandOnTheList;
	int maxLevelOfTheCrop;
	int currentLevelOfTheCrop;
	int theFarmLandCost;

	public string landStatus;


	public FarmLandUnitOnTheList(bool _isTheSpotPurchased,int _indexOfTheLandOnTheList,Farmland _farmlandInfoOfTheSpot){


		farmlandInfoOfTheSpot = _farmlandInfoOfTheSpot;
		if (!_isTheSpotPurchased) {
			isTheSpotEmpty=false;

		} else {
			isTheSpotEmpty=true;

		}

		if (isTheSpotEmpty) {
			landStatus="<Empty>";
		}
		else{
			landStatus= "Cost $"+_farmlandInfoOfTheSpot.costToPurchase+" to unlock";

		}

		cropInfoOfTheSpot = null;
		currentLevelOfTheCrop = maxLevelOfTheCrop = 0;

		theFarmLandCost = _farmlandInfoOfTheSpot.costToPurchase;

	}



}
