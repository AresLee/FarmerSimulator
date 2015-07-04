using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;  
using System;
using System.Collections.Generic;

public struct Farmland{
	public int landSpot;
	public int costToPurchase;

	public Farmland(int _landSpot, int _costToPurchase){
		this.landSpot = _landSpot;
		this.costToPurchase = _costToPurchase;
	
	}
}
public class DataReader : MonoBehaviour {
	private string pathOfTextFile;
	List<int> numberList;

	List<Farmland> farmLandList;

	// Use this for initialization
	void Start () {
	
		pathOfTextFile="Assets/FarmlandInformation.txt";
		numberList = new List<int> ();
		farmLandList = new List<Farmland> ();
		Load (pathOfTextFile);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private bool Load(string fileName)
	{
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
									string[] farmLandItem=e.Split(',');
									farmLandList.Add(new Farmland(Int32.Parse(farmLandItem[0]),Int32.Parse(farmLandItem[1])));
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

			return false;
		}
	}
}

