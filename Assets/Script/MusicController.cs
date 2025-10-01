using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSouceSfx;
    public AudioSource audioMusic;
    public AudioClip click;
    public AudioClip theme;
    public AudioClip win;


    public void HandleClick(AudioClip audioClip)
    {
        audioSouceSfx.PlayOneShot(audioClip);
    }

    public void HandleMusic()
    {
        audioMusic.GetComponent<AudioSource>().clip = theme;
        audioMusic.Play();
    }
}
