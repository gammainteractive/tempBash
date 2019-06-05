using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {


    public MainCharacterAnimations m_mainCharacterAnimationRef;
    public CarAnimations m_carAnimationRef;
    public bool m_isQueueAnimations = false;

    public void GameOverAnimations(bool _isWin)
    {
        StopAllCoroutines();
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

    //Based off the 3 buttons, do a similar attack type
    public void MainCharacterAttackTypePlayQueued(int _attackType)
    {
        m_mainCharacterAnimationRef.AttackTypePlayQueued(_attackType);
    }

    public void MissedInput()
    {
        StartCoroutine(IMissedInput());
    }

    private IEnumerator IMissedInput()
    {
        m_carAnimationRef.AttackAnimation();
        yield return new WaitForSeconds(0.43f);
        m_mainCharacterAnimationRef.PlayPriority((int)MainCharacterAnimations.ANIMATIONS.HURT);
    }
}
