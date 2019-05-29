using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    
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
    public SoundManager m_soundManager;
    public UIManager uiManager;
    public CameraManager m_cameraManager;
    public Player player;
    public Enemy enemy;
    public AnimationManager m_animationManager;

    private float m_healthTimer = 1;
    public float m_currentHealthTimer = 0;
    public GameObject m_stage;

    public enum GAME_MODES
    {
        MEMORY,
        REACTION
    }

    [Header("Game Mode Parameters")]
    public int m_GameMode = 0;
    public int m_CurrentNumberOfMiss = 0;
    public int m_MaxNumberOfMiss = 2;

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
    public float m_ultraCurrentAmount = 0;
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
    public float m_displayPatternDelay = 0.5f;
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

    #region StartEndGameHandlers

    public delegate void StartGameHandler();

    public event StartGameHandler StartGameHandle;

    public void StartGame()
    {
        if (StartGameHandle != null)
        {
            StartGameHandle.Invoke();
        }
    }

    public delegate void EndGameHandler();

    public event EndGameHandler EndGameHandle;

    public void EndGame()
    {
        if (EndGameHandle != null)
        {
            EndGameHandle.Invoke();
        }
    }

    #endregion

    void Awake()
    {
        instance = this;
    }


    // Use this for initialization
    void Start () {

        uiManager.TitleView();
        currentNumPatterns = startNumPatterns;
       
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

        if (dangerMode && m_GameMode == (int)GAME_MODES.MEMORY
            && currentState != GameState.GameOver)
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

    private void SetEnemyHealth(int _value)
    {
        enemy.SetHealth(_value);
        uiManager.UpdateEnemyHealthText(_value);
    }

    private void SetPlayerHealth(int _value)
    {
        player.SetHealth(_value);
        uiManager.UpdatePlayerHealthText(_value);
    }

    void SetGameModeParameters(GAME_MODES _mode)
    {
        switch (_mode)
        {
            case GAME_MODES.MEMORY:
                m_GameMode = (int)GAME_MODES.MEMORY;
                m_MaxNumberOfMiss = 2;
                SetPlayerHealth(100);
                SetEnemyHealth(1000);
                SetEnemyDamage(50);

            break;

            case GAME_MODES.REACTION:
                Debug.Log("Reaction Game");
                m_GameMode = (int)GAME_MODES.REACTION;
                m_MaxNumberOfMiss = 5;
                SetPlayerHealth(100);
                SetEnemyHealth(3000);
                SetEnemyDamage(20);
                //This is the delay after showing the pattern to press
                PromptToInputDelay /= 2.5f;
                //This is the delay after "Watch"
                m_displayPatternDelay = 0.20f;
                //Delay after while showing watch
                promptDelay = 0.15f;    //Default 1
                //Delay on how long the pattern lights up (3 buttons) - Default .3
                ButtonAlertTime = 0.15f;
            break;
        }
    }

    void SetEnemyDamage(int _damage)
    {
        enemy.m_damage = _damage;
    }

    public void StartGame(int _gameMode)
    {
        StartGame();
        m_soundManager.StopMusic();
        m_stage.SetActive(true);
        switch (_gameMode)
        {
            case (int)GAME_MODES.MEMORY:
                SetGameModeParameters(GAME_MODES.MEMORY);
                StartPattern(startNumPatterns, true);
            break;

            case (int)GAME_MODES.REACTION:
                SetGameModeParameters(GAME_MODES.REACTION);
                StartPattern(startNumPatterns, true);
                break;
        }
    }

    void ResetGameParameters()
    {
        currentInputNum = 0;
        ResetInputTimer();
    }
    
    void StartPattern(int numPatterns, bool newPattern = false)
    {
        if (currentState == GameState.GameOver) return;
        SetButtonsInteractable(false);
        ResetGameParameters();
        currentNumPatterns = numPatterns;
        patternRoutine = StartCoroutine(PlaySimonPattern(numPatterns));
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
        yield return new WaitForSeconds(m_displayPatternDelay);
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
        player.TakeHit(enemy.m_damage);

        if (dangerMode || player.currentHealth <= 0)
        {
            GameOver(false); return;
        }

        uiManager.SetUltraButtonInteractable(false);
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

        if (m_GameMode == (int)GAME_MODES.MEMORY)
        {
            dangerMode = true;
        }

        //uiManager.SetReadyInteractable(false);
        //uiManager.SetReadyCooldown();
        if (ultraLevel > 0) ultraLevel--;
        if (ultraLevel > 0)
        {
            uiManager.SetReadyCooldown();
        }
        else
        {
            uiManager.UltraTextSetActive(false);
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

    private AnimationManager.ANIMATIONS GetAnimationFromButton(SimonButton _button)
    {
        if (simonButtons[0] == _button)
        {
            return AnimationManager.ANIMATIONS.PUNCH_UP;//Punch Attacks
        }
        else if (simonButtons[1] == _button)
        {
            return AnimationManager.ANIMATIONS.KICK;//Special Attacks
        }
        else if (simonButtons[2] == _button)
        {
            return AnimationManager.ANIMATIONS.SPECIAL;//Kick Attacks
        }
        else
        {
            throw new UnityException("Button that was pressed doesn't exist");
        }

    }

    public bool SimonButtonHit(SimonButton button)
    {
        if (currentState == GameState.PlayerInput)
        {
            bool correct = HitCorrectButton(button);
            print(correct);
            if (correct)
            {
                // Hit Correct Button
               
                m_animationManager.PlayQueued(GetAnimationFromButton(button));
                
                enemy.TakeHit(ComboMultiplier());
                uiManager.HitDamage(ComboMultiplier());
                currentInputNum++;
                GainCombo(currentInputTime/inputTimePerPrompt);
                if (currentComboLevel >= 3)
                {
                    uiManager.ActivateHitCombo(true);
                }
               /* if(currentInputNum > 3 
                    && currentComboLevel > 3)
                {
                    m_cameraManager.ZoomIn();
                }*/
                uiManager.UpdateHitCombo(currentComboLevel);

                float ultraMultiplier = (float)1 / InputsForUltraLevel;
                float ultraGained = (currentInputTime / inputTimePerPrompt) * ultraMultiplier;
                AddUltra(ultraGained);

                if (currentInputNum >= currentNumPatterns)
                {
                    // Correctly entered Sequence
                   // m_cameraManager.ZoomOut();
                    UpdateInputTime();
                    UpdatePromptDelay();
                    if (m_GameMode == (int)GAME_MODES.MEMORY)
                    {
                        StartPattern(currentNumPatterns + 1);
                    } else
                    {
                        SetButtonPattern();
                        StartPattern(startNumPatterns);
                    }
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
            return correct;
        }
        return false;
    }

    void SetButtonsInteractable(bool interact)
    {
        foreach (var button in simonButtons)
        {
            button.SetButtonInteractable(interact);
        }
    }

    // Ultra Level Amounts

   /* public void UltraReadyButtonHit()
    {
        uiManager.UltraMode(true);
        ActivateUltraMode();
    }*/

    public void UltraButtonHit()
    {
        if (currentState != GameState.UltraMode)
        {
            uiManager.UltraMode(true);
            ActivateUltraMode();
        }
        else
        {
            numUltraHits++;
        }
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
            if (ultraLevel >= MaxUltraLevel)
            {
                uiManager.UpdateUltraFill(1);
            }
            else
            {
                uiManager.UpdateUltraFill(1);
                uiManager.AddUltraBar();
                uiManager.UpdateUltraFill(m_ultraCurrentAmount - 1);
                Debug.Log("ultraCurrentAmount : " + m_ultraCurrentAmount);
                m_ultraCurrentAmount -= 1;
            }
            
        } else
        {
            uiManager.UpdateUltraFill(m_ultraCurrentAmount);
        }

        if (ultraLevel >= 1)
        {
            if (!dangerMode)
                uiManager.SetUltraButtonInteractable(true);
        }
    }

    // ULTRA MODE !!!!

    void ActivateUltraMode()
    {
        currentState = GameState.UltraMode;
        StopCoroutine(patternRoutine);
        StopPlayerInput();
    }

    public void UltraEnded()
    {
        ultraTotalAmount = 0;
        ultraLevel = 0;
        uiManager.UltraMode(false);
        uiManager.UltraTextSetActive(false);
        uiManager.SetUltraButtonInteractable(false);
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
        EndGame();
        m_cameraManager.ZoomOut();
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
