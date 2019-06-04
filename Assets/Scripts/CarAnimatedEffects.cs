using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimatedEffects : AnimationActions {

    private float m_animationDelay = 0.4f;
    private Enemy m_enemy;

    public enum ANIMATIONS
    {
        NONE,
        DEFAULT,
    }

    private void Start()
    {
        m_enemy = GetComponentInParent<Enemy>();
        m_enemy.EnemyTakeHitHandle += PlayDefaultAnimation;
    }

    private void PlayDefaultAnimation()
    {
        StartCoroutine(IPlayDefaultAnimation());
    }

    private IEnumerator IPlayDefaultAnimation()
    {
        yield return new WaitForSeconds(m_animationDelay);
        PlayQueued((int)ANIMATIONS.DEFAULT);
        PlayQueued((int)ANIMATIONS.NONE);
    }



}
