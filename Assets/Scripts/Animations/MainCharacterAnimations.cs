using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterAnimations : AnimationActions {
    
    public enum ANIMATIONS
    {
        NONE,
        IDLE,
        KICK,
        PUNCH,
        SPECIAL,
        DEATH,
        HURT,
        KICK2,
        LOW_KICK,
        PUNCH2,
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
    }

    public override void PlayQueued(int _animation)
    {
        StopIdleAnimation();
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
        m_currentAnimation = (int)ANIMATIONS.IDLE;
        base.PlayOnce = false;
        m_isOnIdleAnimation = true;
        ChangeMaterial(m_animations[m_currentAnimation]);
        base.Play();
    }

    public void GameOver()
    {
        m_disableIdleAnimation = true;
       // ClearAnimationQueues();
        PlayQueued((int)ANIMATIONS.DEATH);
    }
}
