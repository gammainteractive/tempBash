using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterAnimations : AnimationActions {
    
    public enum ANIMATIONS
    {
        NONE,
        IDLE,
        PUNCH,
        KICK,
        SPECIAL,
        DEATH,
        HURT,
        KICK2,
        LOW_KICK,
        PUNCH2,
    }

    //Attack types are based on button values
    public enum ATTACK_TYPES
    {
        PUNCH,
        KICK,
        SPECIAL,
    }

    public bool m_isOnIdleAnimation = false;
    private bool m_disableIdleAnimation = false;

    private void Update()
    {
        if (base.m_queuedAnimations.Count == 0
            && !base.IsPlaying()
            && !m_isOnIdleAnimation
            && !m_disableIdleAnimation)
        {
            PlayIdleOnRepeat();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayQueued(3);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.instance.GameOver(false);
        }
    }

    //Button value or attack types depend on the button value definition on UIManager.
    public void AttackTypePlayQueued(int _attackType)
    {
        StopIdleAnimation();
        int m_animation = (int)GetAnimationFromAttackType(_attackType);
        base.PlayQueued(m_animation);
    }

    public ANIMATIONS GetAnimationFromAttackType(int _attackType)
    {
        int m_random = 0;
        switch (_attackType)
        {
            case ((int)ATTACK_TYPES.KICK):
                m_random = Random.Range(0, 3);
                if (m_random == 0)
                {
                    return ANIMATIONS.KICK;
                } else if(m_random == 1)
                {
                    return ANIMATIONS.KICK2;
                } else 
                {
                    return ANIMATIONS.LOW_KICK;
                }
            case ((int)ATTACK_TYPES.PUNCH):
                m_random = Random.Range(0, 2);
                if (m_random == 0)
                {
                    return ANIMATIONS.PUNCH;
                }
                else
                {
                    return ANIMATIONS.PUNCH2;
                }
            case ((int)ATTACK_TYPES.SPECIAL):
                return ANIMATIONS.SPECIAL;
        }
        return ANIMATIONS.IDLE;
    }

    public override void PlayQueued(int _animation, bool _playOnce = true)
    {
        StopIdleAnimation();
        base.PlayQueued(_animation);
    }

    public void PlayPriority(int _animation, bool _playOnce = true)
    {
        StopIdleAnimation();
        m_queuedAnimations.Clear();
        base.PlayQueued(_animation);
    }

    public override void PlayContinuously(int _animation)
    {
        StopIdleAnimation();
        PlayQueued(_animation);
    }

    private void StopIdleAnimation()
    {
        m_isOnIdleAnimation = false;
        base._playOnce = true;
    }

    private void PlayIdleOnRepeat()
    {
        m_isOnIdleAnimation = true;
        m_currentAnimation = (int)ANIMATIONS.IDLE;
        base.PlayOnce = false;
        ChangeMaterial(m_animations[m_currentAnimation]);
        base.Play();
    }

    public override void PlayRandomQueued()
    {
        int m_randomNumber = UnityEngine.Random.Range(2, m_animations.Length);
        PlayQueued((int)GetAnimationFromAttackType(m_randomNumber));
    }

    public void GameOver()
    {
        m_disableIdleAnimation = true;
        OverrideAnimation((int)ANIMATIONS.DEATH);
    }
}
