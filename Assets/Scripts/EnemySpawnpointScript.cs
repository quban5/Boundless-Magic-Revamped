using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class EnemySpawnpointScript : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float distanceToSpawn = 8.5f;
    private float playerDistance;
    private Dictionary<RuneTypes, float> runeChanceThresholds;

    void Start()
    {
        player = Instance.GetPlayer();
        runeChanceThresholds = Instance.GetRuneChanceThresholds();
    }

    void Update()
    {
        if (player != null)
        {
            playerDistance = transform.position.x - player.transform.position.x;

            if (playerDistance < distanceToSpawn)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        if (transform.parent != null)
        {
            spawnedEnemy.transform.SetParent(transform.parent);
        }
        
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
