using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour {

    TextMeshProUGUI damageText;
    string damageFormat = "<color=blue>{0} Hits</color> <size=40>x</size> <color=orange>{1} Ultra</color> = <color=red>{2} Damage</color>";
    string hitFormat = "<color=red>{0} Damage</color>";

    // Use this for initialization
    void Start () {
        damageText = GetComponentInChildren<TextMeshProUGUI>();
        DeActivateDamageText();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDamageText(int comboHits, int ultraHits, int totalDamage)
    {
        ActivateDamageText();
        damageText.text = string.Format(damageFormat, comboHits, ultraHits, totalDamage);
        Invoke("DeActivateDamageText", 3);
    }

    public void SetDamageText(int damage)
    {
        ActivateDamageText();
        damageText.text = string.Format(hitFormat, damage);
        Invoke("DeActivateDamageText", 0.25f);
    }

    void ActivateDamageText()
    {
        damageText.enabled = true;
    }

    void DeActivateDamageText()
    {
        damageText.enabled = false;
    }
}
