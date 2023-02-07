using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip ballBounceSFX;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonClickSFX()
    {
        audioSource.PlayOneShot(buttonClickSFX);
    }

    public void PLayBallBounceSFX()
    {
        audioSource.PlayOneShot(ballBounceSFX);
    }
}
