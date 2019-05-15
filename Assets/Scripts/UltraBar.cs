using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltraBar : MonoBehaviour {

    public TextMeshProUGUI ultraLevelText;
    public Image ultraBar;
    float startBarWidth;
    string ultraTextFormat = "Lvl {0}";
    bool drainUltra;
    public float ultraDrainTime;
    public float currentDrainTime;
    public int startDrainUltraLevel;


    // Use this for initialization
    void Start () {
        ultraBar.fillAmount = 0;
        startBarWidth = ultraBar.rectTransform.sizeDelta.x;
        //TryDrain();
        UpdateUltraMeter(GameManager.instance.ultraLevel, GameManager.instance.MaxUltraLevel);
    }

    // Update is called once per frame
    void Update () {
        if (drainUltra)
        {
            if (currentDrainTime > 0)
            {
                currentDrainTime -= Time.deltaTime;
                FillMeter(currentDrainTime / ultraDrainTime);
            } else
            {
                UltraEnded();
                drainUltra = false;
            }
        }
    }

    void UltraEnded()
    {
        GameManager.instance.UltraEnded();
    }

    public void DrainUltraBar()
    {
        startDrainUltraLevel = GameManager.instance.ultraLevel;
        ultraDrainTime = (float) startDrainUltraLevel * GameManager.instance.DrainTimePerUltraLevel;
        currentDrainTime = ultraDrainTime;
        drainUltra = true;
    }

    public void UpdateUltraMeter(int ultraLevel, int maxLevel)
    {
        SetFillMeter(ultraLevel, maxLevel);
        SetLevelText(ultraLevel);
    }

    void FillMeter(float ultraRatio)
    {
        Vector2 size = ultraBar.rectTransform.sizeDelta;
        ultraRatio *= (float) startDrainUltraLevel / GameManager.instance.MaxUltraLevel;
        size.x = ultraRatio * startBarWidth;
        ultraBar.rectTransform.sizeDelta = size;
    }



    void SetFillMeter(int ultraLevel, int maxLevel)
    {
        Vector2 size = ultraBar.rectTransform.sizeDelta;
        float levelRatio = (float) ultraLevel / maxLevel;
        size.x = levelRatio * startBarWidth;
        ultraBar.rectTransform.sizeDelta = size;
    }

    void SetLevelText(int ultraLevel)
    {
        ultraLevelText.text = string.Format(ultraTextFormat, ultraLevel);
    }
}
