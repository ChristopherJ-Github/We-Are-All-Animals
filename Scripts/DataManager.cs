using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Data {
	
	public float waterLevel;
	public float linearSnowLevel;
}

public class DataManager : Singleton<DataManager> {
	
	[HideInInspector] public Data data;
	private string path;
	
	public delegate void SaveNotifier ();
	public event SaveNotifier OnSave;
	public void notifySave () {
		
		if (OnSave != null)
			OnSave();
	}
	
	[HideInInspector] public bool successfullyLoaded;
	void Start () {
		
		data = new Data ();
		path = Application.dataPath + "/data.dat";
		Debug.Log ("Data saved at: " + path);
		successfullyLoaded = Load ();
	}
	
	bool Load () {
		
		if (File.Exists(path)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(path, FileMode.Open);
			data = (Data)bf.Deserialize(file);
			file.Close ();
			return true;
		}
		return false;
	}
	
	void Save () {
		
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create(path);
		bf.Serialize (file, data);
		file.Close ();
	}
	
	void OnDestroy () {
		
		notifySave ();
		Save ();
	}
}
