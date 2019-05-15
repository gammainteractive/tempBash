using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageModifierText : MonoBehaviour {

    TextMeshProUGUI damageModifierText;
    string m_damageFormat = "<color=red>{0}x Damage Bonus </color>";
    string m_hitFormat = "<color=red>{0} Damage</color>";

    // Use this for initialization
    void Start()
    {
        damageModifierText = GetComponentInChildren<TextMeshProUGUI>();
        ActivateDamageModifierText(false);

    }

    public void SetdamageModifierText(float _damageModifier)
    {
        ActivateDamageModifierText(true);
        damageModifierText.text = string.Format(m_damageFormat, _damageModifier);
        //Invoke("DeActivatedamageModifierText", 3);
    }
    public void ActivateDamageModifierText(bool _activate)
    {
        damageModifierText.enabled = _activate;
    }
}
