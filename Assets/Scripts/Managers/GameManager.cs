using System;
using System.Collections.Generic;
//using UnityEditor.U2D.Aseprite;
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
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    public static event Action gameOverEvent;
    public static event Action startRunEvent;
    public static event Action returnToTitleScreenEvent;
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

    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    [Header("Score")]
    [SerializeField] private float score;

    private bool justLoaded;

    private bool isMainScene;
    public static class SceneNames
    {
        public const string main = "MainScene";
        public const string titleScreen = "TitleScreen";
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
        isMainScene = scene.name == SceneNames.main;
        justLoaded = true;
    }

    void Start()
    {
        SpellManager.curseEvent += Curse;
        ChunkManager.checkSpeedCapsEvent += CheckSpeedCaps;

        if (isMainScene)
        {
            AudioManager.Instance.PlayMainTheme();
        } else
        {
            AudioManager.Instance.PlayTitleTheme();
        }
    }

    void Update()
    {
        if (isMainScene)
        {
            if (!isMaxSpeedReached)
            {
                IncreaseSpeed();
            }

            if (PlayerInputManager.Instance.restartClicked)
            {
                string currentScene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentScene);
            }
        } else
        {
            if (!justLoaded)
            {
                returnToTitleScreenEvent?.Invoke();
            }
            justLoaded = false;
        }
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

    public void CheckSpeedCaps(int loopIterations)
    {
        foreach (var cap in speedCaps)
        {
            if (loopIterations >= cap.requiredNumberOfChunkLoopIterations)
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

    public void AddScore(float amount)
    {
        score += amount;
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

    public void StartGame()
    {
        SceneManager.LoadScene(SceneNames.main);
        startRunEvent?.Invoke();
        horizontalSpeed = initialHorizontalSpeed;
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + score);
        ResetHorizontalSpeed();
        gameOverEvent?.Invoke();
        score = 0;
        currentTargetSpeedCap = initialHorizontalSpeed;
        isMaxSpeedReached = false;
        enemyList.Clear();
        SceneManager.LoadScene(SceneNames.titleScreen);
    }
    private void OnDestroy()
    {
        SpellManager.curseEvent -= Curse;
        ChunkManager.checkSpeedCapsEvent -= CheckSpeedCaps;
    }
}
