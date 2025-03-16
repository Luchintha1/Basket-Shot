using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [field: SerializeField] public AudioSource[] audioSources { get; private set; } // UI

    [field: SerializeField] public AudioSource music { get; private set; }

    public int isSoundOn { get; set; }

    private void Awake()
    {
        instance = this;

        // isSound = 1 is On
        // isSound = 0 is Off
        isSoundOn = PlayerPrefs.GetInt("Sound", 0);
    }

    private void Start()
    {
        if(isSoundOn == 0)
        {
            music.Stop();

            foreach (AudioSource audio in audioSources)
            {
                audio.volume = 0f;
                audio.Stop();
            }
        }
        else
        {
            music.volume = 0.1f;
            music.Play();
        }
    }

}
