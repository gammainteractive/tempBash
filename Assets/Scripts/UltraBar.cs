using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltraBar : MonoBehaviour {

    public TextMeshProUGUI ultraLevelText;
    public Image[] m_ultraBar;
    public Color[] m_barColors;
    public int m_ultraBarLevel = 0;
    public int m_currentUltraBar = 0;
    string ultraTextFormat = "Lvl {0}";
    bool drainUltra;
    public float ultraDrainTime;
    public float currentDrainTime;
    public int startDrainUltraLevel;


    // Use this for initialization
    void Start () {
        foreach(Image _image in m_ultraBar)
        {
            _image.fillAmount = 0;
        }
        //TryDrain();
        SetUltraLevelText(0);
    }

    // Update is called once per frame
    void Update () {
        if (drainUltra 
            && GameManager.instance.currentState != GameManager.GameState.GameOver)
        {
            if (currentDrainTime > 0)
            {
                currentDrainTime -= Time.deltaTime;
                //FillMeter(currentDrainTime / ultraDrainTime);
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

    /*void FillMeter(float ultraRatio)
    {
        Vector2 size = ultraBar.rectTransform.sizeDelta;
        ultraRatio *= (float) startDrainUltraLevel / GameManager.instance.MaxUltraLevel;
        size.x = ultraRatio * startBarWidth;
        ultraBar.rectTransform.sizeDelta = size;
    }*/

    public void SetUltraBarFill(float _fill)
    {
        m_ultraBar[m_currentUltraBar].fillAmount = _fill;
    }

   /*void SetFillMeter(int ultraLevel, int maxLevel)
    {
        Vector2 size = ultraBar.rectTransform.sizeDelta;
        float levelRatio = (float) ultraLevel / maxLevel;
        size.x = levelRatio * startBarWidth;
        ultraBar.rectTransform.sizeDelta = size;
    }*/

    public void AddUltraBar()
    {
        if (m_ultraBarLevel < m_barColors.Length - 1)
        {
            m_ultraBarLevel++;
        } else
        {
            return;
        }

        ChangeBarColors(1);

        SwitchUltraBar();

        ChangeBarColors(0);
    }

    public void SwitchUltraBar()
    {
        m_ultraBar[m_currentUltraBar].transform.SetAsFirstSibling();
        if (m_currentUltraBar == 0)
        {
            m_currentUltraBar = 1;
        }
        else
        {
            m_currentUltraBar = 0;
        }
    }

    public void ReduceUltraBar()
    {
        if (m_ultraBarLevel == 0)
        {
            return;
        }
        Debug.Log("Reduce Ultra Bar");
        m_ultraBarLevel--;
        m_ultraBar[m_currentUltraBar].GetComponent<Image>().color = m_barColors[m_ultraBarLevel];

        if (m_ultraBarLevel > 0)
        {
            if (m_currentUltraBar == 0)
            {
                m_ultraBar[1].GetComponent<Image>().color = m_barColors[m_ultraBarLevel - 1];
            }
            else
            {
                m_ultraBar[0].GetComponent<Image>().color = m_barColors[m_ultraBarLevel - 1];
            }
        }
    }

    void ChangeBarColors(int _index)
    {
        if(m_ultraBarLevel >= 2)
        {
            m_ultraBar[m_currentUltraBar].GetComponent<Image>().color = m_barColors[m_ultraBarLevel - _index];
        }
    }

    public void ResetUltraBar()
    {
        m_currentUltraBar = 0;
        m_ultraBarLevel = 0;
        for(int i = 0; i <= 1; i++)
        {
            m_ultraBar[i].fillAmount = 0;
            m_ultraBar[i].GetComponent<Image>().color = m_barColors[i];
        }
      //  ChangeBarColors();
    }

    public void SetUltraLevelText(int ultraLevel)
    {
        ultraLevelText.text = string.Format(ultraTextFormat, ultraLevel);
    }
}
