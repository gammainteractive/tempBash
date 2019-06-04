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
    public void MainCharacterAttackRandom()
    {
        m_mainCharacterAnimationRef.PlayRandomQueued();
    }

    public void MissedInput()
    {
        m_mainCharacterAnimationRef.PlayQueued((int)MainCharacterAnimations.ANIMATIONS.HURT);
    }
}
