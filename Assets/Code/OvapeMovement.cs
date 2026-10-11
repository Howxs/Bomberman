using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OvapeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.8f;
    public LayerMask solidWallLayer; // เลือกเฉพาะกำแพงแข็ง (Hard Wall) เท่านั้น ไม่รวม SoftBlock

    [Header("Components")]
    public SpriteRenderer spriteRenderer;

    [Header("Sprite Sheets (เดินทิศละ 2 ภาพ)")]
    public Sprite[] spritesUp;
    public Sprite[] spritesDown;
    public Sprite[] spritesLeft;
    public Sprite[] spritesRight;
    public float animationSpeed = 0.2f;

    [Header("Death & Score Settings")]
    public Sprite deathSprite;
    [SerializeField] private GameObject scorePopupPrefab;

    private Vector2 currentDir;
    private bool isMoving = false;
    private bool isDead = false;
    private int currentAnimationFrame = 0;
    private float animationTimer;
    private Coroutine moveCoroutine;

    private readonly Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        ChooseRandomDirection();
    }

    void Update()
    {
        if (isDead) return;

        if (!isMoving)
        {
            if (CanMoveInDirection(currentDir))
            {
                Vector2 targetPos = (Vector2)transform.position + currentDir;
                moveCoroutine = StartCoroutine(MoveToTile(targetPos));
            }
            else
            {
                ChooseRandomDirection();
            }
        }

        UpdateSpriteAnimation();
    }

    private IEnumerator MoveToTile(Vector2 target)
    {
        isMoving = true;

        while (Vector2.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        isMoving = false;

        if (Random.value < 0.4f) // มีโอกาสเปลี่ยนทิศทางบ่อยขึ้นเล็กน้อยเมื่อทะลุบล็อก
        {
            ChooseRandomDirection();
        }
    }

    private bool CanMoveInDirection(Vector2 dir)
    {
        // เช็กชนเฉพาะกำแพงแข็ง (Hard Wall) ทำให้ทะลุ SoftBlock ได้เพราะไม่เอาเข้ามาใน LayerMask นี้
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.8f, solidWallLayer);
        return hit.collider == null;
    }

    private void ChooseRandomDirection()
    {
        List<Vector2> validDirs = new List<Vector2>();
        foreach (Vector2 dir in possibleDirections)
        {
            if (CanMoveInDirection(dir)) validDirs.Add(dir);
        }

        if (validDirs.Count > 0)
        {
            currentDir = validDirs[Random.Range(0, validDirs.Count)];
        }
        else
        {
            currentDir = Vector2.zero;
        }
    }

    private void UpdateSpriteAnimation()
    {
        if (spriteRenderer == null) return;

        animationTimer += Time.deltaTime;
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;
            currentAnimationFrame = (currentAnimationFrame + 1) % 2;
        }

        Sprite[] currentSpriteSet = null;

        if (currentDir == Vector2.up) currentSpriteSet = spritesUp;
        else if (currentDir == Vector2.down) currentSpriteSet = spritesDown;
        else if (currentDir == Vector2.left) currentSpriteSet = spritesLeft;
        else if (currentDir == Vector2.right) currentSpriteSet = spritesRight;

        if (currentSpriteSet != null && currentSpriteSet.Length >= 2)
        {
            spriteRenderer.sprite = currentSpriteSet[currentAnimationFrame];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Explosion"))
        {
            StartCoroutine(DieRoutine());
        }
        else if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;
        moveSpeed = 0f;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (spriteRenderer != null && deathSprite != null)
        {
            spriteRenderer.sprite = deathSprite;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(200); // ให้คะแนน Ovape สูงขึ้นนิดหน่อย (เช่น 200)
        }

        if (scorePopupPrefab != null)
        {
            GameObject popup = Instantiate(scorePopupPrefab, transform.position, Quaternion.identity);
            Destroy(popup, 2.0f);
        }

        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}