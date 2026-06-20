using UnityEngine;

public class PlayerOuterCollisionScript : MonoBehaviour
{
    private PlayerScript playerScript;
    private void Start()
    {
        playerScript = GetComponentInParent<PlayerScript>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerScript.OnCollision(collision, false);
    }
}
