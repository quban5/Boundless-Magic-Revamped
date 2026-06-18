using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Components
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float[] layerPositionValues;

    [Header("Projectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject fireballPrefab;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration;

    [Header("Runes")]
    [SerializeField] private SpriteRenderer[] runeSprites;

    [Header("Powerups")]
    [SerializeField] private float timeSlowDuration;
    [SerializeField] private float timeSlowMinScale;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject sword;

    //Runtime state
    private bool dead;
    private bool invincible;
    private bool timeSlowActivated;
    private float timeSlowTimer;
    private bool shieldActive;
    private bool swordActive;
    private Color yellowColor;
    private Color redColor;
    private Color blueColor;

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
        SpellManager.invincibilityEvent += StartInvincibility;
        SpellManager.fireballEvent += SpawnFireball;
        SpellManager.timeslowEvent += ActivateTimeSlow;
        SpellManager.shieldEvent += ActivateShield;
        SpellManager.swordEvent += ActivateSword;
        GameManager.Instance.SetPlayer(this);
        dead = false;
        yellowColor = new Color(0.98f, 0.95f, 0.21f, 0.16f);
        redColor = new Color(0.67f, 0.2f, 0.2f, 0.16f);
        blueColor = new Color(0.39f, 0.61f, 1f, 0.16f);

        foreach (SpriteRenderer rune in runeSprites)
        {
            rune.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!dead)
        {
            Movement();
            Shooting();
            CalculateTimeSlow();
            //runeHolder.transform.RotateAround(runeHolder.transform.position, Vector3.forward, 360 * Time.deltaTime);

        }
    }

    public void ActivateTimeSlow()
    {
        timeSlowActivated = true;
        timeSlowTimer = timeSlowDuration;
    }

   private void CalculateTimeSlow()
    {
        if (timeSlowActivated)
        {
            timeSlowTimer -= Time.deltaTime;
            if (timeSlowTimer <= 0)
            {
                timeSlowActivated = false;
                GameManager.Instance.SetTimeSlowScale(1f); // Reset skali w Managerze
            }
            else
            {
                float progress = (timeSlowDuration - timeSlowTimer) / timeSlowDuration;
                float currentScale = Mathf.Max(progress, timeSlowMinScale);
                GameManager.Instance.SetTimeSlowScale(currentScale); // Aktualizacja skali
            }
        }
    }

    public void ActivateShield()
    {
        DeactivateSword();
        shield.SetActive(true);
        shieldActive = true;
    }

    public void ActivateSword()
    {
        DeactivateShield();
        sword.SetActive(true);
        swordActive = true;
    }

    private void DeactivateShield()
    {
        shield.SetActive(false);
        shieldActive = false;
    }

    private void DeactivateSword()
    {
        sword.SetActive(false);
        swordActive = false;
    }

    private void Movement()
    {
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
        transform.position = new Vector3(currentPos.x, lerpedYPos, currentPos.z);

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
            projectileScript.AddSpeed(GameManager.Instance.horizontalSpeed + 5f);
        }
    }

    public void SpawnFireball()
    {
        Instantiate(fireballPrefab, transform.position + Vector3.right, Quaternion.identity);
    }

    private Coroutine invincibilityCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyScript enemyScript = collision.gameObject.GetComponent<EnemyScript>();
            if (!invincible)
            {
                if (swordActive)
                {
                    enemyScript?.DestroyEnemy();
                    GameManager.Instance.AddScore(2);
                }
                else if (shieldActive)
                {
                    DeactivateShield();
                    enemyScript?.DestroyEnemy();
                    GameManager.Instance.AddScore(2);
                }
                else
                {
                    Die();
                }
            }
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!invincible)
            {
                if (shieldActive)
                {
                    Destroy(collision.gameObject);
                    GameManager.Instance.AddScore(1);
                }
                else if (swordActive)
                {
                    DeactivateSword();
                    Destroy(collision.gameObject);
                    GameManager.Instance.AddScore(1);
                }
                else
                {
                    Die();
                }
            }
        }
        if (collision.gameObject.CompareTag("SpawnChunkTrigger"))
        {
            ChunkManager.Instance.SpawnChunk();
        }
    }

    private void OnDestroy()
    {
        SpellManager.invincibilityEvent -= StartInvincibility;
        SpellManager.fireballEvent -= SpawnFireball;
        SpellManager.timeslowEvent -= ActivateTimeSlow;
        SpellManager.shieldEvent -= ActivateShield;
        SpellManager.swordEvent -= ActivateSword;
    }

    private void Die()
    {
        dead = true;
        GameManager.Instance.GameOver();
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
        spriteRenderer.color = Color.gray;

        float warningDuration = 2f;
        float blinkInterval = 0.2f; 

        if (invincibilityDuration > warningDuration)
        {
            yield return new WaitForSeconds(invincibilityDuration - warningDuration);
        }

 
        float elapsed = 0f;
        while (elapsed < warningDuration)
        {
         
            if (spriteRenderer.color == Color.gray)
                spriteRenderer.color = Color.white;
            else
                spriteRenderer.color = Color.gray;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        
        invincible = false;
        spriteRenderer.color = Color.white;
        invincibilityCoroutine = null;
    }

    public void SetRuneSprites(List<GameManager.RuneTypes> runes)
    {
        int i = 0;
        foreach (SpriteRenderer runeSprite in runeSprites)
        {
            if (runes.Count > i)
            {
                switch (runes[i])
                {
                    case GameManager.RuneTypes.Red:
                        runeSprite.color = redColor;
                        break;
                    case GameManager.RuneTypes.Yellow:
                        runeSprite.color = yellowColor;
                        break;
                    case GameManager.RuneTypes.Blue:
                        runeSprite.color = blueColor;
                        break;
                }
                runeSprite.gameObject.SetActive(true);
                i++;
            }
            else
            {
                runeSprite.gameObject.SetActive(false);
            }
        }
    }
}
