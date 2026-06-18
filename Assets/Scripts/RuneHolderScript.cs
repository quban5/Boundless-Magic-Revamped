using System;
using System.Collections.Generic;
using UnityEngine;

public class RuneHolderScript : MonoBehaviour
{
    [Header("Runes")]
    [SerializeField] private SpriteRenderer[] runeSprites;

    [Header("Colors")]
    private Color yellowColor;
    private Color redColor;
    private Color blueColor;

    [Header("Spell icons")]
    [SerializeField] private SpriteRenderer spellIconRenderer;
    [SerializeField] private Sprite fireballSpellIcon;
    [SerializeField] private Sprite invulnerabilitySpellIcon;
    [SerializeField] private Sprite timeslowSpellIcon;
    [SerializeField] private Sprite shieldSpellIcon;
    [SerializeField] private Sprite swordSpellIcon;
    [SerializeField] private Sprite curseSpellIcon;

    [Header("Particles")]
    [SerializeField] private ParticleSystem successParticles;
    [SerializeField] private ParticleSystem failureParticles;


    void Start()
    {
        foreach (SpriteRenderer rune in runeSprites)
        {
            rune.gameObject.SetActive(false);
        }

        spellIconRenderer.gameObject.SetActive(false);

        yellowColor = new Color(0.98f, 0.95f, 0.21f, 0.16f);
        redColor = new Color(0.67f, 0.2f, 0.2f, 0.16f);
        blueColor = new Color(0.39f, 0.61f, 1f, 0.16f);

        SpellManager.onRunesChanged += SetRuneSprites;
        SpellManager.onSpellReadyToCast += SetSpellIcon;
        SpellManager.spellCastedEvent += ClearSpellIcon;
    }

    private void OnDestroy()
    {
        SpellManager.onRunesChanged -= SetRuneSprites;
        SpellManager.onSpellReadyToCast -= SetSpellIcon;
        SpellManager.spellCastedEvent -= ClearSpellIcon;
    }

    public void SetRuneSprites(List<SpellManager.RuneTypes> runes)
    {
        int i = 0;
        foreach (SpriteRenderer runeSprite in runeSprites)
        {
            if (runes.Count > i)
            {
                switch (runes[i])
                {
                    case SpellManager.RuneTypes.Red:
                        runeSprite.color = redColor;
                        break;
                    case SpellManager.RuneTypes.Yellow:
                        runeSprite.color = yellowColor;
                        break;
                    case SpellManager.RuneTypes.Blue:
                        runeSprite.color = blueColor;
                        break;
                }
                runeSprite.gameObject.SetActive(true);
                i++;
            }
            else
            {
                runeSprite.gameObject.SetActive(false);
            }
        }
    }

    public void SetSpellIcon(SpellManager.SpellTypes spellType)
    {
        spellIconRenderer.gameObject.SetActive(true);

        foreach (SpriteRenderer rune in runeSprites)
        {
            rune.gameObject.SetActive(false);
        }

        switch (spellType)
        {
            case SpellManager.SpellTypes.Fireball:
                spellIconRenderer.sprite = fireballSpellIcon;
                break;
            case SpellManager.SpellTypes.Invulnerability:
                spellIconRenderer.sprite = invulnerabilitySpellIcon;
                break;
            case SpellManager.SpellTypes.TimeSlow:
                spellIconRenderer.sprite = timeslowSpellIcon;
                break;
            case SpellManager.SpellTypes.Shield:
                spellIconRenderer.sprite = shieldSpellIcon;
                break;
            case SpellManager.SpellTypes.Sword:
                spellIconRenderer.sprite = swordSpellIcon;
                break;
            case SpellManager.SpellTypes.Curse:
                spellIconRenderer.sprite = curseSpellIcon;
                break;
        }
    }

    public void ClearSpellIcon()
    {
        if (spellIconRenderer.gameObject.activeSelf) 
        {
            successParticles.Play();
        } else
        {
            failureParticles.Play();
        }
        spellIconRenderer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
