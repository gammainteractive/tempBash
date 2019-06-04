using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadJSONdata : MonoBehaviour {
    public bool loadData;
    string path;
    string jsonString;

	// Use this for initialization
	void Awake () {
        if (loadData) LoadJsonData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadJsonData()
    {
        path = Application.streamingAssetsPath + "/MemoryFightVariables.json";
        jsonString = File.ReadAllText(path);
        GameManager gameMan = GetComponent<GameManager>();
        JsonUtility.FromJsonOverwrite(jsonString, gameMan);
    }
}
