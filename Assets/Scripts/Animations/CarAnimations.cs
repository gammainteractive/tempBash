using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimations : AnimationActions
{
    public enum ANIMATIONS
    {
        NONE,
        DEATH,
        IDLE_0,
        ATTACK_0,
        IDLE_1,
        ATTACK_1,
        DAMAGED_1,
        IDLE_2,
        ATTACK_2,
        DAMAGED_2,
        IDLE_3,
        ATTACK_3,
        DAMAGED_3,

    }

    public enum HEALTH_STATE
    {
        STATE_0,
        STATE_1,
        STATE_2,
        STATE_3,
        STATE_4,
    }

    public bool m_isOnIdleAnimation = false;
    private bool m_disableIdleAnimation = false;
    private int m_healthState = 0;
    private Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponent<Enemy>();
        _enemy.EnemyTakeHitHandle += HealthCarStateAnimation;
    }

    // Update is called once per frame
    void Update () {

        if (base.m_queuedAnimations.Count == 0
           && !base.IsPlaying()
           && !m_isOnIdleAnimation
           && !m_disableIdleAnimation)
        {
            PlayIdleOnRepeat();
        }
    }

    private void HealthCarStateAnimation()
    {
        int m_healthPercent = _enemy.m_currentHealthPercent;
        if(m_healthPercent <= 99 && m_healthPercent >= 70)
        {
            m_healthState = (int)HEALTH_STATE.STATE_1;
        } else if (m_healthPercent <= 69 && m_healthPercent >= 40)
        {
            m_healthState = (int)HEALTH_STATE.STATE_2;
        }
        else if (m_healthPercent <= 39) {
            m_healthState = (int)HEALTH_STATE.STATE_3;
        }
        DamagedAnimation();
    }

    private void DamagedAnimation()
    {
        switch (m_healthState)
        {
            case ((int)HEALTH_STATE.STATE_0):
                PlayQueued((int)ANIMATIONS.DAMAGED_1);
                break;
            case ((int)HEALTH_STATE.STATE_1):
                PlayQueued((int)ANIMATIONS.DAMAGED_1);
                break;
            case ((int)HEALTH_STATE.STATE_2):
                PlayQueued((int)ANIMATIONS.DAMAGED_2);
                break;
            case ((int)HEALTH_STATE.STATE_3):
                PlayQueued((int)ANIMATIONS.DAMAGED_3);
                break;
        }
        m_isOnIdleAnimation = false;
    }

    private int GetCurrentIdleAnimation()
    {
        switch (m_healthState)
        {
            case ((int)HEALTH_STATE.STATE_0):
                return (int)ANIMATIONS.IDLE_0;
            case ((int)HEALTH_STATE.STATE_1):
                return (int)ANIMATIONS.IDLE_1;
            case ((int)HEALTH_STATE.STATE_2):
                return (int)ANIMATIONS.IDLE_2;
            case ((int)HEALTH_STATE.STATE_3):
                return (int)ANIMATIONS.IDLE_3;
        }
        return 0;
    }

    private void PlayIdleOnRepeat()
    {
        m_currentAnimation = GetCurrentIdleAnimation();
        base.PlayOnce = false;
        m_isOnIdleAnimation = true;
        ChangeMaterial(m_animations[m_currentAnimation]);
        base.Play();
    }

    public void GameOver()
    {
        StartCoroutine(IGameOver());
    }

    private IEnumerator IGameOver()
    {
        yield return new WaitForSeconds(0.4f);
        m_disableIdleAnimation = true;
        yield return new WaitForSeconds(0.2f);
        PlayQueued((int)ANIMATIONS.DEATH);
    }
    
}
