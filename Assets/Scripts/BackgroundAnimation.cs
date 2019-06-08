using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimation : MonoBehaviour {

    public static BackgroundAnimation Instance;
    public Image m_image;
    Material m_material;
    public bool m_run = true;
    private float m_offset = 0;
    public float m_speedModifier = 1.6f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_material = m_image.material;
    }

    // Update is called once per frame
    void Update () {
        if (m_run)
        {
            m_offset += Time.deltaTime * m_speedModifier;
            m_material.mainTextureOffset = new Vector2(m_offset, 0);
            if(m_offset >= 1)
            {
                m_offset = 0;
            }
        }
    }

    private void OnDisable()
    {
        m_run = false;
    }
}
