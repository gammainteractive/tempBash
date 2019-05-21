using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

    Image healthBar;
    TextMeshProUGUI healthText;
    string healthTextFormat = "{0} HP";

    // Use this for initialization
    void Awake()
    {
        healthBar = GetComponentInChildren<Image>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
        UpdateHealthText(currentHealth);
    }

    public void UpdateHealthText(float health)
    {
        healthText.text = string.Format(healthTextFormat, health);
    }

}