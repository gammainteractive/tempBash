using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimatedEffects : AnimationActions
{

    private float m_animationDelay = 0.4f;
    private Enemy m_enemy;
    private Material m_material;

    public enum ANIMATIONS
    {
        NONE,
        DEFAULT,
    }

    private void Start()
    {
        m_enemy = GetComponentInParent<Enemy>();
        m_enemy.EnemyTakeHitHandle += PlayDefaultAnimation;
        m_enemy.EnemyAttackHandle += StopAnimation;
    }

    private void StopAnimation()
    {
        StopClearAllAnimation();
        PlayQueued((int)ANIMATIONS.NONE);
        StopClearAllAnimation();
    }

    private void PlayDefaultAnimation()
    {
        StartCoroutine(IPlayDefaultAnimation());
    }

    private IEnumerator IPlayDefaultAnimation()
    {
        yield return new WaitForSeconds(m_animationDelay);
        ToggleEffectsView(true);
        PlayQueued((int)ANIMATIONS.DEFAULT);
        while (IsPlaying())
        {
            yield return null;
        }
        ToggleEffectsView(false);
    }

    private void ToggleEffectsView(bool _isEnable)
    {
        GetComponent<Renderer>().enabled = _isEnable;
    }
}
