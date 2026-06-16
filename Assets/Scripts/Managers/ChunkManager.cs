using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

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

    public static event Action<int> checkSpeedCapsEvent;

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
    [SerializeField]
    private List<ChunkDifficulties> chunkLoopOrder = new List<ChunkDifficulties>
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

    private void Start()
    {
        GameManager.gameOverEvent += ResetChunkData;
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
        if (isFirstChunkToSpawn)
        {
            checkSpeedCapsEvent?.Invoke(numberOfChunkLoopIterations);
            isFirstChunkToSpawn = false;
        }
        else if (currentChunkLoopIndex == 0)
        {
            numberOfChunkLoopIterations++;
            Debug.Log(numberOfChunkLoopIterations + " chunk iterations");
            checkSpeedCapsEvent?.Invoke(numberOfChunkLoopIterations);
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

    public void ResetChunkData()
    {
        currentChunkLoopIndex = 0;
        numberOfChunkLoopIterations = 0;
        isFirstChunkToSpawn = true;
        spawnedChunks.Clear();
    }
    private void OnDestroy()
    {
        GameManager.gameOverEvent -= ResetChunkData;
    }
}
