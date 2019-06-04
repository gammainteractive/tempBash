using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    public Image healthBarCurrent;
    TextMeshProUGUI healthText;
    string healthTextFormat = "{0} HP";

    // Use this for initialization
    void Awake()
    {
        //healthBarCurrent = GetComponentInChildren<Image>();
        //healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBarCurrent.fillAmount = currentHealth / maxHealth;
        UpdateHealthText(currentHealth);
    }

    public void UpdateHealthText(float health)
    {
        //healthText.text = string.Format(healthTextFormat, health);
    }

}