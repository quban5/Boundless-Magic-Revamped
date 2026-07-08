using System;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] private Sprite yellowSprite;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public enum EnemyTypes
    {
        Red,
        Yellow,
        Blue
    }

    void Start()
    {
        GameManager.Instance.AddToEnemyList(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        float playerXPos = GameManager.Instance.GetPlayer().transform.position.x;

        if (playerXPos - 5 > transform.position.x)
        {
            ReturnToPool();
        }
    }

    public void DestroyEnemy()
    {
        SpellManager.Instance.EnemyKilled(enemyType);
        AudioManager.Instance.PlayKillSound();
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        GameManager.Instance.RemoveFromEnemyList(gameObject);
        PoolManager.Instance.ReturnObjectToPool(PoolManager.Instance.GetEnemyPrefab(), gameObject);
    }

    public void SetEnemyType(EnemyTypes enemyType)
    {
        this.enemyType = enemyType;

        switch (enemyType)
        {
            case EnemyTypes.Red:
                spriteRenderer.sprite = redSprite;
                break;
            case EnemyTypes.Yellow:
                spriteRenderer.sprite = yellowSprite;
                break;
            case EnemyTypes.Blue:
                spriteRenderer.sprite = blueSprite;
                break;
        }
    }
}
