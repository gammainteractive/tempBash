using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationActions : QueuedAnimatedTiledTexture
{
    [HideInInspector]
    protected int m_currentAnimation = -1;
    private int m_maxQueuedAnimations = 3;

    public CustomAnimationTextureModel[] m_animations;

    public virtual void PlayContinuously(int _animation)
    {
        PlayQueued(_animation, false);
    }

    public virtual void PlayRandomQueued()
    {
        int m_randomNumber = UnityEngine.Random.Range(0, m_animations.Length);
        PlayQueued(m_randomNumber);
    }

    public void PlayCurrentQueueOnly(CustomAnimationTextureModel _animation, bool _playOnce = true)
    {
        base.PlayOnce = _playOnce;
        m_currentAnimation = -1;
        ChangeMaterial(_animation);
        base.Play();
    }

    public virtual void PlayQueued(int _animation, bool _playOnce = true)
    {
        base.PlayOnce = _playOnce;
        CustomAnimationTextureModel _animToSet = m_animations[_animation];
        if (m_currentAnimation != _animation && !base.IsPlaying())
        {
            m_currentAnimation = _animation;
            ChangeMaterial(_animToSet);
        }
        else if (base.IsPlaying())
        {
            if(m_queuedAnimations.Count > m_maxQueuedAnimations - 1)
            {
                base.m_queuedAnimations.RemoveAt(m_maxQueuedAnimations - 1);
            }
            base.m_queuedAnimations.Add(_animToSet);
        }
        base.Play();
    }

    public void ChangeMaterial(CustomAnimationTextureModel _animation)
    {
        base.ChangeCustomAnimationMaterial(_animation.Material, _animation.Rows, _animation.Columns, _animation.FrameSkips);
    }

    private void ChangeMaterial(int _animation)
    {
        CustomAnimationTextureModel m_customAnim = m_animations[_animation];
        base.ChangeCustomAnimationMaterial(m_customAnim.Material, m_customAnim.Rows, m_customAnim.Columns, m_customAnim.FrameSkips);
    }

    public void OverrideAnimation(int _animation, bool _playOnce = true)
    {
        StopClearAllAnimation();
        CustomAnimationTextureModel m_customAnim = m_animations[_animation];
        base.StopCurrentAnimation();
        PlayQueued(_animation, _playOnce);
    }

    public void StopClearAllAnimation()
    {
        ClearAnimationQueues();
        StopAllAnimations();
        _isPlaying = false;
    }

    public void ClearAnimationQueues()
    {
        m_queuedAnimations.Clear();
    }
}

