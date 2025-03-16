using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSound : MonoBehaviour
{
    private void soundController(int index, float vol)
    {
        if (AudioManager.instance.isSoundOn == 1) // If sound is On
        {
            AudioManager.instance.audioSources[index].Stop();

            AudioManager.instance.audioSources[index].pitch = UnityEngine.Random.Range(0.85f, 0.95f);
            AudioManager.instance.audioSources[index].volume = vol;
            AudioManager.instance.audioSources[index].Play();
        }
        else // If sound is Off
        {
            AudioManager.instance.audioSources[index].volume = 0f;
            AudioManager.instance.audioSources[index].Stop();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Sound");
        soundController(2, 1f);
    }
}
