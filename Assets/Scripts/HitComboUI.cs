using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitComboUI : MonoBehaviour {

    public GameObject m_comboTextContainer;
    public TextMeshProUGUI comboText;
    string comboFormat = " <color=white>{0}</color>";
   
    string m_damageFormat = "<color=white>{0}</color><color=yellow>x</color><br><color=red>HIT</color>";

    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        ActivateHitCombo(true);
        Debug.Log("Combo activated from Debug");
#else
          ActivateHitCombo(false);
#endif
    }

    public void ActivateHitCombo(bool active)
    {
        m_comboTextContainer.SetActive(active);
        //comboText.enabled = active;
    }

    public void SetHitCombo(int hitCombo)
    {
        comboText.text = string.Format(comboFormat, hitCombo);
    }
}
