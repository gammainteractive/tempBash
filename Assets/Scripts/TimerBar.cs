using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour {
    
    public Image fillBar;
    public bool updateBar;
    GameManager gameManager;

	// Use this for initialization
	void Awake () {
        gameManager = GameManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
		if (updateBar)
        {
            fillBar.fillAmount = gameManager.currentInputTime / gameManager.inputTimePerPrompt;
        }
	}

    public void ActivateTimer(bool active)
    {
        fillBar.enabled = active;
        updateBar = active;
    }
}
