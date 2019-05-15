using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public bool startGame;
    public bool dangerMode;
    public enum GameState { DisplayPattern, PlayerInput, UltraMode, GameOver}
    public GameState currentState;
    public SimonButton[] simonButtons;
    public List<SimonButton> currentButtonPattern = new List<SimonButton>();
    public SimonButton[] inputButtonPattern;
    int startNumPatterns = 1;
    public int currentNumPatterns;
    public float ButtonAlertTime;

    public int MaxEnemyHealth;
    public int currentInputNum;
    UIManager uiManager;
    Player player;
    Enemy enemy;

    private float m_healthTimer = 1;
    public float m_currentHealthTimer = 0;

    [Header("Combo Meter")]
    public int MinComboGain = 1;
    public int MaxComboGain = 3;
    public int currentComboLevel = 0;
    public float m_comboDamageModifier = 0.1f;

    [Header("Ultra")]
    public float ultraTotalAmount;
    public int ultraLevel;
    public int MaxUltraLevel = 4;
    public int InputsForUltraLevel = 6;
    public int numUltraHits;
    public float DrainTimePerUltraLevel = 1;
    public float UltraCoolDownTime = 10;
    float m_ultraCurrentAmount = 0;
    [Space]

    [Header("Input Times")]
    public bool RampInputTimes = false;
    public float StartInputTime = 1;
    public float currentInputTime;
    public float inputTimePerPrompt;
    public int InputsForMinInputTime;
    public float MinInputTime;
    float inputTimeDelta;
    [Space]

    [Header("Prompt Times")]
    public bool RampPromptDelays = true;
    public float StartPromptDelay = 1;
    public float promptDelay;
    public float MinPromptDelay;
    public int InputsForMinPromptTime;
    float promptDelayDelta;
    Coroutine patternRoutine;
    Coroutine inputRoutine;
    [Space]

    public float PromptToInputDelay = 0.5f;


    void Awake()
    {
        instance = this;
        uiManager = FindObjectOfType<UIManager>();
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
    }


    // Use this for initialization
    void Start () {
        currentNumPatterns = startNumPatterns;
        if (startGame) StartGame();
        inputTimePerPrompt = StartInputTime;
        promptDelay = StartPromptDelay;
        inputTimeDelta = (StartInputTime - MinInputTime) / InputsForMinInputTime;
        promptDelayDelta = (StartPromptDelay - MinPromptDelay) / InputsForMinPromptTime;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F)) StopCoroutine(patternRoutine);
        if (currentState == GameState.PlayerInput)
        {
            if (currentInputTime > 0)
            {
                currentInputTime -= Time.deltaTime;
            }
            else
            {
                uiManager.ActivateTimerBar(true);
                MissedInput();
            }

        }

        if (dangerMode)
        {
            m_currentHealthTimer += Time.deltaTime;
            if (m_currentHealthTimer >= m_healthTimer)
            {
                m_currentHealthTimer = 0;
                player.AddHealth(10);
                if (player.currentHealth >= 100)
                {
                    dangerMode = false;
                }
            }
        }
    }

    void StartGame()
    {
        StartPattern(startNumPatterns, true);
    }
    
    void StartPattern(int numPatterns, bool newPattern = false)
    {
        if (currentState == GameState.GameOver) return;
        SetButtonsInteractable(false);
        currentInputNum = 0;
        currentNumPatterns = numPatterns;
        patternRoutine = StartCoroutine(PlaySimonPattern(numPatterns));
        ResetInputTimer();
        if (newPattern)
        {
            SetButtonPattern();
            uiManager.ActivateNewPattern();

        }
        else
        {
            AddButtonToPattern();
        }
    }

    IEnumerator PlaySimonPattern(int numPatterns)
    {
        currentState = GameState.DisplayPattern;
        uiManager.ActivatePromptText(true);
        uiManager.PromptPrompt(true);
        if (inputRoutine != null) StopCoroutine(inputRoutine);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < numPatterns; ++i)
        {
            currentButtonPattern[i].SimonAlert();
            if (i != numPatterns-1)
                yield return new WaitForSeconds(promptDelay);
        }
        // Time between final prompt and user input
        yield return new WaitForSeconds(PromptToInputDelay);
        uiManager.PromptPrompt(false);
        if (currentState != GameState.UltraMode || currentState != GameState.GameOver)
            GetPlayerInput();

        inputRoutine = StartCoroutine(InputDelay());

    }

    IEnumerator InputDelay()
    {
        yield return new WaitForSeconds(0.75f);
        uiManager.ActivatePromptText(false);
    }

    void GetPlayerInput()
    {
        SetButtonsInteractable(true);
        currentState = GameState.PlayerInput;
        uiManager.ActivateTimerBar(true);
        ResetInputTimer();
    }

    void ResetInputTimer()
    {
        currentInputTime = inputTimePerPrompt;
    }
    
    void MissedInput()
    {
        player.TakeHit(50);

        if (dangerMode)
        {
            GameOver(false); return;
        }

        uiManager.SetReadyInteractable(false);
        uiManager.SetReadyCooldown();
        uiManager.ActivateHitCombo(false);
        uiManager.ActivateDamageModifier(false);
        uiManager.UpdateHitCombo(1);
        currentComboLevel = 0;
        //promptDelay = StartPromptDelay;
        StartPattern(startNumPatterns, true);
        currentNumPatterns = startNumPatterns;
    
        if (ultraTotalAmount >= 1)
        {
            uiManager.UpdateUltraFill(0);
            uiManager.SwitchUltraBar();
            uiManager.ReduceUltraBar();
            uiManager.UpdateUltraFill(m_ultraCurrentAmount);
        } else
        {
            m_ultraCurrentAmount = 0;
            uiManager.UpdateUltraFill(0);
        }

        ultraTotalAmount = Mathf.Max(ultraTotalAmount - 1, 0);
        dangerMode = true;
        //uiManager.SetReadyInteractable(false);
        //uiManager.SetReadyCooldown();
        if (ultraLevel > 0) ultraLevel--;
        if (ultraLevel > 0)
        {
            uiManager.SetReadyCooldown();
        }
        else
        {
            uiManager.ReadyUltraButton(false);
        }
        uiManager.SetUltraLevelText(ultraLevel);
    }

    void StopPlayerInput()
    {
        SetButtonsInteractable(false);
        currentInputNum = 0;
    }

    bool HitCorrectButton(SimonButton button)
    {
        return currentButtonPattern[currentInputNum] == button;
    }

    void SetButtonPattern()
    {
        currentButtonPattern.Clear();
        for (int i = 0; i < currentNumPatterns; i++)
        {
            currentButtonPattern.Add(RandomButton());
        }
    }

    void AddButtonToPattern()
    {
        currentButtonPattern.Add(RandomButton());
    }

    SimonButton RandomButton()
    {
        return simonButtons[Random.Range(0, simonButtons.Length)];
    }

    public void SimonButtonHit(SimonButton button)
    {
        if (currentState == GameState.PlayerInput)
        {
            bool correct = HitCorrectButton(button);
            print(correct);
            if (correct)
            {
                // Hit Correct Button
                enemy.TakeHit(ComboMultiplier());
                uiManager.HitDamage(ComboMultiplier());
                currentInputNum++;
                GainCombo(currentInputTime/inputTimePerPrompt);
                if (currentComboLevel >= 3) uiManager.ActivateHitCombo(true);
                uiManager.UpdateHitCombo(currentComboLevel);

                float ultraMultiplier = (float)1 / InputsForUltraLevel;
                float ultraGained = (currentInputTime / inputTimePerPrompt) * ultraMultiplier;
                AddUltra(ultraGained);

                if (currentInputNum >= currentNumPatterns)
                {
                    // Correctly entered Sequence
                    UpdateInputTime();
                    UpdatePromptDelay();
                    StartPattern(currentNumPatterns + 1);
                    uiManager.ActivateTimerBar(false);
                } else
                {
                    ResetInputTimer();
                }
            } else
            {
                // Incorrect button response
                MissedInput();
            }

        }
    }

    void SetButtonsInteractable(bool interact)
    {
        foreach (var button in simonButtons)
        {
            button.SetButtonInteractable(interact);
        }
    }

    // Ultra Level Amounts

    public void UltraReadyButtonHit()
    {
        uiManager.UltraMode(true);
        ActivateUltraMode();
    }

    public void UltraArcadeButtonHit()
    {
        if (currentState != GameState.UltraMode) return;
        numUltraHits++;
    }

    public void AddUltra(float amount)
    {
        if (ultraLevel >= MaxUltraLevel) return;

        ultraTotalAmount += amount;
        ultraTotalAmount = Mathf.Min(ultraTotalAmount, 4);
        m_ultraCurrentAmount += amount;

        if (m_ultraCurrentAmount >= 1)
        {
           
            ultraLevel++;
            uiManager.SetUltraLevelText(ultraLevel);
            uiManager.UpdateUltraFill(1);
            uiManager.AddUltraBar();
            uiManager.UpdateUltraFill(m_ultraCurrentAmount - 1);
            Debug.Log("ultraCurrentAmount : " + m_ultraCurrentAmount);
            m_ultraCurrentAmount -= 1;
        } else
        {
            uiManager.UpdateUltraFill(m_ultraCurrentAmount);
        }

        if (ultraLevel >= 1)
        {
            uiManager.ReadyUltraButton(true);
            if (!dangerMode)
                uiManager.SetReadyInteractable(true);
        }
    }

    // ULTRA MODE !!!!

    void ActivateUltraMode()
    {
        currentState = GameState.UltraMode;
        uiManager.ActivateUltraArcadeButton(true);
        uiManager.ActivateUltraReadyButton(false);
        StopCoroutine(patternRoutine);
        StopPlayerInput();
    }

    public void UltraEnded()
    {
        ultraTotalAmount = 0;
        ultraLevel = 0;
        uiManager.UltraMode(false);
        uiManager.SetReadyInteractable(false);
        uiManager.SetReadyCooldown();
        uiManager.ResetUltraBar();
        int totalDamage = ComboMultiplier() * numUltraHits;
        enemy.TakeHit(totalDamage);
        uiManager.SetDamageText(ComboMultiplier(), numUltraHits, totalDamage);
        numUltraHits = 0;
        StartPattern(startNumPatterns, true);
        //inputTimePerPrompt = StartInputTime;
        //promptDelay = StartPromptDelay;
    }

    int ComboMultiplier()
    {
        return currentComboLevel > 2 ? currentComboLevel : 1;
    }

    void GainCombo(float timeRatio)
    {
        int comboGained = Mathf.RoundToInt(Mathf.Lerp(MinComboGain, MaxComboGain, timeRatio));
        print("Combo Gained -- " + comboGained);
        //currentComboLevel += comboGained;

        currentComboLevel += 1;
        SetDamageModifier(currentComboLevel);
    }

    void SetDamageModifier(int _currentComboLevel){
        float m_damageModifier = 1 + (_currentComboLevel * m_comboDamageModifier);
        uiManager.SetDamageModifier(m_damageModifier);
    }

    void UpdateInputTime()
    {
        if (!RampInputTimes) return;
        inputTimePerPrompt -= inputTimeDelta * currentNumPatterns;
        inputTimePerPrompt = Mathf.Max(MinInputTime, inputTimePerPrompt);
    }

    void UpdatePromptDelay()
    {
        if (!RampPromptDelays) return;
        promptDelay -= promptDelayDelta * currentNumPatterns;
        promptDelay = Mathf.Max(MinPromptDelay, promptDelay);
    }

    public void GameOver(bool win)
    {
        currentState = GameState.GameOver;
        uiManager.SetGameOver(win);
        StopPlayerInput();
        StopCoroutine(patternRoutine);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
