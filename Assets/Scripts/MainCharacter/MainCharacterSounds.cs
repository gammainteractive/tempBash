using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterSounds : MonoBehaviour {

    public enum SOUND_EFFECTS
    {
        PUNCH,
        KICK,
        SPECIAL,
    }

    public AudioSource m_audio;
    public AudioClip[] m_clips;

    public void PlaySound(int _clipNumber)
    {
        m_audio.clip = m_clips[_clipNumber];
        m_audio.Play();
    }
}
