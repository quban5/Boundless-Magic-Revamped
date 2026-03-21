using System;
using System.Collections;
using System.Collections.Generic;
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

        Instance = this;
    }

    //Event for when the player gets invincibility from killing enough red enemies
    public static event Action invincibilityEvent;

    [Header("Prefabs")]
    [Tooltip("The prefab for wall spawning")]
    [SerializeField] private GameObject wallPrefab;
    [Tooltip("The prefab for enemy spawning")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Objects")]
    [Tooltip("Object holding the sprites of all floors")]
    [SerializeField] private GameObject ground;
    [Tooltip("The player object")]
    [SerializeField] private GameObject player;

    [Header("Spawning")]
    [Tooltip("The length of each chunk for level generation")]
    [SerializeField] private float chunkLength;
    [Tooltip("Y positions for spawning walls on different floors")]
    [SerializeField] private float[] spawnWallYOffsets;
    [Tooltip("Y positions for spawning enemies on different floors")]
    [SerializeField] private float[] spawnEnemyYOffsets;
    [Tooltip("The distance that the player has crossed into the current chunk")]
    [SerializeField] private float playerChunkDistance;
    [Tooltip("The position from which the player has entered the current chunk")]
    [SerializeField] private float playerLastChunkPosition;
    [Tooltip("List of chunks that can be used for level generation")]
    [SerializeField] private ChunkObject[] chunkList;
    [Tooltip("List of enemies that are currently in the scene")]
    private List<GameObject> enemyList = new List<GameObject>();
    [Tooltip("List of obstacles that are currently in the scene")]
    private List<GameObject> obstacleList = new List<GameObject>();

    [Header("Other")]
    [Tooltip("The player's current score")]
    [SerializeField] private float score;
    [Tooltip("The number of enemies the player has killed in a row")]
    [SerializeField] private int currentComboNumber;
    [Tooltip("Number of enemies the player needs to kill in a row to get a bonus")]
    [SerializeField] private int comboLimit;
    [Tooltip("The type of the current combo enemy")]
    [SerializeField] private EnemyScript.EnemyTypes comboEnemyType;

    //Enum for the different types of obstacles that can be spawned in the game
    public enum ObstacleTypes
    {
        None,
        RedEnemy,
        YellowEnemy,
        Wall
    }

    private void Start()
    {
        //Initial chunk spawning
        playerLastChunkPosition = player.transform.position.x;
    }

    void Update()
    {
        //Restarting the game when the restart button is clicked
        if (PlayerInputManager.Instance.restartClicked)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
        //Spawning new chunks when the player has crossed a certain distance into the current chunk
        if (player.transform.position.x - playerLastChunkPosition >= 20)
        {
            SpawnChunk();
            playerLastChunkPosition = player.transform.position.x;
        }
    }

    private void SpawnChunk()
    {
        //Randomly selecting a chunk from the list of chunks to spawn
        int chunkAmount = chunkList.Length;
        int chunkNumber = UnityEngine.Random.Range(0, chunkAmount - 1);
        ChunkObject loadedChunk = chunkList[chunkNumber];

        //Creating a list of the floor layouts for easier iteration when spawning obstacles
        List<ObstacleTypes[]> floorLayouts = new List<ObstacleTypes[]>();
        floorLayouts.Add(loadedChunk.topFloorLayout);
        floorLayouts.Add(loadedChunk.midTopFloorLayout);
        floorLayouts.Add(loadedChunk.midBottomFloorLayout);
        floorLayouts.Add(loadedChunk.bottomFloorLayout);

        //Calculating the spawn position for the new chunk and the distance between each obstacle to be spawned
        float spawnOffset = player.transform.position.x + chunkLength;
        int floorSpaces = loadedChunk.topFloorLayout.Length - 1;
        float spawnDistance = chunkLength / floorSpaces / 2;

        //Iterating through the floor layouts and spawning the appropriate obstacles based on the layout
        int i = 0;
        foreach (ObstacleTypes[] layout in floorLayouts)
        {
            foreach (ObstacleTypes type in layout)
            {
                //Spawning a wall if the type specified in the layout is a wall
                if (type == ObstacleTypes.Wall)
                {
                    Instantiate(wallPrefab, new Vector3(spawnOffset, spawnWallYOffsets[i], 0), Quaternion.Euler(0, 0, 0));
                }
                //Spawning an enemy if the type specified in the layout is a red or yellow enemy and setting the appropriate type for the enemy script
                if (type == ObstacleTypes.RedEnemy || type == ObstacleTypes.YellowEnemy)
                {
                    GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnOffset, spawnEnemyYOffsets[i], 0), Quaternion.Euler(0, 0, 0));
                    EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();

                    if (type == ObstacleTypes.RedEnemy)
                    {
                        enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Red);
                    }
                    else if (type == ObstacleTypes.YellowEnemy)
                    {
                        enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Yellow);
                    }
                }
                //Increasing the spawn offset for the next obstacle to be spawned
                spawnOffset += spawnDistance;
            }
            i++;
        }
    }

    //Method for when an enemy is killed, increasing the score and combo count and giving the appropriate bonus if the combo limit is reached
    public void EnemyKilled(EnemyScript.EnemyTypes enemyType)
    {
        score += 10;

        //Resetting the combo count if the type of enemy killed is different from the type of enemy in the current combo
        if (comboEnemyType != enemyType)
        {
            currentComboNumber = 0;
        }

        comboEnemyType = enemyType;

        currentComboNumber++;

        //Checking if the player has reached the combo limit and giving the appropriate bonus based on the type of enemy in the combo
        if (currentComboNumber >= comboLimit)
        {
            if (comboEnemyType == EnemyScript.EnemyTypes.Red)
            {
                invincibilityEvent?.Invoke();
            }
            else if (comboEnemyType == EnemyScript.EnemyTypes.Yellow)
            {
                DestroyEverything();
            }
            currentComboNumber = 0;
        }
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
}
