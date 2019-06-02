using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    
    public bool dangerMode;
    public enum GameState { DisplayPattern, PlayerInput, UltraMode, GameOver}
    public GameState currentState;

    public enum SIMON_BUTTON_TYPES {
        PUNCH, //Purple
        KICK,   //Blue
        SPECIAL,    //Green
        INCORRECT   //Black
    }
    public List<SimonButton> m_simonButtonReference;

    public SimonButton[] m_modeASimonButtons;
    public Transform m_modeBgameObject;
    public SimonButton[] m_modeBSimonButtons;

    public List<SimonButton> currentButtonPattern = new List<SimonButton>();
    public SimonButton[] inputButtonPattern;
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
        MODE_B
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

    private int m_buttonRowsModeB = 6;

    public float PromptToInputDelay = 0.5f;

    #region StartEndGameHandlers

    public delegate void StartGameHandler();

    public event StartGameHandler StartGameHandle;

    public int m_buttonRowIndex = 0;
    private bool m_isDebugMode = true;
    private bool m_gameOverFlag = false;

    public void StartGame()
    {
#if !UNITY_EDITOR
        m_isDebugMode = false;
#endif
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

        if (m_GameMode == (int)GAME_MODES.MODE_A)
        {
            GameModeAUpdate();
        }

        if (m_GameMode == (int)GAME_MODES.MODE_B
            && currentState != GameState.GameOver
            && currentState != GameState.UltraMode
            && !m_isDebugMode)
        {
            player.TakeHit(Time.deltaTime + 0.5f);
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
            case GAME_MODES.MODE_A:
                m_GameMode = (int)GAME_MODES.MODE_A;
                inputTimePerPrompt = 1f;
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
        }
    }

    private void InitializeModeB()
    {
        m_modeBSimonButtons = m_modeBgameObject.GetComponentsInChildren<SimonButton>(true);

        int m_random = 0;
        for (int j = 0; j < m_buttonRowsModeB; j++) {
            m_random = Random.Range(0, 3); 
            for (int i = 0; i < 3; i++)
            {
                if (i == m_random)
                {
                    currentButtonPattern.Add(m_simonButtonReference[m_random]);
                    m_modeBSimonButtons[j * 3 + i].SetProperties(m_simonButtonReference[m_random]);
                } else
                {
                    m_modeBSimonButtons[j * 3 + i].SetProperties(m_simonButtonReference[(int)SIMON_BUTTON_TYPES.INCORRECT]);
                }
            }
        }

        //Remove the first index because we actually use the 2nd row as first input for the player
        currentButtonPattern.RemoveAt(0);
    }

    private void ModeBCorrectButton()
    {
        player.AddHealth(20);
        SetNextButtonProperties();
        uiManager.ModeBMoveButtons();

        //Set next row index specifically for the button rows (since the transform moves, we must also adjust this to do along with it)
        m_buttonRowIndex++; 
        if (m_buttonRowIndex > 5)
        {
            m_buttonRowIndex = 0;
        }

        //We must place the animation after incrementing buttonRow index because it is inside a coroutine
        uiManager.MoveAnimationButtons();
    }

    private void SetButtonInteractable()
    {
        //Disable current button interactable and enable on the next row
        if (m_buttonRowIndex + 1 == m_buttonRowsModeB)
        {
            uiManager.SetButtonRowInteractive(0);
        } else
        {
            uiManager.SetButtonRowInteractive(m_buttonRowIndex + 1);
        }
        //uiManager.m_startButtonAnimationModeB = true;
     
    }

    void SetNextButtonProperties()
    {
        SetButtonInteractable();
        //Move to next pattern
        currentButtonPattern.RemoveAt(0);

        //Set button colors on the top row
        int m_random = Random.Range(0, 3);

        currentButtonPattern.Add(m_simonButtonReference[m_random]);
        int m_startIndexOnTopRow = m_buttonRowIndex * 3;
        int m_buttonTarget = (m_startIndexOnTopRow + m_random);

        for (int i = m_startIndexOnTopRow; i < m_startIndexOnTopRow + 3; i++)    //Last row of buttons
        {
            if (currentState == GameState.UltraMode)
            {
                m_modeBSimonButtons[i].SetProperties(m_simonButtonReference[i - m_startIndexOnTopRow]); 
            }
            else
            {
                if (i == (m_buttonTarget))
                {
                    m_modeBSimonButtons[i].SetProperties(m_simonButtonReference[m_random]);
                }
                else
                {
                    m_modeBSimonButtons[i].SetProperties(m_simonButtonReference[(int)SIMON_BUTTON_TYPES.INCORRECT]);
                }
            }
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
            case (int)GAME_MODES.MODE_A:
                SetGameModeParameters(GAME_MODES.MODE_A);
                StartPattern(startNumPatterns, true);
            break;

            case (int)GAME_MODES.MODE_B:
                SetGameModeParameters(GAME_MODES.MODE_B);
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
        } else
        {
            m_animationManager.MissedInput();
        }

        if(m_GameMode == (int)GAME_MODES.MODE_A) {
            uiManager.SetUltraButtonInteractable(false);
        }
       
        uiManager.SetReadyCooldown();
        uiManager.ActivateHitCombo(false);
        uiManager.ActivateDamageModifier(false);
        uiManager.UpdateHitCombo(1);
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

        if (m_GameMode == (int)GAME_MODES.MODE_A)
        {
            StartPattern(startNumPatterns, true);
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

    private int GetAnimationFromButton(int _buttonValue)
    {
        return (_buttonValue + 2);
    }

    public bool SimonButtonHit(int _buttonValue)
    {
        if (currentState == GameState.PlayerInput)
        {
            bool correct = HitCorrectButton(_buttonValue);
            // Hit Correct Button
            if (correct)
            {
                enemy.TakeHit(ComboMultiplier());
                if(enemy.currentHealth <= 0)
                {
                    GameOver(true);
                } else
                {
                    m_animationManager.MainCharacterPlayQueued(GetAnimationFromButton(_buttonValue));
                }
                uiManager.HitDamage(ComboMultiplier());
                GainCombo(currentInputTime/inputTimePerPrompt);
                if (currentComboLevel >= 3)
                {
                    uiManager.ActivateHitCombo(true);
                }
                uiManager.UpdateHitCombo(currentComboLevel);

                float ultraMultiplier = (float)1 / InputsForUltraLevel;
                float ultraGained = (currentInputTime / inputTimePerPrompt) * ultraMultiplier;
              

                /* if(currentInputNum > 3 
                     && currentComboLevel > 3)
                 {
                     m_cameraManager.ZoomIn();
                 }*/
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
        else if (currentState == GameState.UltraMode)
        {
            ModeBCorrectButton();
            numUltraHits++;
            return true;
        }
        return false;
    }

    private void ModeACorrectButton()
    {
        currentInputNum++;
        if (currentInputNum >= currentNumPatterns)
        {
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
            if (!dangerMode
                && m_GameMode == (int)GAME_MODES.MODE_A)
            {
                uiManager.SetUltraButtonInteractable(true);
            } else if(m_GameMode == (int)GAME_MODES.MODE_B)
            {
                UltraActivateModeB();
                uiManager.UltraMode(true);
                ActivateUltraMode();
            }
        }
    }

    private void UltraActivateModeB()
    {
      //  m_ultraModeB = new SimonButton[m_modeBSimonButtons.Length];
        for (int j = 0; j < m_buttonRowsModeB; j++)
        {
            for (int i = 0; i < 3; i++)
            {
               // m_ultraModeB[j * 3 + i] = new SimonButton(m_modeBSimonButtons[j * 3 + i]);
                m_modeBSimonButtons[j * 3 + i].SetProperties(m_simonButtonReference[i]);
            }
        }
    }

    private void DeactivateUltraModeB()
    {
        for (int j = 0; j < m_buttonRowsModeB; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                m_modeBSimonButtons[j * 3 + i].SetButtonValue(5);
            }
        }
    }

    // ULTRA MODE !!!!
    void ActivateUltraMode()
    {
        currentState = GameState.UltraMode;
        if (m_GameMode == (int)GAME_MODES.MODE_A)
        {
            StopCoroutine(patternRoutine);
            StopPlayerInput();
        }
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
}
