using UnityEngine;

public class PlayerInnerCollisionScript : MonoBehaviour
{
    private PlayerScript playerScript;
    private void Start()
    {
        playerScript = GetComponentInParent<PlayerScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerScript.OnCollision(collision, true);
    }
}
