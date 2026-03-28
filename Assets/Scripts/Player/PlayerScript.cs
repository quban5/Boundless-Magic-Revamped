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
        UpMiddle = 1,
        DownMiddle = 2,
        Bottom = 3,
        OutOfBoundsBottom = 4,
        OutOfBoundsTop = 5
    }

    private layerPositions layerPosition;

    void Start()
    {
        layerPosition = layerPositions.UpMiddle;
        rb = GetComponent<Rigidbody2D>();
        GameManager.invincibilityEvent += StartInvincibility;
        GameManager.Instance.SetPlayer(gameObject);
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
            } else
            {
                layerPosition = layerPositions.OutOfBoundsTop;
            }
        }
        //Move down
        if (PlayerInputManager.Instance.downClicked)
        {
            if ((int)layerPosition < 3)
            {
                layerPosition += 1;
            } else
            {
                layerPosition = layerPositions.OutOfBoundsBottom;
            }
        }
        //Align to layer position
        Vector3 currentPos = transform.position;
        float targetYPos = layerPositionValues[(int)layerPosition];
        float lerpedYPos = Mathf.Lerp(currentPos.y, targetYPos, verticalSpeed * Time.deltaTime);
        transform.position = new Vector3(currentPos.x, lerpedYPos);

        if (layerPosition == layerPositions.OutOfBoundsBottom)
        {
            if (currentPos.y-0.5f < layerPositionValues[(int)layerPositions.OutOfBoundsBottom])
            {
                layerPosition = layerPositions.Top;
                transform.position = new Vector3(currentPos.x,layerPositionValues[(int)layerPositions.OutOfBoundsTop]);
            }
        }

        if (layerPosition == layerPositions.OutOfBoundsTop)
        {
            if (currentPos.y + 0.5f > layerPositionValues[(int)layerPositions.OutOfBoundsTop])
            {
                layerPosition = layerPositions.Bottom;
                transform.position = new Vector3(currentPos.x, layerPositionValues[(int)layerPositions.OutOfBoundsBottom]);
            }
        }
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
        if (collision.gameObject.CompareTag("ChunkEntry"))
        {
            GameManager.Instance.SpawnChunk();
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
