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
        PUNCH,
        SPECIAL,
        DEATH,
        HURT,
        KICK2,
        LOW_KICK,
        PUNCH2,

    }

    private ANIMATIONS m_currentAnimation = ANIMATIONS.NONE;
    public QueuedAnimatedTiledTexture m_animationRef;
    public CustomAnimationTextureModel[] m_animations;

    public bool m_isQueueAnimations = false;
    public bool m_isOnIdleAnimation = false;

    [Header("Animation Speed")]
    public int m_fps = 20;

    
	void Start () {
        m_animationRef.QueueMoves = this.m_isQueueAnimations;
        m_animationRef.ChangeAnimationFPS(m_fps);

       // StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        UIManager.Instance.GameView();
        yield return new WaitForEndOfFrame();
        //PlayQueued(ANIMATIONS.IDLE);
        //GameManager.instance.StartGame(0);
        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // PlayContinuously(ANIMATIONS.IDLE);
             PlayQueued(ANIMATIONS.PUNCH2);
            //PlayIdleOnRepeat();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //PlayQueued(ANIMATIONS.KICK);
            PlayQueued(ANIMATIONS.KICK2);
            //PlayRandomQueued();
        }
    
        if(m_animationRef.m_queuedAnimations.Count == 0 
            && !m_animationRef.IsPlaying()
            && !m_isOnIdleAnimation)
        {
            PlayIdleOnRepeat();
        }
    }

    private void StopIdleAnimation()
    {
        m_isOnIdleAnimation = false;
        m_animationRef._playOnce = true;
    }

    private void PlayIdleOnRepeat()
    {
        m_currentAnimation = ANIMATIONS.IDLE;
        m_animationRef.PlayOnce = false;
        m_isOnIdleAnimation = true;
        ChangeMaterial(ANIMATIONS.IDLE);
        m_animationRef.Play();
    }

    public void PlayContinuously(ANIMATIONS _animation)
    {
        StopIdleAnimation();
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
        StopIdleAnimation();
        m_animationRef.PlayOnce = true;
        Debug.Log("Anim: " + _animation.ToString() + " Current:" + m_currentAnimation);
        CustomAnimationTextureModel _animToSet = m_animations[(int)_animation];
        if (m_currentAnimation != _animation && !m_animationRef.IsPlaying())
        {
            m_currentAnimation = _animation;
            ChangeMaterial(_animToSet);
        } else if (m_animationRef.IsPlaying())
        {
            m_animationRef.m_queuedAnimations.Add(_animToSet);
        }
        m_animationRef.Play();
    }

    private void ChangeMaterial(CustomAnimationTextureModel _animation)
    {
        m_animationRef.ChangeCustomAnimationMaterial(_animation.Material, _animation.Rows, _animation.Columns, _animation.FrameSkips);
    }

    private void ChangeMaterial(ANIMATIONS _animation)
    {
        CustomAnimationTextureModel m_customAnim = m_animations[(int)_animation];
        m_animationRef.ChangeCustomAnimationMaterial(m_customAnim.Material, m_customAnim.Rows, m_customAnim.Columns, m_customAnim.FrameSkips);
    }
}
