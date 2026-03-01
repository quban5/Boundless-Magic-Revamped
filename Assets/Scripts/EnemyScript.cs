using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] int maxHealth;
    [SerializeField] private int health;

    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            GameManager.Instance.EnemyKilled();
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
}
