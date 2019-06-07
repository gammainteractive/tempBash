using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour {

    private Image m_image;
    private float m_fadeSpeed = 1.6f;

    private void Start()
    {
        m_image = GetComponent<Image>();
    }

    public void ShowText()
    {
        Color m_temp = m_image.color;
        m_temp.a = 1f;
        m_image.color = m_temp;
    }

    public void StartFade()
    {
        StartCoroutine(AlphaFade());
    }

    IEnumerator AlphaFade()
    {
        Color m_temp = m_image.color;
        m_temp.a = 1f;
        float m_alpha = 1.0f;
        while (m_alpha > 0.0f)
        {
            m_alpha -= m_fadeSpeed * Time.deltaTime;
            m_temp.a = m_alpha;
            m_image.color = m_temp;
            yield return null;
        }
    }
}
