using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

public class CanvasManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Active Spells")]
    [SerializeField] private TMP_Text[] activeSpellNames;
    [SerializeField] private Image[] activeSpellRuneSymbols;

    [Header("Pausing")]
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject UIScreen;
    [SerializeField] private TMP_Text unpauseCounter;

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

    public void OnPause()
    {
        UIScreen.SetActive(false);
        pauseScreen.SetActive(true);
        GameManager.Instance.PauseTime();
    }

    public void OnResume()
    {
        pauseScreen.SetActive(false);
        UIScreen.SetActive(true);
        unpauseCounter.gameObject.SetActive(true);
        StartCoroutine(UnpauseCounter());
    }

    IEnumerator UnpauseCounter()
    {
        unpauseCounter.text = "3";
        yield return new WaitForSecondsRealtime(1);
        unpauseCounter.text = "2";
        yield return new WaitForSecondsRealtime(1);
        unpauseCounter.text = "1";
        yield return new WaitForSecondsRealtime(1);
        unpauseCounter.gameObject.SetActive(false);
        GameManager.Instance.UnpauseTime();
    }

    public void OnExit()
    {
        GameManager.Instance.GameOver();
    }
}
