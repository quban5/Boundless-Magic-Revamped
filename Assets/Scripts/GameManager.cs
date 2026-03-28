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

    [Header("Objects")]
    [Tooltip("Object holding the sprites of all floors")]
    [SerializeField] private GameObject ground;
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
    [SerializeField] private GameObject chunkEntry;

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

    }

    void Update()
    {
        //Restarting the game when the restart button is clicked
        if (PlayerInputManager.Instance.restartClicked)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentScene);
        }
    }

    public void SpawnChunk()
    {
        //Moving the chunk entry to the position of the next chunk
        chunkEntry.transform.position += new Vector3(chunkLength + distanceBetweenChunks, 0, 0);

        int chunkAmount = chunkList.Length;
        int chunkNumber = UnityEngine.Random.Range(0, chunkAmount - 1);
        GameObject loadedChunk = chunkList[chunkNumber];
        Vector3 spawnPos = new Vector3(chunkEntry.transform.position.x+(chunkLength/2), 0, 0);
        Instantiate(loadedChunk, spawnPos, Quaternion.identity);
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
