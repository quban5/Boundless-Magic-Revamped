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
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float speedIncrease;
    [SerializeField] private float[] layerPositionValues;
    [SerializeField] private bool dead;
    [SerializeField] private bool invincible;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private SpriteRenderer[] runeSprites;
    [SerializeField] private GameObject runeHolder;

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
            horizontalSpeed += Time.deltaTime * speedIncrease;
            runeHolder.transform.RotateAround(runeHolder.transform.position, Vector3.forward, 360 * Time.deltaTime);
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
                Die();
            }
            else
            {
                invincible = false;
                renderer.color = Color.white;
                StopCoroutine(invincibilityCoroutine);
                invincibilityCoroutine = null;
                Die();
            }
        }
        if (collision.gameObject.CompareTag("ChunkEntry"))
        {
            GameManager.Instance.SpawnChunk();
        }
    }

    private void Die()
    {
        horizontalSpeed = 0;
        rb.linearVelocity = new Vector2(0, 0);
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
