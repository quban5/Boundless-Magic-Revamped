using UnityEngine;

public class EnemySpawnpointScript : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float playerDistance;
    [SerializeField] private float distanceToSpawn;
    void Start()
    {
        player = GameManager.Instance.GetPlayer();
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

        if (randomEnemyType <= 0.4f)
        {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Red);
        }
        else if (randomEnemyType > 0.4f && randomEnemyType <= 0.7f)
        {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Blue);
        }
        else if (randomEnemyType > 0.7f && randomEnemyType <= 0.9f)
        {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Green);
        }
        else
        {
            enemyScript.SetEnemyType(EnemyScript.EnemyTypes.Yellow);
        }



        Destroy(gameObject);
    }
}
