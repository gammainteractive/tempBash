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
        CAT_SPECIAL = 10,
        ELEPHANT_SPECIAL = 11,
        MONKEY_SPECIAL = 12,
        PENGUIN_SPECIAL = 13,
    }

    public bool m_isOnIdleAnimation = false;
    protected bool m_disableIdleAnimation = false;
    public UltraAnimations m_ultraAnimations;
    public Material m_material;

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
            PlayCurrentQueueOnly();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.instance.GameOver(false);
        }
    }

    //Button value or attack types depend on the button value definition on UIManager.
    //Returns sound effect tied up to the animations
    public void AttackTypePlayQueued(int _attackType)
    {
        StopIdleAnimation();
        int m_animation = 0;
        m_animation = (int)GetAnimationFromAttackType(_attackType);
        base.PlayQueued(m_animation);
    }

    public void MainCharacterAttackUltra(int _animation)
    {
        StartCoroutine(UltraAttack(_animation));
    }

    private IEnumerator UltraAttack(int _animation)
    {
        while (m_ultraAnimations.IsPlaying())
        {
            yield return null;
        }
        m_ultraAnimations.GetComponent<Renderer>().enabled = true;
        m_ultraAnimations.PlayUltra(_animation);
        while (m_ultraAnimations.IsPlaying())
        {
            yield return null;
        }
        m_ultraAnimations.GetComponent<Renderer>().enabled = false;
    }

    private void ToggleCharacterView(bool _isEnable)
    {
        m_material = GetComponent<Renderer>().material;
        Color _temp = m_material.color;
        if (_isEnable)
        {
            _temp.a = 0;
        } else
        {
            _temp.a = 1;
        }

        m_material.color = _temp;
    }

   /* public int GetSoundsFXAttack(int _randomNum)
    {
        if (_randomNum == 0)
        {
            return (int)ANIMATIONS.CAT_SPECIAL;
        }
        else if (_randomNum == 1)
        {
            return (int)ANIMATIONS.ELEPHANT_SPECIAL;
        }
        else if (_randomNum == 2)
        {
            return (int)ANIMATIONS.MONKEY_SPECIAL;
        }
        else if (_randomNum == 3)
        {
            return (int)ANIMATIONS.PENGUIN_SPECIAL;
        }
        return 0;
    }*/

    public void Reset()
    {
        m_disableIdleAnimation = false;
    }

    public ANIMATIONS GetAnimationFromAttackType(int _attackType)
    {
        int m_random = 0;
        switch (_attackType)
        {
            case ((int)AttackTypes.ATTACK_TYPES.KICK):
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
            case ((int)AttackTypes.ATTACK_TYPES.PUNCH):
                m_random = Random.Range(0, 2);
                if (m_random == 0)
                {
                    return ANIMATIONS.PUNCH;
                }
                else
                {
                    return ANIMATIONS.PUNCH2;
                }
            case ((int)AttackTypes.ATTACK_TYPES.SPECIAL):
                return ANIMATIONS.SPECIAL;
        }
        return ANIMATIONS.IDLE;
    }

    public void PlayUltra(int _animation)
    {
        m_ultraAnimations.PlayUltra(_animation - 10);
    }

    public void PlayQueuedAnimations()
    {
        StopIdleAnimation();
        for (int i = 0; i < m_queuedAnimations.Count; i++)
        {
            base.Play();
        }
    }

    //Special for mode C, we can queue with no limit
    public void AddQueuedAnimations(int _animation)
    {
        int m_temp = (int)GetAnimationFromAttackType(_animation);
        CustomAnimationTextureModel _animToSet = m_animations[m_temp];
        base.m_queuedAnimations.Add(_animToSet);
    }

    public void PlayCurrentQueueOnly()
    {
        StopIdleAnimation();
        base.PlayCurrentQueueOnly(m_queuedAnimations[0]); 
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

    public void GameOver()
    {
        m_disableIdleAnimation = true;
        OverrideAnimation((int)ANIMATIONS.DEATH);
    }
}
