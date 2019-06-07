﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltraButton : MonoBehaviour
{
    public GameObject m_ultraGO;
    Button m_ultraButton;

    public GameObject m_ultraFillGO;
    Image m_ultraFillImage;
    Button m_ultraFillButton;
   

    public bool coolDown;
    public float ultraCooldownTime;
    float currentTime;

    public bool m_startCooldown = false;

    // Use this for initialization
    void Awake()
    {
        m_ultraFillImage = m_ultraFillGO.GetComponentInChildren<Image>();
        m_ultraFillButton = m_ultraFillGO.GetComponent<Button>();

        m_ultraButton = m_ultraGO.GetComponent<Button>();
        
        m_ultraFillImage.fillAmount = 0;
        SetUltraButtonInteractable(false);
    }

    private void Start()
    {
        SetUltraButtonInteractable(false);  
        GameManager.instance.StartGameHandle += () => {
            StartUltraCooldown();
        };
        GameManager.instance.EndGameHandle += () =>
        {
            StopUltraCooldown();
        };
    }

    private void OnEnable()
    {
        StartUltraCooldown();
    }

    public void StartUltraCooldown()
    {
        CoolDown();
        m_startCooldown = true;
    }

    private void StopUltraCooldown()
    {
        m_startCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDown
            && GameManager.instance.currentState != GameManager.GameState.GameOver
            && m_startCooldown)
        {
            if (currentTime < GameManager.instance.UltraCoolDownTime)
            {
                currentTime += Time.deltaTime;
                m_ultraFillImage.fillAmount = currentTime / GameManager.instance.UltraCoolDownTime;
            }
            else
            {
                coolDown = false;
                m_ultraFillImage.fillAmount = 1;
                m_ultraFillButton.interactable = true;
            }
        }
    }

    public void HitUltraButton()
    {
        GameManager.instance.UltraButtonHit();
    }

    public void CoolDown()
    {
        coolDown = true;
        currentTime = 0;
    }

    public void SetUltraButtonInteractable(bool interact)
    {
        m_ultraFillButton.interactable = interact;
    }

}
