using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    //Events
    public static event Action invincibilityEvent;
    public static event Action fireballEvent;
    public static event Action timeslowEvent;
    public static event Action shieldEvent;
    public static event Action swordEvent;
    public static event Action curseEvent;
    public static event Action<int, string> setActiveSpellTextEvent;

    [Header("Runes & Spells")]
    [SerializeField] private SpellTypes currentCastableSpell;

    //Dictionaries and lists
    private Dictionary<RuneTypes, float> runeChances = new Dictionary<RuneTypes, float>();
    public Dictionary<RuneTypes, float> runeChanceThresholds = new Dictionary<RuneTypes, float>();
    private Dictionary<SpellTypes, bool> unlockedSpells = new Dictionary<SpellTypes, bool>();
    private List<List<RuneTypes>> spellPool = new List<List<RuneTypes>>();
    private List<List<RuneTypes>> activeSpells = new List<List<RuneTypes>>();
    private List<SpellTypes> activeSpellTypes = new List<SpellTypes>();
    private List<RuneTypes> spellCombo = new List<RuneTypes>();

    private void Start()
    {
        GameManager.gameOverEvent += spellCombo.Clear;

        unlockedSpells.Add(SpellTypes.Invulnerability, true);
        unlockedSpells.Add(SpellTypes.Fireball, false);
        unlockedSpells.Add(SpellTypes.TimeSlow, false);
        unlockedSpells.Add(SpellTypes.Shield, false);
        unlockedSpells.Add(SpellTypes.Curse, false);
        unlockedSpells.Add(SpellTypes.Sword, false);
        unlockedSpells.Add(SpellTypes.TripleTrouble, false);

        activeSpellTypes.Add(SpellTypes.None);
        activeSpellTypes.Add(SpellTypes.None);
        activeSpellTypes.Add(SpellTypes.None);

        if (SceneManager.GetActiveScene().name == SceneNames.main && runeChanceThresholds.Count == 0)
        {
            GenerateSpellPool();
        }
    }

    public void ChangeActiveSpell(int spell)
    {
        SpellTypes tempActiveSpellType = activeSpellTypes[spell];

        tempActiveSpellType++;

        if (tempActiveSpellType > SpellTypes.TripleTrouble)
        {
            tempActiveSpellType = SpellTypes.None;
        }

        bool conflict = true;
        while (conflict)
        {
            conflict = false;
            int i = 0;
            foreach (SpellTypes spellType in activeSpellTypes)
            {
                if (i != spell)
                {
                    if (tempActiveSpellType == spellType && tempActiveSpellType != SpellTypes.None)
                    {
                        tempActiveSpellType++;

                        if (tempActiveSpellType > SpellTypes.TripleTrouble)
                        {
                            tempActiveSpellType = SpellTypes.None;
                        }

                        conflict = true;
                        break;
                    }
                }
                i++;
            }
        }

        activeSpellTypes[spell] = tempActiveSpellType;

        setActiveSpellTextEvent?.Invoke(spell, activeSpellTypes[spell].ToString());
    }

    public void CastSpell()
    {
        switch (currentCastableSpell)
        {
            case SpellTypes.Invulnerability:
                invincibilityEvent?.Invoke();
                GameManager.Instance.AddScore(10);
                break;
            case SpellTypes.Fireball:
                fireballEvent?.Invoke();
                GameManager.Instance.AddScore(10);
                break;
            case SpellTypes.TimeSlow:
                timeslowEvent?.Invoke();
                GameManager.Instance.AddScore(10);
                break;
            case SpellTypes.Shield:
                shieldEvent?.Invoke();
                GameManager.Instance.AddScore(10);
                break;
            case SpellTypes.Curse:
                curseEvent?.Invoke();
                GameManager.Instance.AddScore(10);
                break;
            case SpellTypes.Sword:
                swordEvent?.Invoke();
                GameManager.Instance.AddScore(10);
                break;
            case SpellTypes.TripleTrouble:
                //Implement triple trouble spell
                break;
        }
        currentCastableSpell = SpellTypes.None;
        spellCombo.Clear();
        GameManager.Instance.GetPlayer().SetRuneSprites(spellCombo);
    }

    public void GenerateSpellPool()
    {
        spellPool.Clear();

        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Yellow, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Blue, RuneTypes.Blue, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Blue, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Red, RuneTypes.Red });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Yellow, RuneTypes.Yellow });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Yellow, RuneTypes.Yellow, RuneTypes.Yellow });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Yellow, RuneTypes.Blue, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Red, RuneTypes.Yellow });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Red, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Yellow, RuneTypes.Yellow, RuneTypes.Blue });

        Debug.Log("Spell Pool Generated:");
        for (int i = 0; i < 10; i++)
        {
            string spellString = "";
            foreach (RuneTypes rune in spellPool[i])
            {
                spellString += rune.ToString() + " ";
            }
            Debug.Log("Spell " + (i + 1) + ": " + spellString);
        }

        //Choose the first 3 spells in the spell pool to be the active spells at the start of the game
        activeSpells.Clear();
        for (int i = 0; i < 3; i++)
        {
            int poolIndex = UnityEngine.Random.Range(0, spellPool.Count);
            activeSpells.Add(spellPool[poolIndex]);
            spellPool.RemoveAt(poolIndex);
            Debug.Log("Active Spell " + (i + 1) + ": " + string.Join(" ", activeSpells[i]));
        }

        //Calculating the chances of each rune type appearing in the active spells
        int totalRuneCount = 0;

        runeChances.Clear();
        runeChances.Add(RuneTypes.Red, 0);
        runeChances.Add(RuneTypes.Yellow, 0);
        runeChances.Add(RuneTypes.Blue, 0);

        foreach (List<RuneTypes> spell in activeSpells)
        {
            foreach (RuneTypes rune in spell)
            {
                totalRuneCount++;
                runeChances[rune]++;
            }
        }

        for (int i = 0; i < runeChances.Count; i++)
        {
            RuneTypes runeType = (RuneTypes)i;
            float chance = (runeChances[runeType] / totalRuneCount);
            runeChances[runeType] = chance;
            Debug.Log(runeType.ToString() + " Chance: " + chance.ToString("F2"));
        }

        //Calculating the thresholds for each rune type based on their chances
        runeChanceThresholds.Clear();
        runeChanceThresholds.Add(RuneTypes.Red, runeChances[RuneTypes.Red]);
        runeChanceThresholds.Add(RuneTypes.Yellow, runeChances[RuneTypes.Red] + runeChances[RuneTypes.Yellow]);
        runeChanceThresholds.Add(RuneTypes.Blue, 1f);

        Debug.Log("Rune Chance Thresholds:");
        foreach (KeyValuePair<RuneTypes, float> kvp in runeChanceThresholds)
        {
            Debug.Log(kvp.Key.ToString() + ": " + kvp.Value.ToString());
        }
    }

    public Dictionary<RuneTypes, float> GetRuneChanceThresholds()
    {
        return runeChanceThresholds;
    }

    public List<List<RuneTypes>> GetActiveSpells()
    {
        return activeSpells;
    }

    //Method for when an enemy is killed, increasing the score and combo count and giving the appropriate bonus if the combo limit is reached
    public void EnemyKilled(EnemyScript.EnemyTypes enemyType)
    {
        GameManager.Instance.AddScore(2);

        if (spellCombo.Count >= 3)
        {
            spellCombo.Clear();
        }

        spellCombo.Add((RuneTypes)enemyType);

        int i = 0;

        foreach (List<RuneTypes> spell in activeSpells)
        {
            if (spell.SequenceEqual(spellCombo.OrderBy(x => x)))
            {
                currentCastableSpell = activeSpellTypes[i];
            }
            i++;
        }

        GameManager.Instance.GetPlayer().SetRuneSprites(spellCombo);
    }

    private void OnDestroy()
    {
        GameManager.gameOverEvent -= spellCombo.Clear;
    }
}
