using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterSounds : MonoBehaviour {

    public enum SOUND_EFFECTS
    {
        PUNCH,
        KICK,
        SPECIAL,
        CAT_SPECIAL = 3,
        ELEPHANT_SPECIAL = 4,
        MONKEY_SPECIAL = 5,
        PENGUIN_SPECIAL = 6,
    }

    public AudioSource m_audio;
    public AudioClip[] m_clips;

    public void PlaySound(int _clipNumber)
    {
        m_audio.clip = m_clips[_clipNumber];
        m_audio.Play();
    }
}
