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
    public bool m_disableIdleAnimation = false;
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
    }

    //Button value or attack types depend on the button value definition on UIManager.
    //Returns sound effect tied up to the animations
    public void AttackTypePlayQueued(int _attackType)
    {
        StopIdleAnimation();
        int m_animation = 0;
        m_animation = (int)GetAnimationFromAttackType(_attackType);
        PlayQueued(m_animation);
    }

    public void MainCharacterAttackUltra(int _animation)
    {
        m_ultraAnimations.PlayUltraAnimations(_animation);
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

    //Special for mode C, we can queue with no limit
    public void AddQueuedAnimations(int _animation)
    {
        if (m_queuedAnimations.Count < 4)
        { 
            int m_temp = (int)GetAnimationFromAttackType(_animation);
            CustomAnimationTextureModel _animToSet = m_animations[m_temp];
            base.m_queuedAnimations.Add(_animToSet);
        }
    }

    public void StopUltraAnimations()
    {
        StartCoroutine(IStopUltraAnimations());
    }

    private IEnumerator IStopUltraAnimations()
    {
        StopIdleAnimation();
        m_ultraAnimations.StopCurrentAnimation();
        yield return new WaitForSeconds(2.3f);
        //LastAnimationQueueEvent();
    }

    public void PlayPriority(int _animation, bool _playOnce = true)
    {
        StopIdleAnimation();
        m_queuedAnimations.Clear();
        PlayQueued(_animation);
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

    public override void PlayQueued(int _animation, bool _playOnce = true)
    {
        StopIdleAnimation();
        base.PlayOnce = _playOnce;
        CustomAnimationTextureModel _animToSet = m_animations[_animation];
        if (m_currentAnimation != _animation && !IsPlaying())
        {
            m_currentAnimation = _animation;
            ChangeMaterial(_animToSet);
        }
        else if (IsPlaying())
        {
            if (m_queuedAnimations.Count > 3)
            {
                m_queuedAnimations.RemoveAt(2);
            }
            m_queuedAnimations.Add(_animToSet);
        }
        Play();
    }

    public override void Play()
    {
        StartCoroutine(IPlayMainCharacter());
    }

    public IEnumerator IPlayMainCharacter()
    {
        while (_isPlaying)
        {
            yield return null;
        }
        _isPlaying = true;
        CustomAnimationTextureModel m_temp = null;
        if (m_queuedAnimations.Count > 0)
        {
            m_temp = m_queuedAnimations[0];
            ChangeCustomAnimationMaterial(m_temp.Material, m_temp.Rows, m_temp.Columns, m_temp.FrameSkips);
        }

       /* if(m_queuedAnimations.Count == 0)
        {
            LastAnimationQueueEvent();
        }*/
        // Make sure the renderer is enabled
        GetComponent<Renderer>().enabled = true;

        //Because of the way textures calculate the y value, we need to start at the max y value
        _index = _columns;

        // Start the update tiling coroutine
        yield return StartCoroutine(updateTiling());

        m_queuedAnimations.Remove(m_temp);
        if (m_queuedAnimations.Count == 0)
        {
            LastAnimationQueueEvent();
        }
        _isPlaying = false;
    }
}
