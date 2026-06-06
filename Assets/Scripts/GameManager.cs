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
    //Event for when the player gets invincibility from killing enough red enemies
    public static event Action invincibilityEvent;
    [Header("Objects")]
    [Tooltip("The player object")]
    [SerializeField] private GameObject player;
    [Header("Spawning")]
    [Tooltip("The length of each chunk, used to determine where to spawn the next chunk")]
    [SerializeField] private float chunkLength;
    [Tooltip("Distance between chunks")]
    [SerializeField] private float distanceBetweenChunks;
    [Tooltip("List of chunks that can be used for level generation")]
    [SerializeField] private GameObject[] chunkList;
    [Tooltip("List of enemies that are currently in the scene")]
    private List<GameObject> enemyList = new List<GameObject>();
    [Tooltip("List of obstacles that are currently in the scene")]
    private List<GameObject> obstacleList = new List<GameObject>();
    [Header("Other")]
    [Tooltip("The player's current score")]
    [SerializeField] private float score;
    private Dictionary<RuneTypes, float> runeChances = new Dictionary<RuneTypes, float>();
    public Dictionary<RuneTypes, float> runeChanceThresholds = new Dictionary<RuneTypes, float>();
    private Dictionary<SpellTypes, bool> unlockedSpells = new Dictionary<SpellTypes, bool>();
    private List<List<RuneTypes>> spellPool = new List<List<RuneTypes>>();
    private List<List<RuneTypes>> activeSpells = new List<List<RuneTypes>>();
    [SerializeField] private Vector3 chunkSpawnPos;
    private List<GameObject> spawnedChunks = new List<GameObject>();
    private List<RuneTypes> spellCombo = new List<RuneTypes>();
    public enum RuneTypes
    {
        Red = 0,
        Yellow = 1,
        Blue = 2
    }
    public enum SpellTypes
    {
        Invulnerability = 0,
        Fireball = 1,
        TimeSlow = 2,
        Shield = 3,
        Curse = 4,
        Sword = 5,
        TripleTrouble = 6,
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

        foreach (List<RuneTypes> spell in activeSpells)
        {
            int i = 0;
            if (spell.SequenceEqual(spellCombo))
            {
                if (i == 0)
                {
                    invincibilityEvent?.Invoke();
                }
                else if (i == 1)
                {
                    DestroyEverything();
                }
            }
            i++;
        }

        if (spellCombo.Count > 2)
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
        for (int i = 0; i < 8; i++)
        {
            while (true)
            {
                List<RuneTypes> generatedSpell = new List<RuneTypes>();
                for (int j = 0; j < 3; j++)
                {
                    int runeNum = UnityEngine.Random.Range(0, 3);
                    generatedSpell.Add((RuneTypes)runeNum);
                }

                if (!spellPool.Any(poolSpell => poolSpell.SequenceEqual(generatedSpell)))
                {
                    spellPool.Add(generatedSpell);
                    break;
                }
            }
        }

        Debug.Log("Spell Pool Generated:");
        for (int i = 0; i < 8; i++)
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
