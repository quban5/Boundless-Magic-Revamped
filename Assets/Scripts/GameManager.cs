using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action magicToggleEvent;

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

    [Header("Magic")]
    [SerializeField] private bool magicActivated;
    [SerializeField] private int ammo;

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

        //Toggle the magic
        if (PlayerInputManager.Instance.magicClicked)
        {
            magicActivated = !magicActivated;
            magicToggleEvent?.Invoke();

            if (magicActivated)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnInterval);

        int randomVal = UnityEngine.Random.Range(0, 2);

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

    public bool GetMagicActivated()
    {
        return magicActivated;
    }

    public int GetAmmo()
    {
        return ammo;
    }

    public void AddAmmo(int ammo)
    {
        this.ammo += ammo;
    }
}
