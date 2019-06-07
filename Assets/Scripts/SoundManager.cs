using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource[] m_audio;
    public AudioClip[] m_music;
    public AudioClip[] m_other;
    public MainCharacterSounds m_mainCharacterSounds;

    public enum AUDIO_TYPE
    {
        MUSIC,
        SOUNDS,
    }
    public enum MUSIC
    {
        TITLE,
        GAME
    }

    public enum OTHERS
    {
        BUZZER_INCORECT_INPUT,
        CHIME_CORRECT_INPUT,
        CHIME_CORRECT_SEQUENCE,
        GAME_WIN,
    }

    // Use this for initialization
    void Start () {
        PlayTitleMusic();
    }
	
    public void StopMusic()
    {
        m_audio[(int)AUDIO_TYPE.MUSIC].Stop();
    }

    public void PlayCorrectButton()
    {
        m_audio[(int)AUDIO_TYPE.SOUNDS].clip = m_other[(int)OTHERS.CHIME_CORRECT_INPUT];
        m_audio[(int)AUDIO_TYPE.SOUNDS].Play();
    }

    public void PlayCorrectSequence()
    {
        m_audio[(int)AUDIO_TYPE.SOUNDS].clip = m_other[(int)OTHERS.CHIME_CORRECT_SEQUENCE];
        m_audio[(int)AUDIO_TYPE.SOUNDS].Play();
    }

    public void PlayIncorrectSequence()
    {
        m_audio[(int)AUDIO_TYPE.SOUNDS].clip = m_other[(int)OTHERS.BUZZER_INCORECT_INPUT];
        m_audio[(int)AUDIO_TYPE.SOUNDS].Play();
    }

    public void PlayGameOverSound(bool _isWin)
    {
        if (_isWin)
        {
            m_audio[(int)AUDIO_TYPE.SOUNDS].clip = m_other[(int)OTHERS.GAME_WIN];
        }
        m_audio[(int)AUDIO_TYPE.SOUNDS].Play();
    }

    public void PlayTitleMusic()
    {
        m_audio[(int)AUDIO_TYPE.MUSIC].clip = m_music[(int)MUSIC.TITLE];
        m_audio[(int)AUDIO_TYPE.MUSIC].Play();
    }

    public void PlayGameMusic()
    {
        m_audio[(int)AUDIO_TYPE.MUSIC].clip = m_music[(int)MUSIC.GAME];
        m_audio[(int)AUDIO_TYPE.MUSIC].Play();
    }
     

    public void MainCharacterAttackTypePlaySound(int _index)
    {
        m_mainCharacterSounds.PlaySound(_index);
    }

    private int m_ultraSoundEffectsIndex = 3;

    public void MainCharacterAttackUltra(int _animRef)
    {
        //Animation references are based on te MainCharacterAnimation values
       // Debug.Log("Index: " + (_animRef + m_ultraSoundEffectsIndex));
        m_mainCharacterSounds.PlaySound((int)_animRef  + m_ultraSoundEffectsIndex);
        /* switch (_animRef)
         {
             case (int)MainCharacterAnimations.ANIMATIONS.CAT_SPECIAL:
                 m_mainCharacterSounds.PlaySound((int)MainCharacterSounds.SOUND_EFFECTS.CAT_SPECIAL);
                 break;
             case (int)MainCharacterAnimations.ANIMATIONS.ELEPHANT_SPECIAL:
                 m_mainCharacterSounds.PlaySound((int)MainCharacterSounds.SOUND_EFFECTS.ELEPHANT_SPECIAL);
                 break;
             case (int)MainCharacterAnimations.ANIMATIONS.MONKEY_SPECIAL:
                 m_mainCharacterSounds.PlaySound((int)MainCharacterSounds.SOUND_EFFECTS.MONKEY_SPECIAL);
                 break;
             case (int)MainCharacterAnimations.ANIMATIONS.PENGUIN_SPECIAL:
                 m_mainCharacterSounds.PlaySound((int)MainCharacterSounds.SOUND_EFFECTS.PENGUIN_SPECIAL);
                 break;
         }*/
    }
}
