using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float playerXPos = GameManager.Instance.GetPlayer().transform.position.x;
        if (playerXPos - 5 > transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
