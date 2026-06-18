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

    void Start()
    {
        foreach (SpriteRenderer rune in runeSprites)
        {
            rune.gameObject.SetActive(false);
        }

        yellowColor = new Color(0.98f, 0.95f, 0.21f, 0.16f);
        redColor = new Color(0.67f, 0.2f, 0.2f, 0.16f);
        blueColor = new Color(0.39f, 0.61f, 1f, 0.16f);

        SpellManager.onRunesChanged += SetRuneSprites;
    }

    private void OnDestroy()
    {
        SpellManager.onRunesChanged -= SetRuneSprites;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
