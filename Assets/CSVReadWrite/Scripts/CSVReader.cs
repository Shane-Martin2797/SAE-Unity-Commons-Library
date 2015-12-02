using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class CSVReader : SingletonBehaviourII<CSVReader>
{

	//This is the file we are writing to/reading from
	public TextAsset file;
	
	
	protected override void OnSingletonAwake ()
	{
		Load ();
	}
	
	void Update ()
	{
		if (Input.anyKeyDown) {
			Save ();
		}
	}
	/// <summary>
	/// Loads the inventory.
	/// </summary>
	void Load ()
	{
		//This is to get all the lines
		string[] lines = file.text.Split ("\n" [0]);
		for (var i = 0; i < lines.Length; ++i) {
			//This is to get every thing that is comma separated
			string[] parts = lines [i].Split ("," [0]);
			for (int j = 0; j < parts.Length; ++j) {
				//Do something in here with the loop. It loops through everything
				//That is saved in the CSV.
				Debug.Log ("Line: " + i + " Part: " + j + " Says " + parts [j]);
			}
			
		}
	}
	
	/// <summary>
	/// Saves the inventory.
	/// </summary>
	void Save ()
	{
		string filePath = getPath ();
		
		//This is the writer, it writes to the filepath
		StreamWriter writer = new StreamWriter (filePath);
		
		//This writes in a for loop.
		for (int i = 0; i < 10; ++i) {
			writer.WriteLine ("Hello" + "," + "This" + "," + "is" + "," + "Line" + (i + 1));
		}
		
		//This is just an example out of the for loop
		writer.WriteLine ("Hello" + "," + "Something" + "," + "AnotherVariable");
		
		writer.Flush ();
		//This closes the file
		writer.Close ();
	}
	
	//This gets the path (Thanks Unity)! using Application. works
	private string getPath ()
	{
		#if UNITY_EDITOR
		return Application.dataPath + "/CSVReadWrite" + "/CSV/" + "CSVFile.csv";
		#elif UNITY_ANDROID
		return Application.persistentDataPath+"CSVFile.csv";
		#elif UNITY_IPHONE
		return Application.persistentDataPath+"/"+"CSVFile.csv";
		#else
		return Application.dataPath +"/"+"CSVFile.csv";
		#endif
	}	
}
