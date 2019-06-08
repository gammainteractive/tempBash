using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltraButton : MonoBehaviour
{
    public GameObject m_ultraGO;
    public GameObject m_ultraEffects;
    public GameObject m_ultraFillGO;
    public RectTransform m_actionIcon;
    public bool coolDown;
    public float ultraCooldownTime;
    public bool m_startCooldown = false;
    public Animator m_animator;

    private float m_yOffsetPressed = 15f;
    private Vector2 m_defaultPosition;
    private Image m_ultraFillImage;
    private Button m_ultraFillButton;
    private Button m_ultraButton;
    private float currentTime;

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
        m_defaultPosition = new Vector2(m_actionIcon.anchoredPosition.x, m_actionIcon.anchoredPosition.y);

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

    public void ActionIconAdjust(bool _isDefault)
    {
        if (_isDefault)
        {
            m_actionIcon.anchoredPosition = m_defaultPosition;
        }
        else
        {
            m_actionIcon.anchoredPosition = m_defaultPosition + new Vector2(0, -m_yOffsetPressed);
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

    public void UltraEffectsToggle(bool _isEnable)
    {
        m_ultraEffects.SetActive(_isEnable);
        m_animator.SetBool("UltraEffects", _isEnable);
    }

    public void SetUltraButtonInteractable(bool interact)
    {
        m_ultraFillButton.interactable = interact;
    }

}
