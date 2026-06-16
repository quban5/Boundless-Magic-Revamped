using System.Collections;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private ProjectileTypes projectileType;

    private enum ProjectileTypes
    {
        Star,
        Fireball
    }

    void Start()
    {
        StartCoroutine(DeathTimer());
    }

    void Update()
    {
        transform.position = transform.position + new Vector3(speed, 0, 0) * Time.deltaTime;
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
        }

        if (!collision.gameObject.CompareTag("Player") && projectileType != ProjectileTypes.Fireball)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void addSpeed(float speed)
    {
        this.speed += speed;
    }
}
