using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
//using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
    public static event Action<int,string> setActiveSpellTextEvent;

    [Header("Player")]
    [SerializeField] private PlayerScript playerScript;
    [Header("Movement Settings")]
    [SerializeField] public float initialHorizontalSpeed;
    public float horizontalSpeed;
    [SerializeField] private float speedIncrease = 0.1f;
    [Serializable] public struct SpeedCaps
    {
        [Tooltip("Number of chunk loop iterations required for speed increase")]
        public int requiredNumberOfChunkLoopIterations;

        [Tooltip("Target cap to increase speed to")]
        public float targetSpeedCap;
    }
    [SerializeField] private List<SpeedCaps> speedCaps = new List<SpeedCaps>();
    private float currentTargetSpeedCap;
    private float timeSlowScale = 1f;
    [SerializeField] private float maxSpeedLimit;
    private bool isMaxSpeedReached = false;

    [Header("Chunk Generation")]
    private int currentChunkLoopIndex = 0;
    public enum ChunkDifficulties
    {
        Easy,
        Medium,
        Hard,
        Reward
    }
    [SerializeField] private GameObject[] easyChunkList;
    [SerializeField] private GameObject[] mediumChunkList;
    [SerializeField] private GameObject[] hardChunkList;
    [SerializeField] private GameObject[] rewardChunkList;
    [SerializeField] private List<ChunkDifficulties> chunkLoopOrder = new List<ChunkDifficulties>
    {
        ChunkDifficulties.Easy,
        ChunkDifficulties.Medium,
        ChunkDifficulties.Hard,
        ChunkDifficulties.Reward,
        ChunkDifficulties.Medium,
        ChunkDifficulties.Hard,
        ChunkDifficulties.Easy,
        ChunkDifficulties.Medium,
        ChunkDifficulties.Hard
    };
    [SerializeField] private Vector3 chunkSpawnPos;
    [SerializeField] private List<GameObject> spawnedChunks = new List<GameObject>();
    private int numberOfChunkLoopIterations;
    bool isFirstChunkToSpawn = true;

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
    private bool isMainScene;
    private const string mainSceneName = "MainScene";
    private const string titleScreenSceneName = "TitleScreen";

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

        activeSpellTypes.Add(SpellTypes.None);
        activeSpellTypes.Add(SpellTypes.None);
        activeSpellTypes.Add(SpellTypes.None);

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(mainSceneName) && runeChanceThresholds.Count == 0)
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isMainScene = scene.name == mainSceneName;
    }

    void Update()
    {
        if (isMainScene)
        {
            if (isMaxSpeedReached == false)
            {
                IncreaseSpeed();
            }
            
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
        playerScript.SetRuneSprites(spellCombo);
    }
    public void AddToEnemyList(GameObject enemy)
    {
        enemyList.Add(enemy);
    }

    public void RemoveFromEnemyList(GameObject enemy)
    {
        enemyList.Remove(enemy);
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
        GameObject[] chunkList = GetNextChunkListToSpawnFrom();
        int chunkAmount = chunkList.Length;
        int chunkNumber = UnityEngine.Random.Range(0, chunkAmount);
        GameObject loadedChunk = chunkList[chunkNumber];
        GameObject currentChunk = Instantiate(loadedChunk, chunkSpawnPos, Quaternion.identity);
        Debug.Log(currentChunk);
        spawnedChunks.Add(currentChunk);
        if (spawnedChunks.Count >= 3)
        {
            Destroy(spawnedChunks[spawnedChunks.Count - 3]);
        }
    }

    private GameObject[] GetNextChunkListToSpawnFrom()
    {
        if (isFirstChunkToSpawn == true)
        {
            CheckSpeedCaps();
            isFirstChunkToSpawn = false;
        }
        else if (currentChunkLoopIndex == 0)
        {
            numberOfChunkLoopIterations ++;
            Debug.Log(numberOfChunkLoopIterations + " chunk iterations");
            CheckSpeedCaps();
        }

        ChunkDifficulties currentDifficulty = chunkLoopOrder[currentChunkLoopIndex];
        currentChunkLoopIndex = (currentChunkLoopIndex + 1) % chunkLoopOrder.Count;

        switch (currentDifficulty)
        {
            case ChunkDifficulties.Easy: return easyChunkList;
            case ChunkDifficulties.Medium: return mediumChunkList;
            case ChunkDifficulties.Hard: return hardChunkList;
            case ChunkDifficulties.Reward: return rewardChunkList;
            default: return easyChunkList;
        }
    }

    private void IncreaseSpeed()
    {
        if (horizontalSpeed < currentTargetSpeedCap)
        {
            horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, currentTargetSpeedCap, speedIncrease * Time.deltaTime);
         }
        else if (horizontalSpeed == maxSpeedLimit)
        {
            isMaxSpeedReached = true;
        }
    }

    private void CheckSpeedCaps()
    {
        foreach (var cap in speedCaps)
        {
            if (numberOfChunkLoopIterations >= cap.requiredNumberOfChunkLoopIterations)
            {
                currentTargetSpeedCap = cap.targetSpeedCap;
            }
        }
    }

    public float GetCurrentSpeed()
    {
        return horizontalSpeed * timeSlowScale;
    }
    
    public void ResetHorizontalSpeed()
    {
        horizontalSpeed = initialHorizontalSpeed;
    }

    public void SetTimeSlowScale(float scale)
    {
        timeSlowScale = scale;
    }

    //Method for when an enemy is killed, increasing the score and combo count and giving the appropriate bonus if the combo limit is reached
    public void EnemyKilled(EnemyScript.EnemyTypes enemyType)
    {
        score += 10;

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

        playerScript.SetRuneSprites(spellCombo);
    }
    public float GetScore()
    {
        return score;
    }
    public void SetPlayer(PlayerScript playerScript)
    {
        this.playerScript = playerScript;
    }
    public PlayerScript GetPlayer()
    {
        return playerScript;
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

    public void StartGame()
    {
        SceneManager.LoadScene(mainSceneName);
        horizontalSpeed = initialHorizontalSpeed;
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + score);
        ResetHorizontalSpeed();
        score = 0;
        currentChunkLoopIndex = 0;
        numberOfChunkLoopIterations = 0;
        currentTargetSpeedCap = initialHorizontalSpeed;
        isFirstChunkToSpawn = true;
        isMaxSpeedReached = false;
        spawnedChunks.Clear();
        enemyList.Clear();
        obstacleList.Clear();
        spellCombo.Clear();
        SceneManager.LoadScene(titleScreenSceneName);
    }

}
