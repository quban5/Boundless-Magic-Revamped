using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnpointScript : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float distanceToSpawn = 8.5f;
    private float playerDistance;
    private Dictionary<SpellManager.RuneTypes, float> runeChanceThresholds;

    void Start()
    {
        player = GameManager.Instance.GetPlayer().gameObject;
        runeChanceThresholds = SpellManager.Instance.GetRuneChanceThresholds();
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

        if (randomEnemyType <= runeChanceThresholds[SpellManager.RuneTypes.Red]) {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Red);
        } else if (randomEnemyType <= runeChanceThresholds[SpellManager.RuneTypes.Yellow]) {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Yellow);
        } else {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Blue);
        }



        Destroy(gameObject);
    }
}
