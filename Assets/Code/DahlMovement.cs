using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DahlMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3.0f; // ความเร็วสูงกว่าตัวปกติ
    public LayerMask obstacleLayer; // กำแพงและบล็อกทั่วไป

    [Header("Components")]
    public SpriteRenderer spriteRenderer;

    [Header("Sprite Sheets (เดินทิศละ 2 ภาพ)")]
    public Sprite[] spritesUp;
    public Sprite[] spritesDown;
    public Sprite[] spritesLeft;
    public Sprite[] spritesRight;
    public float animationSpeed = 0.15f; // ขยับเร็วขึ้นตามความไว

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

        // เปลี่ยนทิศทางน้อยลงหน่อย เพื่อให้มันไหลไปในทิศทางเดิมนานขึ้น (ชอบเดินยาวๆ ซ้าย-ขวา)
        if (Random.value < 0.2f)
        {
            ChooseRandomDirection();
        }
    }

    private bool CanMoveInDirection(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.8f, obstacleLayer);
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
            // เพิ่มน้ำหนักให้ชอบเลือกเดิน ซ้าย หรือ ขวา มากกว่า บน หรือ ล่าง
            if (validDirs.Contains(Vector2.left) && Random.value < 0.35f) currentDir = Vector2.left;
            else if (validDirs.Contains(Vector2.right) && Random.value < 0.35f) currentDir = Vector2.right;
            else
            {
                currentDir = validDirs[Random.Range(0, validDirs.Count)];
            }
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
            GameManager.Instance.AddScore(400); // Dahl ได้ 400 คะแนนตามข้อมูลดั้งเดิม
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