using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton pattern to ensure only one instance of GameManager exists
    public static GameManager Instance { get; private set; }


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

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Chunk Generation")]
    [SerializeField] private float chunkLength;
    [SerializeField] private float distanceBetweenChunks;
    [SerializeField] private GameObject[] chunkList;
    [SerializeField] private Vector3 chunkSpawnPos;
    [SerializeField] private List<GameObject> spawnedChunks = new List<GameObject>();

    [Header("Enemies & Obstacles")]
    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] private List<GameObject> obstacleList = new List<GameObject>();

    [Header("Score")]
    [SerializeField] private float score;

    [Header("Runes & Spells")]
    [SerializeField] private float[] runeChanceValues;
    [SerializeField] private float[] runeChanceThresholdValues;
    [SerializeField] private SpellTypes currentCastableSpell;

    //Dictionaries and lists
    private Dictionary<RuneTypes, float> runeChances = new Dictionary<RuneTypes, float>();
    public Dictionary<RuneTypes, float> runeChanceThresholds = new Dictionary<RuneTypes, float>();
    private Dictionary<SpellTypes, bool> unlockedSpells = new Dictionary<SpellTypes, bool>();
    private List<List<RuneTypes>> spellPool = new List<List<RuneTypes>>();
    private List<List<RuneTypes>> activeSpells = new List<List<RuneTypes>>();
    private List<SpellTypes> activeSpellTypes = new List<SpellTypes>();
    private List<RuneTypes> spellCombo = new List<RuneTypes>();

    public enum RuneTypes
    {
        Red = 0,
        Yellow = 1,
        Blue = 2
    }
    public enum SpellTypes
    {
        None = 0,
        Invulnerability = 1,
        Fireball = 2,
        TimeSlow = 3,
        Shield = 4,
        Curse = 5,
        Sword = 6,
        TripleTrouble = 7,
    }
    private void Start()
    {
        unlockedSpells.Add(SpellTypes.Invulnerability, true);
        unlockedSpells.Add(SpellTypes.Fireball, false);
        unlockedSpells.Add(SpellTypes.TimeSlow, false);
        unlockedSpells.Add(SpellTypes.Shield, false);
        unlockedSpells.Add(SpellTypes.Curse, false);
        unlockedSpells.Add(SpellTypes.Sword, false);
        unlockedSpells.Add(SpellTypes.TripleTrouble, false);

        activeSpellTypes.Add(SpellTypes.Sword);
        activeSpellTypes.Add(SpellTypes.Curse);
        activeSpellTypes.Add(SpellTypes.TimeSlow);

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainScene") && runeChanceThresholds.Count == 0)
        {
            GenerateSpellPool();
        }
    }

    void Update()
    {
        //Restarting the game when the restart button is clicked
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainScene"))
        {
            if (PlayerInputManager.Instance.restartClicked)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentScene);
            }
        }
    }
    public void CastSpell()
    {
        switch (currentCastableSpell)
        {
            case SpellTypes.Invulnerability:
                invincibilityEvent?.Invoke();
                break;
            case SpellTypes.Fireball:
                fireballEvent?.Invoke();
                break;
            case SpellTypes.TimeSlow:
                timeslowEvent?.Invoke();
                break;
            case SpellTypes.Shield:
                shieldEvent?.Invoke();
                break;
            case SpellTypes.Curse:
                Curse();
                break;
            case SpellTypes.Sword:
                swordEvent?.Invoke();
                break;
            case SpellTypes.TripleTrouble:
                //Implement triple trouble spell
                break;
        }
        spellCombo.Clear();
        player.GetComponent<PlayerScript>().SetRuneSprites(spellCombo);
    }

    private void Curse()
    {
        foreach (GameObject enemy in enemyList)
        {
            Destroy(enemy);
        }
        enemyList.Clear();
    }

    public void SpawnChunk()
    {
        chunkSpawnPos += new Vector3(chunkLength + distanceBetweenChunks, 0, 0);
        int chunkAmount = chunkList.Length;
        int chunkNumber = UnityEngine.Random.Range(0, chunkAmount);
        GameObject loadedChunk = chunkList[chunkNumber];
        GameObject currentChunk = Instantiate(loadedChunk, chunkSpawnPos + new Vector3(chunkLength / 2, 0, 0), Quaternion.identity);
        spawnedChunks.Add(currentChunk);
        if (spawnedChunks.Count >= 3)
        {
            Destroy(spawnedChunks[spawnedChunks.Count - 3]);
        }
    }

    //Method for when an enemy is killed, increasing the score and combo count and giving the appropriate bonus if the combo limit is reached
    public void EnemyKilled(EnemyScript.EnemyTypes enemyType)
    {
        score += 10;

        spellCombo.Add((RuneTypes)enemyType);

        int i = 0;

        foreach (List<RuneTypes> spell in activeSpells)
        {
            if (spell.OrderBy(x => x).SequenceEqual(spellCombo.OrderBy(x => x)))
            {
                currentCastableSpell = activeSpellTypes[i];
            }
            i++;
        }

        if (spellCombo.Count > 3)
        {
            spellCombo.Clear();
        }

        player.GetComponent<PlayerScript>().SetRuneSprites(spellCombo);
    }

    //Method for destroying all enemies and obstacles in the scene, used for the yellow enemy combo bonus
    private void DestroyEverything()
    {
        foreach (GameObject enemy in enemyList)
        {
            Destroy(enemy);
            enemyList.Remove(enemy);
        }
        foreach (GameObject obstacle in obstacleList)
        {
            Destroy(obstacle);
            obstacleList.Remove(obstacle);
        }
    }
    public float GetScore()
    {
        return score;
    }
    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
    public GameObject GetPlayer()
    {
        return player;
    }
    public void GenerateSpellPool()
    {
        spellPool.Clear();
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Blue, RuneTypes.Red, RuneTypes.Yellow });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Blue, RuneTypes.Blue, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Blue, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Red, RuneTypes.Red });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Yellow, RuneTypes.Red, RuneTypes.Yellow });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Yellow, RuneTypes.Yellow, RuneTypes.Yellow });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Yellow, RuneTypes.Blue, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Red, RuneTypes.Yellow });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Red, RuneTypes.Red, RuneTypes.Blue });
        spellPool.Add(new List<RuneTypes>() { RuneTypes.Blue, RuneTypes.Yellow, RuneTypes.Yellow });

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

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + score);
        SceneManager.LoadScene("TitleScreen");
    }

}
