using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;  
using System;
using System.Collections.Generic;
using UnityEngine.UI;


public class DataReader : MonoBehaviour {
	private string fileNameOfFarmLandTxt;
	private string fileNameOfCropInfoTxt;
	List<int> numberList;

	List<Farmland> farmLandList;
	List<Crop> cropList;

	// Use this for initialization
	void Start () {
		numberList = new List<int> ();
		farmLandList = new List<Farmland> ();
		cropList = new List<Crop> ();



		//the filename for defult information text file stored in Assets folder
		fileNameOfFarmLandTxt="FarmlandInformation.txt";
		fileNameOfCropInfoTxt="CropInfomation.txt";

		//check if the persistent folder has have the information text files
		//if yes, load data from the file directally, if not, create one from the defult/initial data
		checkExisting (fileNameOfFarmLandTxt);
		checkExisting (fileNameOfCropInfoTxt);

		//load data from the persistent folder, 
		//the text files can be modified any time and loaded next time when you run the program
		LoadData (fileNameOfFarmLandTxt);
		LoadData (fileNameOfCropInfoTxt);

		GameObject.Find ("Text").GetComponent<Text> ().text = "count Farm: " + farmLandList.Count + "\ncount Crop: " + cropList.Count;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private bool checkExisting(string fileName){
		List<string> exsitingFileFromDataFolder=new List<string>();
		string[] fileListFromDataFolder = Directory.GetFiles (Application.persistentDataPath);
		foreach (string f in fileListFromDataFolder) {
			exsitingFileFromDataFolder.Add(f);
		}
		
		string filePath=Application.persistentDataPath+"/"+fileName;
		//if the presistent folder doesnt have the information text file,
		//the program will generate the files based on the defult/initial data given.
		if (!exsitingFileFromDataFolder.Contains (filePath)) {

			string fileString;
			StreamReader theReader=new StreamReader("Assets/"+fileName,Encoding.Default);

			fileString=theReader.ReadToEnd();

			System.IO.File.WriteAllText(filePath,fileString);
			theReader.Close();
			return false;
		} else {
			return true;
		}
		
	}

	private bool LoadData(string fileName)
	{
		fileName = Application.persistentDataPath+"/" + fileName;
		Debug.Log (fileName);
		// Handle any problems that might arise when reading the text
		try
		{
			string line;
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader(fileName, Encoding.Default);
			
			// Immediately clean up the reader after this block of code is done.
			// You generally use the "using" statement for potentially memory-intensive objects
			// instead of relying on garbage collection.
			// (Do not confuse this with the using directive for namespace at the 
			// beginning of a class!)
			using (theReader)
			{
				// While there's lines left in the text file, do this:

					line = theReader.ReadToEnd();


					if (line != null)
					{
						// Do whatever you need to do with the text line, it's a string now
						// In this example, I split it into arguments based on comma
						// deliniators, then send that array to DoStuff()
						string[] entries = line.Split(new char[] {'\n'},StringSplitOptions.RemoveEmptyEntries);


						if (entries.Length > 0)

							
							//assume Farmland information just contain two formats: LAND_ID and COST_TO_PURCHASE
							foreach (string e in entries) {
								//# in the txt is ignored
								if(e!=""&&!e.Contains("#")){
									
								
								if (fileName==Application.persistentDataPath+"/" +fileNameOfFarmLandTxt) {
										string[] farmLandItem=e.Split(',');
										farmLandList.Add(new Farmland(Int32.Parse(farmLandItem[0]),Int32.Parse(farmLandItem[1])));
								}
								if (fileName==Application.persistentDataPath+"/"+fileNameOfCropInfoTxt) {
										string[] cropItem=e.Split(',');
										cropList.Add(new Crop(cropItem[0],Int32.Parse(cropItem[1]),Int32.Parse(cropItem[2]),Int32.Parse(cropItem[3])));
								}
									

									
								}
							

				
						}

					}

		
			
				
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
				return true;
			}
		}
		
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (Exception e)
		{
			Debug.Log(e.Message);
			GameObject.Find ("Text1").GetComponent<Text> ().text=e.Message;
			return false;
		}
	}

	public struct Farmland{
		public int landSpot;
		public int costToPurchase;
		
		public Farmland(int _landSpot, int _costToPurchase){
			this.landSpot = _landSpot;
			this.costToPurchase = _costToPurchase;
		}
	}

	public struct Crop{
		public string cropName;
		public int level;
		public int costToPlant;
		public int cashOutputPerDay;
		
		public Crop(string _cropName,int _level,int _costToPlant, int cashOutputPerDay){
			this.cropName = _cropName;
			this.level=_level;
			this.costToPlant=_costToPlant;
			this.cashOutputPerDay=_costToPlant;

		}
	}
}

