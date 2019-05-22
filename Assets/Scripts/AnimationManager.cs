using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    
    public enum ANIMATIONS
    {
        NONE,
        IDLE,
        KICK,
        PUNCH_UP,
        JAB,
        POUND,
        SPECIAL
    }

    private ANIMATIONS m_currentAnimation = ANIMATIONS.NONE;
    public QueuedAnimatedTiledTexture m_animationRef;
    public CustomAnimationTextureModel[] m_animations;

    public bool m_isQueueAnimations = false;

    [Header("Animation Speed")]
    public int m_fps = 20;

    
	void Start () {
        m_animationRef.QueueMoves = this.m_isQueueAnimations;
        m_animationRef.ChangeAnimationFPS(m_fps);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // PlayContinuously(ANIMATIONS.IDLE);
            PlayQueued(ANIMATIONS.JAB);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayRandomQueued();
        }

    }

    public void PlayContinuously(ANIMATIONS _animation)
    {
        m_animationRef._playOnce = false;
        PlayQueued(_animation);
    }

    public void PlayRandomQueued()
    {
        int m_randomNumber = UnityEngine.Random.Range(2, m_animations.Length);
        Array _values = Enum.GetValues(typeof(ANIMATIONS));
        ANIMATIONS m_randomAnimation = (ANIMATIONS)_values.GetValue(m_randomNumber);
        PlayQueued(m_randomAnimation);
    }

    public void PlayQueued(ANIMATIONS _animation)
    {
        Debug.Log("Anim: " + _animation.ToString() + " Current:" + m_currentAnimation);
        if(m_currentAnimation != _animation)
        {
            m_currentAnimation = _animation;
            ChangeMaterial(_animation);
        }
        m_animationRef.Play();
    }

    public void ChangeMaterial(ANIMATIONS _animation)
    {
        CustomAnimationTextureModel m_customAnim = m_animations[(int)_animation];
        m_animationRef.ChangeCustomAnimationMaterial(m_customAnim.Material, m_customAnim.Rows, m_customAnim.Columns, m_customAnim.FrameSkips);
      /*  m_animationRef._rows = m_animations[m_temp].m_rows;
        m_animationRef._columns = m_animations[m_temp].m_columns;*/
    }
}
