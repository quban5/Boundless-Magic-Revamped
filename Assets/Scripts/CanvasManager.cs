using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class CanvasManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Active Spells")]
    [SerializeField] private TMP_Text[] activeSpellNames;
    [SerializeField] private Image[] activeSpellRuneSymbols;

    void Start()
    {
        SetSpellNames(SpellManager.Instance.GetActiveSpellTypes());
        SetSpellRuneColours(SpellManager.Instance.GetActiveSpells());
    }

    private void SetSpellNames(List<SpellManager.SpellTypes> activeSpellTypes)
    {
        int i = 0;
        foreach (TMP_Text spellName in activeSpellNames)
        {
            spellName.text = activeSpellTypes[i].ToString();
            i++;
        }
    }

    private void SetSpellRuneColours(List<List<SpellManager.RuneTypes>> activeSpells)
    {
        int i = 0;
        foreach (List<SpellManager.RuneTypes> spell in activeSpells)
        {
            foreach (SpellManager.RuneTypes rune in spell)
            {
                switch (rune)
                {
                    case SpellManager.RuneTypes.Red:
                        activeSpellRuneSymbols[i].color = Color.red;
                        break;
                    case SpellManager.RuneTypes.Yellow:
                        activeSpellRuneSymbols[i].color = Color.yellow;
                        break;
                    case SpellManager.RuneTypes.Blue:
                        activeSpellRuneSymbols[i].color = Color.blue;
                        break;
                }
                i++;
            }
        }
    }

    void Update()
    {
        //Set the score text
        scoreText.text = "Score: " + GameManager.Instance.GetScore().ToString();
    }
}
