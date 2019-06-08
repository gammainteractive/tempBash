using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltraAnimations : AnimationActions {

    public enum ANIMATIONS
    {
        CAT_SPECIAL,
        ELEPHANT_SPECIAL,
        MONKEY_SPECIAL,
        PENGUIN_SPECIAL,
        NONE,
    }

    public Vector3[] m_ultraPositions;
    public Vector3[] m_ultraScales;

    public void PlayUltra(int _animation, bool _playOnce = true)
    {
        int m_random = Random.Range(0, 5);
        int m_animationRef = 0;
        if (m_random == 0)
        {
            m_animationRef = (int)ANIMATIONS.CAT_SPECIAL;
        }
        else if (m_random == 1)
        {
            m_animationRef = (int)ANIMATIONS.ELEPHANT_SPECIAL;
        }
        else if (m_random == 2)
        {
            m_animationRef = (int)ANIMATIONS.MONKEY_SPECIAL;
        }
        else if (m_random == 3)
        {
            m_animationRef = (int)ANIMATIONS.PENGUIN_SPECIAL;
        }
        transform.localScale = m_ultraScales[m_animationRef];
        transform.localPosition = m_ultraPositions[m_animationRef];
        PlayQueued(m_animationRef);
    }

    public void PlayUltraAnimations(int _animation)
    {
        StartCoroutine(IUltraAnimations(_animation));
    }

    private IEnumerator IUltraAnimations(int _animation)
    {
        while (IsPlaying())
        {
            yield return null;
        }
        GetComponent<Renderer>().enabled = true;
        PlayUltra(_animation);
        while (IsPlaying())
        {
            yield return null;
        }
        GetComponent<Renderer>().enabled = false;
    }

    private void DebugPlay()
    {
        int Anim = 3;
        transform.localScale = m_ultraScales[Anim];
        transform.localPosition = m_ultraPositions[Anim];
        PlayContinuously(Anim);
    }

    public override void StopCurrentAnimation()
    {
        StopClearAllAnimation();
        StopAllAnimations();
        base.StopCurrentAnimation();
        PlayQueued((int)ANIMATIONS.NONE);
        //StopClearAllAnimation();
    }


}
