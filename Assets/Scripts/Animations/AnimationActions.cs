using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationActions : QueuedAnimatedTiledTexture
{
    public int m_currentAnimation;

    public CustomAnimationTextureModel[] m_animations;

    public virtual void PlayContinuously(int _animation)
    {
        PlayQueued(_animation);
    }

    public void PlayRandomQueued()
    {
        int m_randomNumber = UnityEngine.Random.Range(2, m_animations.Length);
        PlayQueued(m_randomNumber);
    }

    public virtual void PlayQueued(int _animation)
    {
        base.PlayOnce = true;
        CustomAnimationTextureModel _animToSet = m_animations[_animation];
        if (m_currentAnimation != _animation && !base.IsPlaying())
        {
            m_currentAnimation = _animation;
            ChangeMaterial(_animToSet);
        }
        else if (base.IsPlaying())
        {
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

    public void ClearAnimationQueues()
    {
        m_queuedAnimations.Clear();
    }
}

