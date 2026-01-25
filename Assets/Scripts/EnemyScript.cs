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
            Destroy(gameObject);
        }
    }

    public void GetHit()
    {
        health--;
    }
}
