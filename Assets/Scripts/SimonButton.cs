using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class SimonButton : MonoBehaviour
{
    public int m_buttonValue = 0;
    public GameObject m_buttonTarget;
    Image myImage;
    Button myButton;
    public RectTransform m_actionIcon;
    public Image m_actionImage;
    private Vector2 m_defaultPosition;
    private float m_yOffsetPressed = 15f;
    private float m_buttonPressedTime = 0.15f;
    public Sprite m_defaultButtonSprite;
    public Sprite m_ButtonWhenShowingPattern;
    public Sprite m_ButtonWhenPressed;
    public Sprite m_ButtonIncorrect;
    private Sprite m_defaultIcon;

    public Sprite[] m_ButtonColors;

    public GameObject m_ultraEffects;
    public Animator m_ultraEffectsAnimator;

    public SimonButton(SimonButton _button)
    {
        m_buttonValue = _button.m_buttonValue;
        m_defaultButtonSprite = _button.m_defaultButtonSprite;
        m_ButtonWhenPressed = _button.m_ButtonWhenPressed;
        m_ButtonWhenShowingPattern = _button.m_ButtonWhenShowingPattern;
        m_defaultButtonSprite = _button.m_defaultButtonSprite;
        m_ButtonIncorrect = _button.m_ButtonIncorrect;
    }

	// Use this for initialization
	void Awake () {
        myImage = m_buttonTarget.GetComponent<Image>();
        myButton = m_buttonTarget.GetComponent<Button>();
        m_defaultButtonSprite = m_buttonTarget.GetComponent<Image>().sprite;
        m_defaultIcon = m_actionImage.sprite;
    }

    private void Start()
    {
        m_ultraEffects.SetActive(false);
        //For reason these reset back to 0 when in game
        m_yOffsetPressed = 15;
        m_buttonPressedTime = 0.15f;
        
        m_defaultPosition = new Vector2(m_actionIcon.anchoredPosition.x, m_actionIcon.anchoredPosition.y);
        myButton.onClick.AddListener(ButtonHit);
    }

   /* public void Initialize()
    {
        m_actionImage = m_actionIcon.GetComponent<Image>();
        m_ultraEffectsAnimator = GetComponentInChildren<Animator>();
        m_ultraEffects = m_ultraEffectsAnimator.gameObject;
        m_ultraEffects.SetActive(false);
        //For reason these reset back to 0 when in game
        m_yOffsetPressed = 15;
        m_buttonPressedTime = 0.15f;

        m_defaultPosition = new Vector2(m_actionIcon.anchoredPosition.x, m_actionIcon.anchoredPosition.y);
        myButton.onClick.AddListener(ButtonHit);
    }*/

    private void InitializeView()
    {
        myImage.sprite = m_defaultButtonSprite;
    }

    public void SimonAlert()
    {
        StartCoroutine(ButtonAlert());
    }

    public void SetProperties(SimonButton _simonButton, bool _isUltraMode = false)
    {
        if (!_isUltraMode)
        {
            SetDefaultIcon();
        }
        m_buttonValue = _simonButton.m_buttonValue;
        m_defaultButtonSprite = _simonButton.m_defaultButtonSprite;
        m_ButtonWhenPressed = _simonButton.m_defaultButtonSprite;
        InitializeView();
    }

    public void ToggleUltraEffects(bool _isEnable)
    {
        if(m_ultraEffectsAnimator == null)
        {
            return;
        }
        m_ultraEffects.SetActive(_isEnable);
        m_ultraEffectsAnimator.SetBool("UltraEffects", _isEnable);
    }

    public void SetButtonValue(int _value)
    {
        m_buttonValue = _value;
    }

    IEnumerator ButtonAlert()
    {
        myImage.sprite = m_ButtonWhenShowingPattern;
        yield return new WaitForSeconds(GameManager.instance.ButtonAlertTime);
        myImage.sprite = m_defaultButtonSprite;
    }

    void ButtonHit()
    {
        Debug.Log("Button hit");
        //Enable button hits when buttons are not animating or it will cause bugs
       // if (!UIManager.Instance.m_startButtonAnimationModeB){
            if (GameManager.instance.SimonButtonHit(m_buttonValue))
            {
                StartCoroutine(CorrectButtonPress());
            }
            else
            {
                StartCoroutine(IncorrectButtonPress());
            }
        //}
    }

    public void SetDefaultIcon()
    {
        m_actionImage.sprite = m_defaultIcon;
    }

    public void SetUltraIcon(Sprite _icon)
    {
        m_actionImage.sprite = _icon;
    }

    private IEnumerator CorrectButtonPress()
    {
        ActionIconAdjust(false);
        myImage.sprite = m_ButtonWhenPressed;
        yield return new WaitForSeconds(m_buttonPressedTime);
        ActionIconAdjust(true);
        myImage.sprite = m_defaultButtonSprite;
    }

    private IEnumerator IncorrectButtonPress()
    {
        ActionIconAdjust(false);
        myImage.sprite = m_ButtonIncorrect;
        yield return new WaitForSeconds(m_buttonPressedTime);
        ActionIconAdjust(true);
        myImage.sprite = m_defaultButtonSprite;
    }

    public void SetButtonInteractable(bool interactable)
    {
        myButton.interactable = interactable;
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
}
