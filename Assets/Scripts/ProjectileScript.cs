using System.Collections;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float initialSpeed;
    [SerializeField] private float lifetime;
    [SerializeField] private ProjectileTypes projectileType;

    private enum ProjectileTypes
    {
        Star,
        Fireball
    }

    private void OnEnable()
    {
        StartCoroutine(DeathTimer());
        speed = initialSpeed;
    }

    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
            enemy.DestroyEnemy();
        }

        if (collision.gameObject.CompareTag("Obstacle") && projectileType == ProjectileTypes.Fireball)
        {
            Destroy(collision.gameObject);
            GameManager.Instance.AddScore(1);
        }

        if (!collision.gameObject.CompareTag("Player") && projectileType != ProjectileTypes.Fireball)
        {
            PoolManager.Instance.ReturnObjectToPool(PoolManager.Instance.GetProjectilePrefab(), gameObject);
        }
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void AddSpeed(float speed)
    {
        this.speed += speed;
    }
}
