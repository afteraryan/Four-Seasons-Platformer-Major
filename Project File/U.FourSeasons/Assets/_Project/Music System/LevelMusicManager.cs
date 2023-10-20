using System;
using System.Collections;
using UnityEngine;

public class LevelMusicManager : MonoBehaviour
{
    public AudioClip sadStartMusic;
    public AudioClip upbeatSantaHatMusic;

    private AudioSource sadStartAudioSource;
    private AudioSource upbeatSantaHatAudioSource;

    public float fadeDuration = 1f;

    private void Awake()
    {
        // Create and configure the audio sources
        sadStartAudioSource = gameObject.AddComponent<AudioSource>();
        sadStartAudioSource.clip = sadStartMusic;
        sadStartAudioSource.loop = true;
        sadStartAudioSource.playOnAwake = true;

        upbeatSantaHatAudioSource = gameObject.AddComponent<AudioSource>();
        upbeatSantaHatAudioSource.clip = upbeatSantaHatMusic;
        upbeatSantaHatAudioSource.loop = true;
        sadStartAudioSource.playOnAwake = false;
    }

    private void Start()
    {
        sadStartAudioSource.Play();
    }

    private void OnEnable()
    {
        I_SantaHat.OnSantaHatTriggered += HandleSantaHatTriggered;  // Subscribe to the event
    }

    private void OnDisable()
    {
        I_SantaHat.OnSantaHatTriggered -= HandleSantaHatTriggered;  // Unsubscribe from the event
    }

    private void HandleSantaHatTriggered()  // This method will handle the event
    {
        StartCoroutine(FadeOut(sadStartAudioSource, fadeDuration));
        StartCoroutine(FadeIn(upbeatSantaHatAudioSource, fadeDuration));
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        audioSource.volume = 0f;
        audioSource.Play();
        
        float startVolume = 0f;

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += startVolume + Time.deltaTime / duration*3;
            yield return null;
        }

        audioSource.volume = 1f;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}