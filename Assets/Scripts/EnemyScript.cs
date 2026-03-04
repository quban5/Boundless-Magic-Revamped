using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private EnemyTypes enemyType;
    private SpriteRenderer spriteRenderer;

    public enum EnemyTypes
    {
        Red,
        Yellow
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        health = maxHealth;

        if (enemyType == EnemyTypes.Red)
        {
            spriteRenderer.color = Color.red;
        } else if (enemyType == EnemyTypes.Yellow)
        {
            spriteRenderer.color = Color.black;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            GameManager.Instance.EnemyKilled(enemyType);
            Destroy(gameObject);
        }
        float playerXPos = GameManager.Instance.GetPlayer().transform.position.x;
        if (playerXPos-5>transform.position.x)
        {
            Destroy(gameObject);
        }
    }

    public void GetHit()
    {
        health--;
    }

    public void SetEnemyType(EnemyTypes enemyType)
    {
        this.enemyType = enemyType;
    }
}
