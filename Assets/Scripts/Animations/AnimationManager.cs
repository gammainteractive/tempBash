using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {


    public MainCharacterAnimations m_mainCharacterAnimationRef;
    public CarAnimations m_carAnimationRef;
    public bool m_isQueueAnimations = false;


    [Header("Animation Speed")]
    public int m_fps = 20;

    
	void Start () {
        m_mainCharacterAnimationRef.ChangeAnimationFPS(m_fps);

#if UNITY_EDITOR
       // StartCoroutine(LateStart());
#endif
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        UIManager.Instance.GameView();
        yield return new WaitForEndOfFrame();
        //PlayQueued(ANIMATIONS.IDLE);
        //GameManager.instance.StartGame(0);
        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_mainCharacterAnimationRef.GameOver();
            // PlayContinuously(ANIMATIONS.IDLE);
            // m_mainCharacterAnimationRef.PlayQueued((int)MainCharacterAnimations.ANIMATIONS.PUNCH2
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //m_mainCharacterAnimationRef.PlayQueued((int)MainCharacterAnimations.ANIMATIONS.KICK2);
            m_mainCharacterAnimationRef.PlayRandomQueued();
        }
    }

    public void GameOverAnimations(bool _isWin)
    {
        if (_isWin)
        {
            m_carAnimationRef.GameOver();
        } else
        {
            m_mainCharacterAnimationRef.GameOver();
        }
    }

    public void MainCharacterPlayQueued(int _animation)
    {
        m_mainCharacterAnimationRef.PlayQueued(_animation);
    }

    public void MissedInput()
    {
        m_mainCharacterAnimationRef.PlayQueued((int)MainCharacterAnimations.ANIMATIONS.HURT);
    }
}
