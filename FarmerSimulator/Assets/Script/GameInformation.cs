using UnityEngine;
using System.Collections;
using UIWidgets;
using UnityEngine.UI;

public class GameInformation : MonoBehaviour {
	float timer;

	float timeIncreasingSpeed=5;
	public bool isTimeRunning;
	int daysCounter=0;
	int pastDaysCounter;
	float percentageOfProgressBar;
	Text textOnProgressBar;
	public int moneyTotal=5000;
	Text textBelowProgressBar;
	public int moneyCanEarnPerDay;

	UIdataImporter UIdataScript;

	private Progressbar timeProgressbar;

	Text moneyText;
	Text moneyCanEarnPerDayText;
	public bool isCalculatingMoneyCanEarnPerDay;
	bool isMoneyIncreasing;
	// Use this for initialization
	void Start () {
		timeProgressbar = GameObject.Find ("TimeProgressbar").GetComponent<Progressbar> ();
		timeProgressbar.Max = 100;
		textOnProgressBar = GameObject.FindGameObjectWithTag ("TextOnProgressBar").GetComponent<Text> ();
		textBelowProgressBar=GameObject.FindGameObjectWithTag ("TextbelowProgressBar").GetComponent<Text> ();
		moneyText = GameObject.FindGameObjectWithTag ("MoneyText").GetComponent<Text> ();
		UIdataScript = GameObject.Find ("ScriptContainer").GetComponent<UIdataImporter> ();
		moneyCanEarnPerDayText = GameObject.FindGameObjectWithTag ("MoneyEarnPerDayText").GetComponent<Text> ();


	//	timeProgressbar.Value = 10;
	}

	void updateMoney(){
		moneyText.text ="Total $ "+moneyTotal.ToString();
		moneyCanEarnPerDayText.text="Earn $"+moneyCanEarnPerDay+" per day";
	}

	// Update is called once per frame
	void Update () {

		updateMoney ();
		timerFunc ();
		processBarForTimeFunc ();


		calculateMoneyCanEarnPerDay ();
	//	Debug.Log ("moneyCanEarnPerDay: " + moneyCanEarnPerDay);
//		Debug.Log("timer: "+(int)timer+ " day: "+daysCounter+" percentage: "+percentageOfProgressBar);
		Debug.Log ("percentageOfProgressBar: " + percentageOfProgressBar);
	}

	void timerFunc(){

		if (isTimeRunning) {
			//1 second in the real life = 1 hour in the game by defult
			timer += timeIncreasingSpeed*Time.deltaTime;

		}
		pastDaysCounter = daysCounter;
		//when reach every 24 hours in the game world, dayCounter will be added one
		daysCounter = (int)(timer / 24);

		

		percentageOfProgressBar = (timer % 24) / 24;

		//get the money earned by the end of a day
		if (pastDaysCounter!=daysCounter&&isTimeRunning) {
			moneyTotal+=moneyCanEarnPerDay;
			isMoneyIncreasing=false;
		}
	}

	void processBarForTimeFunc(){

		timeProgressbar.Value = (int)(percentageOfProgressBar*100);
		int currentHour = (int)(timer % 24);

		textOnProgressBar.text="Time: "+currentHour+":00";
		textBelowProgressBar.text = "Day "+daysCounter;

	}

	void calculateMoneyCanEarnPerDay (){
		if (isCalculatingMoneyCanEarnPerDay) {
			moneyCanEarnPerDay=0;
			foreach (var f in UIdataScript.currentFarmLandList ) {
				if (f.cropInfoOfTheSpot!=null) {
					moneyCanEarnPerDay+=f.cropInfoOfTheSpot.cashOutputPerDay;
				}
			}
			isCalculatingMoneyCanEarnPerDay=false;
		}
	}

}

public class FarmLandUnitOnTheList{
	public Farmland farmlandInfoOfTheSpot;
	public Crop cropInfoOfTheSpot;
	public bool isTheSpotEmpty;
	public bool isTheSpotPurchased;
	public int indexOfTheLandOnTheList;
	int maxLevelOfTheCrop;
	int currentLevelOfTheCrop;
	int theFarmLandCost;

	public string landStatus;


	public FarmLandUnitOnTheList(bool _isTheSpotPurchased,int _indexOfTheLandOnTheList,Farmland _farmlandInfoOfTheSpot){
		isTheSpotPurchased = _isTheSpotPurchased;
		indexOfTheLandOnTheList = _indexOfTheLandOnTheList;

		farmlandInfoOfTheSpot = _farmlandInfoOfTheSpot;
		if (!isTheSpotPurchased) {
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
