using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UYltraCounter : MonoBehaviour {

    TextMeshProUGUI counter;

	// Use this for initialization
	void Awake () {
        counter = GetComponent<TextMeshProUGUI>();
        counter.text = 0.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        counter.text = GameManager.instance.numUltraHits.ToString() + " Hits!!";
	}
}
