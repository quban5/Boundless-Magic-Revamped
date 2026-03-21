using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action invincibilityEvent;

    private List<GameObject> enemyList = new List<GameObject>();
    private List<GameObject> obstacleList = new List<GameObject>();
    [SerializeField] private ChunkObject[] chunkList;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Objects")]
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject player;

    [Header("Spawning")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnIntervalIncrease;
    [SerializeField] private float minSpawnInterval;
    [SerializeField] private float spawnXOffset;
    [SerializeField] private float[] spawnObstacleYOffset;
    [SerializeField] private float[] spawnEnemyYOffset;
    [SerializeField] private float playerDistance;
    [SerializeField] private float playerLastPositionX;

    [Header("Other")]
    [SerializeField] private float score;
    [SerializeField] private int enemiesKilled;
    [SerializeField] private int enemyComboCount;
    [SerializeField] private EnemyScript.EnemyTypes enemyComboType;
    public enum ObstacleTypes
    {
        None,
        RedEnemy,
        YellowEnemy,
        Wall
    }

    private void Start()
    {
        playerLastPositionX = player.transform.position.x;
    }

    void Update()
    {
        //Restart game
        if (PlayerInputManager.Instance.restartClicked)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
        //Decread spawn interval
        if (spawnInterval > minSpawnInterval)
        {
            spawnInterval -= spawnIntervalIncrease * Time.deltaTime;
        }

        if (player.transform.position.x - playerLastPositionX >= 20)
        {
            SpawnChunk();
            playerLastPositionX = player.transform.position.x;
        }
    }

    private void SpawnChunk()
    {
        int chunkAmmount = chunkList.Length;
        int chunkNumber = UnityEngine.Random.Range(0, chunkAmmount - 1);
        ChunkObject loadedChunk = chunkList[chunkNumber];

        float floorLength = 10f;

        List<ObstacleTypes[]> floorLayouts = new List<ObstacleTypes[]>();
        floorLayouts.Add(loadedChunk.topFloorLayout);
        floorLayouts.Add(loadedChunk.midTopFloorLayout);
        floorLayouts.Add(loadedChunk.midBottomFloorLayout);
        floorLayouts.Add(loadedChunk.bottomFloorLayout);

        float spawnOffset = player.transform.position.x + floorLength;
        int floorSpaces = loadedChunk.topFloorLayout.Length - 1;
        float spawnDistance = floorLength / floorSpaces / 2;

        int i = 0;
        foreach (ObstacleTypes[] layout in floorLayouts)
        {
            foreach (ObstacleTypes type in layout)
            {
                if (type == ObstacleTypes.Wall)
                {
                    Instantiate(obstaclePrefab, new Vector3(spawnOffset, spawnObstacleYOffset[i], 0), Quaternion.Euler(0, 0, 0));
                }
                if (type == ObstacleTypes.RedEnemy || type == ObstacleTypes.YellowEnemy)
                {
                    GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnOffset, spawnEnemyYOffset[i], 0), Quaternion.Euler(0, 0, 0));
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
                spawnOffset += spawnDistance;
            }
            i++;
        }
    }

    public void EnemyKilled(EnemyScript.EnemyTypes enemyType)
    {
        score += 10;

        if (enemyComboType != enemyType)
        {
            enemiesKilled = 0;
        }

        enemyComboType = enemyType;

        enemiesKilled++;

        if (enemiesKilled >= enemyComboCount)
        {
            if (enemyComboType == EnemyScript.EnemyTypes.Red)
            {
                invincibilityEvent?.Invoke();
            }
            else if (enemyComboType == EnemyScript.EnemyTypes.Yellow)
            {
                DestroyEverything();
            }
            enemiesKilled = 0;
        }
    }

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
