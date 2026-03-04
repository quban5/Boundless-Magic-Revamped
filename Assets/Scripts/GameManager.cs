using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action invincibilityEvent;

    private List<GameObject> enemyList;
    private List<GameObject> obstacleList;

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

    [Header("Other")]
    [SerializeField] private float score;
    [SerializeField] private int enemiesKilled;
    [SerializeField] private int enemyComboCount;
    [SerializeField] private EnemyScript.EnemyTypes enemyComboType;

    void Start()
    {
        StartCoroutine(SpawnTimer());
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
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnInterval);

        int randomVal = UnityEngine.Random.Range(0, 2);
        int spawnLayerPos = UnityEngine.Random.Range(0, 3);

        float spawnPosX = camera.transform.position.x + spawnXOffset;

        if (randomVal == 0)
        {
            float spawnPosY = spawnEnemyYOffset[spawnLayerPos];
            Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY);
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, transform.rotation);
            enemyList.Add(newEnemy);

            int enemyTypeVal = UnityEngine.Random.Range(0, 2);
            EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
            if (enemyTypeVal == 0)
            {
                enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Red);
            } else if (enemyTypeVal == 1)
            {
                enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Yellow);
            }
        }
        else
        {
            float spawnPosY = spawnObstacleYOffset[spawnLayerPos];
            Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY);
            GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, transform.rotation);
            obstacleList.Add(newObstacle);
        }

        StartCoroutine(SpawnTimer());
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
