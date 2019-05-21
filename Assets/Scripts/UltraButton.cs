using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltraButton : MonoBehaviour
{
    public GameObject arcadeButton;
    public GameObject readyButtonGO;
    Image readyButtonImage;
    public Color buttonColor;
    Button readyButton;
    TextMeshProUGUI ultraText;
    bool coolDown;
    public float ultraCooldownTime;
    float currentTime;

    // Use this for initialization
    void Awake()
    {
        readyButtonImage = readyButtonGO.GetComponentInChildren<Image>();
        buttonColor = readyButtonImage.color;
        readyButton = readyButtonGO.GetComponent<Button>();
        ultraText = GetComponentInChildren<TextMeshProUGUI>();
        readyButtonImage.fillAmount = 1;
        SetReadyInteractable(false);
    }

    private void Start()
    {
        ActivateArcadeButton(false);
        ReadyUltraButton(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDown
            && GameManager.instance.currentState != GameManager.GameState.GameOver)
        {
            if (currentTime < GameManager.instance.UltraCoolDownTime)
            {
                currentTime += Time.deltaTime;
                readyButtonImage.fillAmount = currentTime / GameManager.instance.UltraCoolDownTime;
            }
            else
            {
                coolDown = false;
                readyButtonImage.fillAmount = 1;
                readyButton.interactable = true;
                GameManager.instance.dangerMode = false;
            }
        }
    }

    public void ReadyUltraButton(bool active)
    {
        readyButtonImage.color = active ? buttonColor : Color.white;
        //readyButton.enabled = active;
        ultraText.enabled = active;
    }

    public void HitReadyButton()
    {
        GameManager.instance.UltraReadyButtonHit();
    }

    public void HitArcadeButton()
    {
        GameManager.instance.UltraArcadeButtonHit();
    }

    public void CoolDown()
    {
        coolDown = true;
        currentTime = 0;
    }

    public void SetReadyInteractable(bool interact)
    {
        readyButton.interactable = interact;
    }
    

    public void ActivateArcadeButton(bool active)
    {
        arcadeButton.SetActive(active);

    }

    public void ActivateReadyButton(bool active)
    {
        readyButtonGO.SetActive(active);
    }


}
