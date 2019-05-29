using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimonButton : MonoBehaviour
{
    
    Image myImage;
    Button myButton;
    public RectTransform m_actionIcon;
    private Vector2 m_defaultPosition;
    private float m_yOffsetPressed = 15;
    private float m_buttonPressedTime = 0.15f;
    public Sprite m_defaultButtonSprite;
    public Sprite m_ButtonWhenShowingPattern;
    public Sprite m_ButtonWhenPressed;
    public Sprite m_ButtonIncorrect;
    GameManager gameManager;

    public Sprite[] m_ButtonColors;

	// Use this for initialization
	void Awake () {
        gameManager = FindObjectOfType<GameManager>();
        myImage = GetComponentInChildren<Image>();
        myButton = GetComponent<Button>();
        //buttonColor = myImage.color;
        m_defaultButtonSprite = GetComponent<Image>().sprite;
    }

    private void Start()
    {
        m_defaultPosition = m_actionIcon.anchoredPosition;
        myButton.onClick.AddListener(ButtonHit);
    }

    public void SimonAlert()
    {
        StartCoroutine(ButtonAlert());
    }

    IEnumerator ButtonAlert()
    {
        myImage.sprite = m_ButtonWhenShowingPattern;
        yield return new WaitForSeconds(GameManager.instance.ButtonAlertTime);
        myImage.sprite = m_defaultButtonSprite;
    }

    void ButtonHit()
    {
        if (gameManager.SimonButtonHit(this))
        {
           // myImage.sprite = m_ButtonWhenPressed;
            StartCoroutine(CorrectButtonPress());
        }
        else
        {
            StartCoroutine(IncorrectButtonPress());
        }
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
