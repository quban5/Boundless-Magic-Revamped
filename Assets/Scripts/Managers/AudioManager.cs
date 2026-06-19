using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("Audio sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Sound effects")]
    [SerializeField] private AudioClip startSfx;
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioClip killSfx;
    [SerializeField] private AudioClip spellCastSfx;
    [SerializeField] private AudioClip spellSetSfx;

    [Header("Music")]
    [SerializeField] private AudioClip titleTheme;
    [SerializeField] private AudioClip mainTheme;

    private void Start()
    {
        GameManager.gameOverEvent += PlayTitleTheme;
        GameManager.startRunEvent += PlayMainTheme;
        GameManager.startRunEvent += PlayStartSound;
        SpellManager.spellSuccessEvent += PlaySpellCastSound;
        SpellManager.spellFailureEvent += PlayKillSound;
        SpellManager.setActiveSpellTextEvent += PlayShootSound;
    }
    public void PlayStartSound()
    {
        sfxSource.PlayOneShot(startSfx);
    }
    public void PlayShootSound()
    {
        sfxSource.PlayOneShot(shootSfx);
    }
    public void PlayKillSound()
    {
        sfxSource.PlayOneShot(killSfx);
    }
    public void PlayShootSound(int i, string s)
    {
        sfxSource.PlayOneShot(shootSfx);
    }
    public void PlaySpellCastSound()
    {
        sfxSource.PlayOneShot(spellCastSfx);
    }
    public void PlaySpellSetSound()
    {
        sfxSource.PlayOneShot(spellSetSfx);
    }
    public void PlayTitleTheme()
    {
        musicSource.clip = titleTheme;
        musicSource.Play();
    }
    public void PlayMainTheme()
    {
        musicSource.clip = mainTheme;
        musicSource.Play();
    }

    private void OnDisable()
    {
        GameManager.gameOverEvent -= PlayTitleTheme;
        GameManager.startRunEvent -= PlayMainTheme;
        GameManager.startRunEvent -= PlayStartSound;
        SpellManager.spellSuccessEvent -= PlaySpellCastSound;
        SpellManager.spellFailureEvent -= PlayKillSound;
        SpellManager.setActiveSpellTextEvent -= PlayShootSound;
    }
}