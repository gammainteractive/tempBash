using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    public Transform[] UiViews;
    public enum UI_VIEW
    {
        TITLE_SCREEN,
        GAME_VIEW
    }

    public Transform[] m_gameModeViews;
    public enum GAME_MODE_VIEW
    {
        MODE_A,
        MODE_B
    }

    public Transform[] m_buttonRowsModeB;
    public TimerBar timerBar;
    public HealthBar playerHealth;
    public HealthBar enemyHealth;
    public HitComboUI hitCombo;
    public UltraBar ultraBar;
    public UltraButton ultraButton;
    public WatchPrompt promptText;
    public DamageText damageText;
    public GameObject gameOver;
    public GameObject restartButton;
    public DamageModifierText m_damageModifier;
    public Transform m_ultraHitsText;

    private List<Vector3> m_defaultButtonPositionModeB;
    private List<Vector3> m_defaultButtonScaleModeB;
    public List<Vector3> m_targetButtonPositionsModeB;
    public List<Vector3> m_targetButtonScaleModeB;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        ActivateTimerBar(false);
        restartButton.SetActive(false);
        InitDefaultPositions();
    }

    private void InitDefaultPositions()
    {
        m_defaultButtonPositionModeB = new List<Vector3>();
        m_defaultButtonScaleModeB = new List<Vector3>();
        m_targetButtonPositionsModeB = new List<Vector3>();
        m_targetButtonScaleModeB = new List<Vector3>();

        for (int i = 0; i < m_buttonRowsModeB.Length; i++)
        {
            Vector2 _defaultPosition = m_buttonRowsModeB[i].localPosition;
            Vector2 _defaultScale = m_buttonRowsModeB[i].localScale;
            m_defaultButtonPositionModeB.Add(_defaultPosition);
            m_defaultButtonScaleModeB.Add(_defaultScale);
            m_targetButtonPositionsModeB.Add(_defaultPosition);
            m_targetButtonScaleModeB.Add(_defaultScale);
        }
    }

    public void TitleView()
    {
        UiViews[(int)UI_VIEW.GAME_VIEW].gameObject.SetActive(false);
        UiViews[(int)UI_VIEW.TITLE_SCREEN].gameObject.SetActive(true);
    }

    public void GameView()
    {
        UiViews[(int)UI_VIEW.GAME_VIEW].gameObject.SetActive(true);
        UiViews[(int)UI_VIEW.TITLE_SCREEN].gameObject.SetActive(false);
    }

    public void HitEnemy()
    {
        //enemyHealth.TakeHit();
    }

    public void UpdatePlayerHealthText(float _value)
    {
        playerHealth.UpdateHealthText(_value);
    }

    public void UpdateEnemyHealthText(float _value)
    {
        enemyHealth.UpdateHealthText(_value);
    }

    public void SetGameOver(bool winner)
    {
        
        if (winner)
        {
            gameOver.GetComponentInChildren<TextMeshProUGUI>(true).text = "Game Over\n<color=yellow>You Win";
        }
        else
        {
            gameOver.GetComponentInChildren<TextMeshProUGUI>(true).text = "Game Over\nYou Lose";
            playerHealth.UpdateHealthBar(0, 100);
        }
        gameOver.SetActive(true);
        restartButton.SetActive(true);
       
    }

    public void UltraMode(bool modeOn)
    {
        UltraTextSetActive(modeOn);
        if (modeOn)
        {
            ultraBar.DrainUltraBar();
        }
        else
        {
            SetUltraLevelText(0);
        }
    }

    void Update()
    {

    }

    public bool SetButtonRowInteractive(int _currentRow)
    {
        Button[] _buttons = m_buttonRowsModeB[_currentRow].GetComponentsInChildren<Button>();
        bool _indexReset = true;

        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = false;
        }

        //If current row is max row, reset to 0
        if (_currentRow == m_buttonRowsModeB.Length - 1)
        {
            _buttons = m_buttonRowsModeB[0].GetComponentsInChildren<Button>();
        }
        else
        {
            _buttons = m_buttonRowsModeB[_currentRow + 1].GetComponentsInChildren<Button>();
            _indexReset = false;
        }

        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = true;
        }

        return _indexReset;
    }

    public void ModeBMoveButtons()
    {
        Vector3 m_tempPos = m_targetButtonPositionsModeB[m_buttonRowsModeB.Length - 1];
        Vector3 m_tempScale = m_targetButtonScaleModeB[m_buttonRowsModeB.Length - 1];
        for (int i = m_buttonRowsModeB.Length - 1; i >= 0; i--)
        {
            if(i != 0) {
                m_targetButtonPositionsModeB[i] = m_targetButtonPositionsModeB[i - 1];
                m_targetButtonScaleModeB[i] = m_targetButtonScaleModeB[i - 1];
            } else
            {
                m_targetButtonPositionsModeB[i] = m_tempPos;
                m_targetButtonScaleModeB[i] = m_tempScale;
            }
           
            m_buttonRowsModeB[i].localScale = m_targetButtonScaleModeB[i];
            m_buttonRowsModeB[i].localPosition = m_targetButtonPositionsModeB[i];

        }

       
    }

    public void SetDamageModifier(float _modifier)
    {
        m_damageModifier.SetdamageModifierText(_modifier);
    }

    public void HitDamage(int damager)
    {
        damageText.SetDamageText(damager);
    }

    public void SetDamageText(int combo, int ultra, int damage)
    {
        damageText.SetDamageText(combo, ultra, damage);
    }
    public void ActivatePromptText(bool activate)
    {
        promptText.ActivatePromptText(activate);

    }

    public void ShowGameModeView(GAME_MODE_VIEW _gameViewMode)
    {
        for(int i = 0; i < m_gameModeViews.Length; i++)
        {
            m_gameModeViews[i].gameObject.SetActive(false);
        }
        m_gameModeViews[(int)_gameViewMode].gameObject.SetActive(true);
    }


    public void PromptPrompt(bool prompt)
    {
        if (prompt) promptText.Prompt();
        else promptText.Input();
    }

    public void SetReadyCooldown()
    {
        ultraButton.CoolDown();
    }

    public void SetUltraButtonInteractable(bool interactable)
    {
        ultraButton.SetUltraButtonInteractable(interactable);
    }

    public void UltraTextSetActive(bool ready)
    {
        m_ultraHitsText.gameObject.SetActive(ready);
    }

    public void SetUltraLevelText(int ultraLevel)
    {
        ultraBar.SetUltraLevelText(ultraLevel);
    }

    public void ResetUltraBar()
    {
        ultraBar.ResetUltraBar();
    }

    public void SwitchUltraBar()
    {
        ultraBar.SwitchUltraBar();
    }

    public void AddUltraBar()
    {
        ultraBar.AddUltraBar();
    }

    public void ReduceUltraBar()
    {
        ultraBar.ReduceUltraBar();
    }

    public void UpdateUltraFill(float _fill)
    {
        ultraBar.SetUltraBarFill(_fill);
    }

    public void ActivateDamageModifier(bool activate)
    {
        m_damageModifier.ActivateDamageModifierText(activate);
    }

    public void ActivateHitCombo(bool activate)
    {
        hitCombo.ActivateHitCombo(activate);
    }

    public void UpdateHitCombo(int comboLevel)
    {
        hitCombo.SetHitCombo(comboLevel);
    }

    public void ActivateTimerBar(bool active)
    {
        timerBar.ActivateTimer(active);
    }
}
