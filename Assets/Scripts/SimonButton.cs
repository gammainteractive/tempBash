using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimonButton : MonoBehaviour {
    
    Image myImage;
    Button myButton;
    Color buttonColor;
    GameManager gameManager;

	// Use this for initialization
	void Awake () {
        gameManager = FindObjectOfType<GameManager>();
        myImage = GetComponentInChildren<Image>();
        myButton = GetComponent<Button>();
        buttonColor = myImage.color;
        myImage.color = Color.white;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SimonAlert()
    {
        StartCoroutine(ButtonAlert());
    }

    IEnumerator ButtonAlert()
    {
        myImage.color = buttonColor;
        yield return new WaitForSeconds(GameManager.instance.ButtonAlertTime);
        myImage.color = Color.white;
    }

    public void ButtonHit()
    {
        gameManager.SimonButtonHit(this);
    }

    public void SetButtonInteractable(bool interactable)
    {
        myButton.enabled = interactable;
    }
    
}
