using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitComboUI : MonoBehaviour {

    TextMeshProUGUI comboText;
    string comboFormat = "{0} Hits!!";

	// Use this for initialization
	void Start () {
        comboText = GetComponentInChildren<TextMeshProUGUI>();
        SetHitCombo(1);
        ActivateHitCombo(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateHitCombo(bool active)
    {
        comboText.enabled = active;
    }

    public void SetHitCombo(int hitCombo)
    {
        comboText.text = string.Format(comboFormat, hitCombo);
    }
}
