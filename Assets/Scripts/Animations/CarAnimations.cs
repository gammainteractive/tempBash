using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimations : AnimationActions
{
    public enum ANIMATIONS
    {
        NONE,
        IDLE,
        DEATH,
    }

    public bool m_isOnIdleAnimation = false;
    private bool m_disableIdleAnimation = false;
	
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
