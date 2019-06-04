using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource m_audio;
    public AudioClip[] m_clips;

    public enum MUSIC
    {
        TITLE,
        GAME
    }

	// Use this for initialization
	void Start () {
        PlayTitleMusic();
    }
	
    public void StopMusic()
    {
        m_audio.Stop();
    }

	public void PlayTitleMusic()
    {
        m_audio.clip = m_clips[(int)MUSIC.TITLE];
        m_audio.Play();
    }
}
