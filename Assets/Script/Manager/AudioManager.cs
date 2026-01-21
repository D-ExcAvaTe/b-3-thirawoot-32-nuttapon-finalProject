using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource; 
    [SerializeField] private AudioSource ambientSource; 
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioClip bgmClip; 
    [SerializeField] private AudioClip ambientClip; 

    private Dictionary<int, AudioClip> sfxClipDictionary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        sfxClipDictionary = new Dictionary<int, AudioClip>();
        for (int i = 0; i < sfxClips.Length; i++)
        {
            if (!sfxClipDictionary.ContainsKey(i))
            {
                sfxClipDictionary.Add(i, sfxClips[i]);
            }
        }
    }

    private void Start()
    {
        PlayBGM();
        PlayAmbient();
    }

    public void PlaySFX(int sfxIndex)
    {
        if (sfxClipDictionary.TryGetValue(sfxIndex, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX with index " + sfxIndex + " not found!");
        }
    }

    public void PlayBGM()
    {
        if (bgmClip != null && bgmSource != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM or BGM Source is missing!");
        }
    }

    public void PlayAmbient()
    {
        if (ambientClip != null && ambientSource != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }
        else
        {
            Debug.LogWarning("Ambient or Ambient Source is missing!");
        }
    }
}
