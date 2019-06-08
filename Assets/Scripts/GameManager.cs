using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    
    public bool dangerMode;
    public enum GameState { DisplayPattern, PlayerInput, UltraMode, GameOver}
    public GameState currentState;

    public SimonButton[] m_modeASimonButtons;
  

    public List<SimonButton> currentButtonPattern = new List<SimonButton>();
    int startNumPatterns = 1;
    public int currentNumPatterns;
    public float ButtonAlertTime;
    
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
        MODE_A,
        MODE_B,
        MODE_C,
    }

    [Header("Game Mode Parameters")]
    public int m_GameMode = 0;

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
    public float m_inputTimeModifierModeB = 4;
    float promptDelayDelta;
    Coroutine patternRoutine;
    Coroutine inputRoutine;
    [Space]

    public List<int> m_buttonPressesModeC = new List<int>();
    public float PromptToInputDelay = 0.5f;
    private float m_modeCDelayPerAnimation = 0.40f;
    private float m_modeCDelayPerAnimationAfterResults = 0.40f;
    private bool m_isDebugMode = true;
    private bool m_gameOverFlag = false;
    private int m_healthRegen = 10;

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

#if !UNITY_EDITOR
        m_isDebugMode = false;
#endif
        Application.targetFrameRate = 120;

        uiManager.TitleView();
        currentNumPatterns = startNumPatterns;
       
        inputTimePerPrompt = StartInputTime;
        promptDelay = StartPromptDelay;
        inputTimeDelta = (StartInputTime - MinInputTime) / InputsForMinInputTime;
        promptDelayDelta = (StartPromptDelay - MinPromptDelay) / InputsForMinPromptTime;
        if (!m_isDebugMode)
        {
            InputsForUltraLevel = 20;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (m_GameMode == (int)GAME_MODES.MODE_A || m_GameMode == (int)GAME_MODES.MODE_C)
        {
            GameModeAUpdate();
        }

        if (m_GameMode == (int)GAME_MODES.MODE_B
            && currentState != GameState.GameOver
            && currentState != GameState.UltraMode
            && !m_isDebugMode)
        {
            player.TakeHit(Time.deltaTime + 0.5f);
            if(player.currentHealth <= 0)
            {
                GameOver(false); return;
            }
        }
    }

    private void GameModeAUpdate() {
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

        if (dangerMode
            && currentState != GameState.GameOver)
        {
            m_currentHealthTimer += Time.deltaTime;
            if (m_currentHealthTimer >= m_healthTimer)
            {
                m_currentHealthTimer = 0;
                player.AddHealth(m_healthRegen);
                if (player.currentHealth >= 100)
                {
                    player.AddHealth(100);
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
            case GAME_MODES.MODE_A:
                m_GameMode = (int)GAME_MODES.MODE_A;
#if UNITY_EDITOR
                inputTimePerPrompt = 10000f;
#else
                inputTimePerPrompt = 1f;
#endif
                SetPlayerHealth(100);
                SetEnemyHealth(1000);
                SetEnemyDamage(50);
                uiManager.ShowGameModeView(UIManager.GAME_MODE_VIEW.MODE_A);
                break;

            case GAME_MODES.MODE_B:
                m_GameMode = (int)GAME_MODES.MODE_B;
                SetEnemyDamage(20);
                SetPlayerHealth(100);
                SetEnemyHealth(3000);
                //This is the delay after showing the pattern to press
                PromptToInputDelay /= 2.5f;
                //Time to do the inputs
                inputTimePerPrompt = 100f;
                //This is the delay after "Watch"
                m_displayPatternDelay = 0.20f;
                //Delay after while showing watch
                promptDelay = 0.15f;    //Default 1
                //Delay on how long the pattern lights up (3 buttons) - Default .3
                ButtonAlertTime = 0.15f;
                InitializeModeB();
                currentState = GameState.PlayerInput;
                uiManager.ShowGameModeView(UIManager.GAME_MODE_VIEW.MODE_B);
            break;
            case GAME_MODES.MODE_C:
                m_GameMode = (int)GAME_MODES.MODE_C;
#if UNITY_EDITOR
                inputTimePerPrompt = 100000f;
#else
                inputTimePerPrompt = 1f;
#endif
                //This is the delay after "Watch"
                m_displayPatternDelay = 1.20f;

                currentNumPatterns = 1;
                SetPlayerHealth(100);
                SetEnemyHealth(1000);
                SetEnemyDamage(50);
                m_healthRegen = 5;
                m_animationManager.m_mainCharacterAnimationRef.LastAnimationQueueEventHandle += EventProcessResults;
                uiManager.ShowGameModeView(UIManager.GAME_MODE_VIEW.MODE_A);
                break;
        }
    }

    private void InitializeModeB()
    {
        int m_random = 0;
        int m_currentIndex = 0;

        for (int j = 0; j < uiManager.m_buttonRowsModeB.Length; j++)
        {
            m_random = Random.Range(0, 3);
            for (int i = 0; i < 3; i++)
            {
                m_currentIndex = j * 3 + i;
                if (i == m_random)
                {
                    currentButtonPattern.Add(uiManager.m_simonButtonReference[m_random]);
                    uiManager.SetButtonProperty(m_currentIndex, m_random);
                }
                else
                {
                    uiManager.SetButtonProperty(m_currentIndex, (int)UIManager.SIMON_BUTTON_TYPES.INCORRECT);
                }
            }
        }

        for(int i = 0; i < 3; i++)
        {
            uiManager.m_buttonInteractionsModeB[i].SetProperties(uiManager.m_modeBSimonButtons[i + 3]);
        }


        //Remove the first index because we actually use the 2nd row as first input for the player
        currentButtonPattern.RemoveAt(0);
    }

    private void ModeBCorrectButton()
    {
        player.AddHealth(20);
       
        uiManager.ModeBMoveButtons();
        SetNextButtonProperties();
        //Set next row index specifically for the button rows (since the transform moves, we must also adjust this to do along with it)
        uiManager.IncrementButtonRowIndex();
        uiManager.SetInteractiveButtonProperties();

        //We must place the animation after incrementing buttonRow index because it is inside a coroutine
        uiManager.MoveAnimationButtons();
    }

    void SetNextButtonProperties()
    {
        //uiManager.SetButtonInteractableModeB();

        //Move to next pattern
        currentButtonPattern.RemoveAt(0);

        //Set button colors on the top row
        int m_random = Random.Range(0, 3);
        currentButtonPattern.Add(uiManager.m_simonButtonReference[m_random]);
        uiManager.SetNextButtonProperties(m_random);
    }

    void SetEnemyDamage(int _damage)
    {
        enemy.m_damage = _damage;
    }

    public void StartGame(int _gameMode)
    {
        StartGame();
        ResetGameParameters();
        ResetHealth();
        m_soundManager.PlayGameMusic();
       // m_soundManager.StopMusic();
        m_stage.SetActive(true);
        switch (_gameMode)
        {
            case (int)GAME_MODES.MODE_A:
                SetGameModeParameters(GAME_MODES.MODE_A);
                StartPattern(startNumPatterns, true);
            break;

            case (int)GAME_MODES.MODE_B:
                SetGameModeParameters(GAME_MODES.MODE_B);
                break;
            case (int)GAME_MODES.MODE_C:
                SetGameModeParameters(GAME_MODES.MODE_C);
                StartPattern(startNumPatterns, true);
                break;
        }
    }

    void ResetHealth()
    {
        player.currentHealth = player.maxHealth;
        enemy.currentHealth = enemy.maxHealth;
    }

    void ResetGameParameters()
    {
        startNumPatterns = 1;
        currentNumPatterns = startNumPatterns;
        m_ultraCurrentAmount = 0;
        m_animationManager.Reset();
        m_buttonPressesModeC.Clear();
        m_gameOverFlag = false;
        currentButtonPattern.Clear();
        uiManager.ResetUI(m_GameMode);
    }
    
    void StartPattern(int numPatterns, bool newPattern = false)
    {
        if (currentState == GameState.GameOver) return;
        SetButtonsInteractable(false);
        currentInputNum = 0;
        ResetInputTimer();
        currentNumPatterns = numPatterns;
        patternRoutine = StartCoroutine(PlaySimonPattern(numPatterns));
        if (newPattern)
        {
            SetButtonPattern();

        }
        else
        {
            AddButtonToPattern();
        }
    }

    IEnumerator PlaySimonPattern(int numPatterns)
    {
        currentState = GameState.DisplayPattern;
        if (m_GameMode == (int)GAME_MODES.MODE_C)
        {
            uiManager.ShowUIPhase((int)PhasesUI.UI_OBJECTS.MODE_C_WATCH);
        }
        else
        {
            uiManager.ActivatePromptText(true);
            uiManager.PromptPrompt(true);
        }
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
        if (m_GameMode == (int)GAME_MODES.MODE_C)
        {
            //uiManager.HideUIPhases();
            uiManager.ShowUIPhase((int)PhasesUI.UI_OBJECTS.MODE_C_REPEAT);
        }
        else
        {
            uiManager.PromptPrompt(false);
        }
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
    
    public void RetryLevel()
    {
        m_animationManager.m_mainCharacterAnimationRef.LastAnimationQueueEventHandle -= EventProcessResults;
        StartGame(m_GameMode);
    }

    void MissedInput()
    {
        player.TakeHit(enemy.m_damage);
       
        m_soundManager.PlayIncorrectSequence();

        if (dangerMode || player.currentHealth <= 0)
        {
            if (ultraLevel > 0)
            {
                ultraLevel = 0;
            }
            else
            {
                GameOver(false); return;
            }
        } else
        {
            m_isProcessResults = false;
            m_animationManager.MissedInput();
        }

        if(m_GameMode == (int)GAME_MODES.MODE_A || m_GameMode == (int)GAME_MODES.MODE_C) {
            uiManager.SetUltraButtonInteractable(false);
        }

        uiManager.ShowMiss();
        uiManager.SetReadyCooldown();
        uiManager.ActivateDamageModifier(false);
        uiManager.ActivateHitCombo(false);
        currentComboLevel = 0;
        //promptDelay = StartPromptDelay;
    
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

        if (ultraLevel > 0)
        {
            ultraLevel--;
        }

        if (m_GameMode == (int)GAME_MODES.MODE_A || m_GameMode == (int)GAME_MODES.MODE_C)
        {
            if (m_GameMode == (int)GAME_MODES.MODE_C) {
                m_buttonPressesModeC.Clear();
                currentState = GameState.DisplayPattern;
                StartCoroutine(StartPatternWithDelay(startNumPatterns, true, 4));
            }
            else
            {
                StartPattern(startNumPatterns, true);
            }

            currentNumPatterns = startNumPatterns;
            dangerMode = true;
            
            if (ultraLevel > 0)
            {
                uiManager.SetReadyCooldown();
            }
            else
            {
                uiManager.UltraTextSetActive(false);
            }
        }
        
        uiManager.SetUltraLevelText(ultraLevel);
    }

    void StopPlayerInput()
    {
        SetButtonsInteractable(false);
        currentInputNum = 0;
    }

    bool HitCorrectButton(int _buttonValue)
    {
        if (currentButtonPattern[currentInputNum].m_buttonValue == _buttonValue
            || _buttonValue == 5)
        {
            return true;
        } else
        {
            return false;
        }
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
        return m_modeASimonButtons[Random.Range(0, m_modeASimonButtons.Length)];
    }

    public bool SimonButtonHit(int _buttonValue)
    {
        if (currentState == GameState.PlayerInput)
        {
            bool correct = HitCorrectButton(_buttonValue);

            if (m_GameMode == (int)GAME_MODES.MODE_C)
            {
                if (correct)
                {
                    float ultraMultiplier = (float)1 / InputsForUltraLevel;
                    AddUltra(ultraMultiplier);
                    m_buttonPressesModeC.Add(_buttonValue);
                } else
                {
                    m_buttonPressesModeC.Add(-1);
                }

                CheckModeCButtonHit(_buttonValue);
                return true;
            }

          
            // Hit Correct Button
            if (correct)
            {
                enemy.TakeHit(ComboMultiplier());
                m_soundManager.PlayCorrectButton();

                if (enemy.currentHealth <= 0)
                {
                    GameOver(true);
                } else if(_buttonValue == 5)    //This is when Ultra mode ends on mode B
                {
                    MainCharacterAttackRandom();
                } else
                {
                    MainCharacterAttack(_buttonValue);
                }
                uiManager.HitDamage(ComboMultiplier());
                GainCombo(currentInputTime/inputTimePerPrompt);
                if (currentComboLevel >= 3)
                {
                    uiManager.ActivateHitCombo(true);
                    uiManager.UpdateHitCombo(currentComboLevel);
                }
              
                /* if(currentInputNum > 3 
                     && currentComboLevel > 3)
                 {
                     m_cameraManager.ZoomIn();
                 }*/
                float ultraMultiplier = (float)1 / InputsForUltraLevel;
                float ultraGained = (currentInputTime / inputTimePerPrompt) * ultraMultiplier;

                if (m_GameMode == (int)GAME_MODES.MODE_A)
                {
                    ModeACorrectButton();
                } else if (m_GameMode == (int)GAME_MODES.MODE_B)
                {
                    ultraGained = ultraMultiplier;
                    ModeBCorrectButton();
                }

                AddUltra(ultraGained);
            } else
            {
                // Incorrect button response
                MissedInput();
            }
            return correct;
        }
        else if (currentState == GameState.UltraMode  && m_GameMode == (int)GAME_MODES.MODE_B)  //Mode A Ultra button is on UltraButtonHit
        {
            // MainCharacterAttackRandom();
            MainCharacterAttackRandomUltra();
            ModeBCorrectButton();
            numUltraHits++;
            return true;
        }
        return false;
    }

    private void ModeACorrectButton()
    {
        currentInputNum++;
        Debug.Log("Current input num: " + currentInputNum + " current num patterns : " + currentNumPatterns);
        if (currentInputNum >= currentNumPatterns)
        {
             m_soundManager.PlayCorrectSequence();
            // Correctly entered Sequence
            // m_cameraManager.ZoomOut();
            UpdateInputTime();
            UpdatePromptDelay();
            StartPattern(currentNumPatterns + 1);
            uiManager.ActivateTimerBar(false);
        }
        else
        {
            ResetInputTimer();
        }
    }

    private void CheckModeCButtonHit(int _buttonValue)
    {
        currentInputNum++;
        if (currentInputNum >= currentButtonPattern.Count)
        {
            currentState = GameState.DisplayPattern;
            SetButtonsInteractable(false);
            bool m_success = true;
            for(int i = 0; i < currentButtonPattern.Count; i++)
            {
                if(m_buttonPressesModeC[i] != currentButtonPattern[i].m_buttonValue)
                {
                    m_success = false;
                }
            }
            if (m_success)
            {
                StartCoroutine(ProcessResults(true));
            } else
            {
                StartCoroutine(ProcessResults(false));
            }
        }
        else
        {
            ResetInputTimer();
        }
    }

    bool m_isProcessResults = true;

    private IEnumerator ProcessResults(bool _success)
    {
        
        uiManager.ActivateTimerBar(false);
        if (_success)
        {
           // m_isProcessResults = true;
            m_soundManager.PlayCorrectSequence();
            yield return StartCoroutine(ShowResults(true));
            // Correctly entered Sequence
            //  enemy.TakeHit(ComboMultiplier() * currentInputNum);
            for (int i = 0; i < currentInputNum; i++)
            {
                MainCharacterAttack(m_buttonPressesModeC[i]);
                if(i == currentInputNum - 1)
                {
                    m_isProcessResults = true;
                }
                //  m_animationManager.MainCharacterPlayQueued(m_buttonPressesModeC[0]);
                //  m_soundManager.MainCharacterAttackTypePlaySound(currentButtonPattern[i].m_buttonValue);
                enemy.TakeHit(ComboMultiplier());
                uiManager.HitDamage(ComboMultiplier());

                GainCombo(currentInputTime / inputTimePerPrompt);
                if (currentComboLevel >= 3)
                {
                    uiManager.ActivateHitCombo(true);
                    uiManager.UpdateHitCombo(currentComboLevel);
                }

                float ultraMultiplier = (float)1 / InputsForUltraLevel;
                AddUltra(ultraMultiplier);

                yield return new WaitForSeconds(m_modeCDelayPerAnimation);  //This is used for the sound to not be played all at once
            }
        } else
        {
            yield return StartCoroutine(ShowResults(false));
            MissedInput();
        }
        m_buttonPressesModeC.Clear();
    }

    private void EventProcessResults()
    {
        if (m_GameMode == (int)GAME_MODES.MODE_C && m_isProcessResults)
        {
            m_isProcessResults = false;
            StartCoroutine(IProcessResults());
        }
    }

    private IEnumerator IProcessResults()
    {
        yield return new WaitForSeconds(1);
        UpdateInputTime();
        UpdatePromptDelay();
        yield return StartCoroutine(StartPatternWithDelay(currentNumPatterns + 1, false));
    }

    private IEnumerator ShowResults(bool _isSuccess, float _delay = 2)
    {
        uiManager.ShowModeCResult(_isSuccess);
        yield return new WaitForSeconds(_delay);
        uiManager.HideUIPhases();
    }

    private IEnumerator StartPatternWithDelay(int _currentNumPatterns, bool _isNewPattern, float delay = 0)
    {
        yield return new WaitForSeconds(delay * m_modeCDelayPerAnimationAfterResults);
        StartPattern(_currentNumPatterns, _isNewPattern);
    }

    void SetButtonsInteractable(bool interact)
    {
        foreach (var button in m_modeASimonButtons)
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
        if (currentState == GameState.PlayerInput)
        {
            if (ultraLevel > 0)
            {
                if (currentState != GameState.UltraMode)
                {
                    Debug.Log("Ultra Button hit");
                    uiManager.UltraMode(true);
                    ActivateUltraMode();
                    uiManager.UltraEffectsToggle(true);
                }
            }
        } else if(currentState == GameState.UltraMode)
        {
            MainCharacterAttackRandomUltra();
            numUltraHits++;
        }
    }

    int m_animationIndexReference = 10;

    public void MainCharacterAttackRandomUltra()
    {
        int m_random = Random.Range(0, 4);
        m_animationManager.MainCharacterAttackUltra(m_random);
        m_soundManager.MainCharacterAttackUltra(m_random);
    }

    private void MainCharacterAttackRandom()
    {
        int m_randomButtonValueNumber = UnityEngine.Random.Range(0, 2);
        m_animationManager.MainCharacterAttackTypePlayQueued(m_randomButtonValueNumber);
        m_soundManager.MainCharacterAttackTypePlaySound(m_randomButtonValueNumber);
    }

    private void MainCharacterAttack(int _buttonValue)
    {
        m_animationManager.MainCharacterAttackTypePlayQueued(_buttonValue);
        m_soundManager.MainCharacterAttackTypePlaySound(_buttonValue);
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
                m_ultraCurrentAmount -= 1;
            }
            
        } else
        {
            uiManager.UpdateUltraFill(m_ultraCurrentAmount);
        }

        if (ultraLevel >= 1)
        {
            if (!dangerMode
                && m_GameMode == (int)GAME_MODES.MODE_A)
            {
                uiManager.SetUltraButtonInteractable(true);
            } else if(m_GameMode == (int)GAME_MODES.MODE_B)
            {
                Debug.Log("Ultra mode B");
                uiManager.UltraMode(true);
                ActivateUltraMode();
            }
        }
    }

    private void DeactivateUltraModeB()
    {
        uiManager.ToggleUltraModeB(false);
    }

    // ULTRA MODE !!!!
    void ActivateUltraMode()
    {
        currentState = GameState.UltraMode;
        if (m_GameMode == (int)GAME_MODES.MODE_A)
        {
            StopCoroutine(patternRoutine);
            StopPlayerInput();
        } else if(m_GameMode == (int)GAME_MODES.MODE_B)
        {
            uiManager.ToggleUltraModeB(true);
        } else if(m_GameMode == (int)GAME_MODES.MODE_C)
        {
            uiManager.HideUIPhases();
        }
    }

    private IEnumerator IUltraEnded()
    {
        uiManager.ShowUltraDamage(true);
        currentState = GameState.DisplayPattern;
        yield return new WaitForSeconds(3.0f);
        uiManager.ShowUltraDamage(false);
        m_isProcessResults = true;
        EventProcessResults();
    }

    public void UltraEnded()
    {
        ultraTotalAmount = 0;
        ultraLevel = 0;
        uiManager.UltraMode(false);
        uiManager.UltraTextSetActive(false);
        uiManager.ResetUltraBar();
        int totalDamage = ComboMultiplier() * numUltraHits;
        enemy.TakeHit(totalDamage);
        if (enemy.currentHealth <= 0)
        {
            if (m_GameMode == (int)GAME_MODES.MODE_C)
            {
                 m_animationManager.StopUltraAnimations();
            }

            GameOver(true);
        } else if(m_GameMode == (int)GAME_MODES.MODE_C)
        {
            uiManager.UltraEffectsToggle(false);
            m_animationManager.StopUltraAnimations();
            m_buttonPressesModeC.Clear();
            StartCoroutine(IUltraEnded());
        }
        uiManager.SetDamageText(ComboMultiplier(), numUltraHits, totalDamage);
        numUltraHits = 0;

        if (m_GameMode == (int)GAME_MODES.MODE_A)
        {
            uiManager.SetUltraButtonInteractable(false);
            uiManager.SetReadyCooldown();
            StartPattern(startNumPatterns, true);
        } else if (m_GameMode == (int)GAME_MODES.MODE_B)
        {
            DeactivateUltraModeB();
            currentState = GameState.PlayerInput;
        }
       
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
        //Added gameover flag due to multiple calls on spam
        if (m_gameOverFlag)
        {
            return;
        } else
        {
            m_gameOverFlag = true;
        }
        EndGame();
        //m_cameraManager.ZoomOut();
        currentState = GameState.GameOver;
        uiManager.SetGameOver(win);
        m_animationManager.GameOverAnimations(win);
        if (m_GameMode == (int)GAME_MODES.MODE_A)
        {
            uiManager.ActivatePromptText(false);
            StopCoroutine(patternRoutine);
        }
        StopPlayerInput();
        
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
