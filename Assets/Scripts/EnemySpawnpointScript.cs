using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class EnemySpawnpointScript : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float playerDistance;
    [SerializeField] private float distanceToSpawn;
    private Dictionary<RuneTypes, float> runeChanceThresholds;
    void Start()
    {
        player = GameManager.Instance.GetPlayer();
        runeChanceThresholds = GameManager.Instance.GetRuneChanceThresholds();
    }

    void Update()
    {
        if (player != null)
        {
            playerDistance = transform.position.x - player.transform.position.x;
        }
        if (playerDistance < distanceToSpawn)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        EnemyScript enemyScript = spawnedEnemy.GetComponent<EnemyScript>();
        float randomEnemyType = Random.Range(0f, 1f);

        if (randomEnemyType <= runeChanceThresholds[RuneTypes.Red]) {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Red);
        } else if (randomEnemyType <= runeChanceThresholds[RuneTypes.Yellow]) {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Yellow);
        } else {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Blue);
        }



        Destroy(gameObject);
    }
}
