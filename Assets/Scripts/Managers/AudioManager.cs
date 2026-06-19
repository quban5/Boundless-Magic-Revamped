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
        GameManager.gameOverEvent += playTitleTheme;
        GameManager.startRunEvent += playMainTheme;
        GameManager.startRunEvent += playStartSound;
        SpellManager.spellSuccessEvent += playSpellCastSound;
        SpellManager.spellFailureEvent += playKillSound;
        SpellManager.setActiveSpellTextEvent += playShootSound;
    }
    public void playStartSound()
    {
        sfxSource.PlayOneShot(startSfx);
    }
    public void playShootSound()
    {
        sfxSource.PlayOneShot(shootSfx);
    }
    public void playKillSound()
    {
        sfxSource.PlayOneShot(killSfx);
    }
    public void playShootSound(int i, string s)
    {
        sfxSource.PlayOneShot(shootSfx);
    }
    public void playSpellCastSound()
    {
        sfxSource.PlayOneShot(spellCastSfx);
    }
    public void playSpellSetSound()
    {
        sfxSource.PlayOneShot(spellSetSfx);
    }
    public void playTitleTheme()
    {
        musicSource.clip = titleTheme;
        musicSource.Play();
    }
    public void playMainTheme()
    {
        musicSource.clip = mainTheme;
        musicSource.Play();
    }
}