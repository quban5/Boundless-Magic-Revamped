using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Components
    private Rigidbody2D rb;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float jumpForce;
    private bool isGrounded;
    [SerializeField] private float speedIncrease;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement();
        Shooting();

        horizontalSpeed += Time.deltaTime * speedIncrease;
    }

    private void Movement()
    {
        //Move right
        rb.linearVelocity = new Vector2(horizontalSpeed, rb.linearVelocity.y);
        //Jump
        if (PlayerInputManager.Instance.JumpPressed && isGrounded)
        {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void Shooting()
    {
        if (PlayerInputManager.Instance.ShootClicked)
        {
            if (GameManager.Instance.GetAmmo() > 0)
            {
                GameManager.Instance.AddAmmo(-1);
                Instantiate(projectilePrefab, transform.position, transform.rotation);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            horizontalSpeed = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
