using System.Collections;
using UnityEngine;
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

        Instance = this;
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Objects")]
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject camera;

    [Header("Spawning")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnXOffset;
    [SerializeField] private float spawnObstacleYOffset;
    [SerializeField] private float spawnEnemyYOffset;

    [Header("Other")]
    [SerializeField] private float score;

    void Start()
    {
        StartCoroutine(SpawnTimer());
    }

    void Update()
    {
        if (PlayerInputManager.Instance.restartClicked)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnInterval);

        int randomVal = Random.Range(0, 2);

        float spawnPosX = camera.transform.position.x + spawnXOffset;

        if (randomVal == 0)
        {
            float spawnPosY = ground.transform.position.y + spawnObstacleYOffset;
            Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY);
            Instantiate(obstaclePrefab, spawnPos, transform.rotation);
        }
        else
        {
            float spawnPosY = ground.transform.position.y + spawnEnemyYOffset;
            Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY);
            Instantiate(enemyPrefab, spawnPos, transform.rotation);
        }

        StartCoroutine(SpawnTimer());
    }

    public void AddScore(float score)
    {
        this.score += score;
    }

    public float GetScore()
    {
        return score;
    }
}
