using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] Sprite yellowSprite;
    [SerializeField] Sprite redSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public enum EnemyTypes
    {
        Red,
        Yellow
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        health = maxHealth;
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

        if (enemyType == EnemyTypes.Red)
        {
            spriteRenderer.sprite = redSprite;
        }
        else if (enemyType == EnemyTypes.Yellow)
        {
            spriteRenderer.sprite = yellowSprite;
        }
    }
}
