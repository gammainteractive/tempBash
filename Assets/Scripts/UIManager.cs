using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {

    public Transform[] UiViews;
    public enum UI_VIEW
    {
        TITLE_SCREEN,
        GAME_VIEW
    }

    public TextMeshProUGUI newPatternText;
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

	// Use this for initialization
	void Start () {
        ActivateTimerBar(false);
        restartButton.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
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
        ActivatePromptText(false);
    }

    public void UltraMode(bool modeOn)
    {
        ActivateUltraArcadeButton(modeOn);
        ActivateUltraReadyButton(!modeOn);
        if (!modeOn)ReadyUltraButton(false);
        if (modeOn)
        {
            ultraBar.DrainUltraBar();
        }
        else
        {
            SetUltraLevelText(0);
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

    public void PromptPrompt(bool prompt)
    {
        if (prompt) promptText.Prompt();
        else promptText.Input();
    }

    public void SetReadyCooldown()
    {
        ultraButton.CoolDown();
    }

    public void SetReadyInteractable(bool interactable)
    {
        ultraButton.SetReadyInteractable(interactable);
    }

    public void ActivateUltraReadyButton(bool activate)
    {
        ultraButton.ActivateReadyButton(activate);
    }

    public void ReadyUltraButton(bool ready)
    {
        ultraButton.ReadyUltraButton(ready);
    }

    public void ActivateUltraArcadeButton(bool activate)
    {
        ultraButton.ActivateArcadeButton(activate);
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

    public void ActivateNewPattern()
    {
        StartCoroutine(NewPatternDelay());
    }

    IEnumerator NewPatternDelay()
    {
        newPatternText.enabled = true;
        yield return new WaitForSeconds(1);
        newPatternText.enabled = false;

    }
}
