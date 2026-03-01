using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action invincibilityEvent;

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
    [SerializeField] private int enemyBonusCount;

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
            Instantiate(enemyPrefab, spawnPos, transform.rotation);
        }
        else
        {
            float spawnPosY = spawnObstacleYOffset[spawnLayerPos];
            Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY);
            Instantiate(obstaclePrefab, spawnPos, transform.rotation);
        }

        StartCoroutine(SpawnTimer());
    }

    public void EnemyKilled()
    {
        score += 10;
        enemiesKilled++;

        if (enemiesKilled >= enemyBonusCount)
        {
            invincibilityEvent?.Invoke();
            enemiesKilled = 0;
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
