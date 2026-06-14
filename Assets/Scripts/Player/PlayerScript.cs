using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Components
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float speedIncrease;
    [SerializeField] private float[] layerPositionValues;
    [SerializeField] private bool dead;
    [SerializeField] private bool invincible;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private SpriteRenderer[] runeSprites;
    [SerializeField] private GameObject runeHolder;
    [SerializeField] private GameObject fireballPrefab;

    [Header("Powerups")]
    private bool timeSlowActivated;
    [SerializeField] private float timeSlowDuration;
    [SerializeField] private float timeSlowMinScale;
    private float timeSlowScale = 1;
    private float timeSlowTimer;
    [SerializeField] private GameObject shield;
    private bool shieldActive;
    [SerializeField] private GameObject sword;
    private bool swordActive;

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
        GameManager.fireballEvent += SpawnFireball;
        GameManager.timeslowEvent += ActivateTimeSlow;
        GameManager.shieldEvent += ActivateShield;
        GameManager.swordEvent += ActivateSword;
        GameManager.Instance.SetPlayer(gameObject);
        dead = false;

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
        timeSlowScale = timeSlowMinScale;
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
            projectileScript.addSpeed(GameManager.Instance.horizontalSpeed + 5f);
        }
    }

    public void SpawnFireball()
    {
        Instantiate(fireballPrefab, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
    }

    private Coroutine invincibilityCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!invincible)
            {
                if (swordActive)
                {
                    Destroy(collision.gameObject);
                } 
                else if (shieldActive)
                {
                    DeactivateShield();
                    Destroy(collision.gameObject);
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
                } 
                else if (swordActive) 
                {
                    DeactivateSword();
                    Destroy(collision.gameObject);
                } 
                else 
                {
                    Die();
                }
            }
        }
        if (collision.gameObject.CompareTag("SpawnChunkTrigger"))
        {
            GameManager.Instance.SpawnChunk();
        }
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
                        runeSprite.color = Color.red;
                        break;
                    case GameManager.RuneTypes.Yellow:
                        runeSprite.color = Color.yellow;
                        break;
                    case GameManager.RuneTypes.Blue:
                        runeSprite.color = Color.blue;
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
