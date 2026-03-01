using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Components
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float speedIncrease;
    [SerializeField] private float[] layerPositionValues;
    [SerializeField] private bool dead;
    [SerializeField] private bool invincible;
    [SerializeField] private float invincibilityDuration;

    private enum layerPositions
    {
        Top = 0,
        Middle = 1,
        Bottom = 2
    }

    private layerPositions layerPosition;

    void Start()
    {
        layerPosition = layerPositions.Middle;
        rb = GetComponent<Rigidbody2D>();
        GameManager.invincibilityEvent += StartInvincibility;
    }

    void Update()
    {
        if (!dead)
        {
            Movement();
            Shooting();
            horizontalSpeed += Time.deltaTime * speedIncrease;
        }
    }

    private void Movement()
    {
        //Move right
        rb.linearVelocity = new Vector2(horizontalSpeed, rb.linearVelocity.y);
        //Move up
        if (PlayerInputManager.Instance.upClicked)
        {
            if ((int)layerPosition > 0)
            {
                layerPosition -= 1;
            }
        }
        //Move down
        if (PlayerInputManager.Instance.downClicked)
        {
            if ((int)layerPosition < 2)
            {
                layerPosition += 1;
            }
        }
        //Align to layer position
        Vector3 currentPos = transform.position;
        float targetYPos = layerPositionValues[(int)layerPosition];
        Debug.Log(targetYPos);
        float lerpedYPos = Mathf.Lerp(currentPos.y, targetYPos, verticalSpeed * Time.deltaTime);
        transform.position = new Vector3(currentPos.x, lerpedYPos);
    }

    private void Shooting()
    {
        if (PlayerInputManager.Instance.ShootClicked)
        {
            GameObject newBullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
            ProjectileScript projectileScript = newBullet.GetComponent<ProjectileScript>();
            projectileScript.addSpeed(horizontalSpeed);
        }
    }
    private Coroutine invincibilityCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            if (!invincible)
            {
                horizontalSpeed = 0;
                rb.linearVelocity = new Vector2(0, 0);
                dead = true;
            } else
            {
                invincible = false;
                renderer.color = Color.white;
                StopCoroutine(invincibilityCoroutine);
                invincibilityCoroutine = null;
            }
        }
    }

    public void StartInvincibility()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        invincibilityCoroutine = StartCoroutine(InvincibilityTimer());
    }

    IEnumerator InvincibilityTimer()
    {
        invincible = true;
        renderer.color = Color.gray;

        float warningDuration = 2f;
        float blinkInterval = 0.2f; 

        if (invincibilityDuration > warningDuration)
        {
            yield return new WaitForSeconds(invincibilityDuration - warningDuration);
        }

 
        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
         
            if (renderer.color == Color.gray)
                renderer.color = Color.white;
            else
                renderer.color = Color.gray;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        
        invincible = false;
        renderer.color = Color.white;
        invincibilityCoroutine = null;
    }
}
