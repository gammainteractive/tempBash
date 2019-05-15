using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {

    public TextMeshProUGUI newPatternText;
    public TimerBar timerBar;
    public HealthBar playerHealth;
    public HealthBar enemyHealth;
    public HitComboUI hitCombo;
    public UltraBar ultraBar;
    public UltraButton ultraButton;
    public WatchPrompt promptText;
    public DamageText damageText;
    public TextMeshProUGUI gameOver;
    public GameObject restartButton;

	// Use this for initialization
	void Start () {
        ActivateTimerBar(false);
        gameOver.enabled = false;
        restartButton.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HitEnemy()
    {
        //enemyHealth.TakeHit();
    }

    public void SetGameOver(bool winner)
    {
        if (winner)
        {
            gameOver.text = "Game Over\n<color=yellow>You Win";
        }
        else
        {
            gameOver.text = "Game Over\nYou Lose";
            playerHealth.UpdateHealthBar(0, 100);
        }
        gameOver.enabled = true;
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
            UpdateUltraBar(0, GameManager.instance.MaxUltraLevel);
        }
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

    public void UpdateUltraBar(int ultraLevel, int maxLevel)
    {
        ultraBar.UpdateUltraMeter(ultraLevel, maxLevel);
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
