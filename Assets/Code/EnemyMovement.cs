using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public LayerMask obstacleLayer; // เลือก Wall, SoftBlock, Bomb ใน Inspector

    [Header("Components")]
    public SpriteRenderer spriteRenderer;

    [Header("Sprite Sheets (เดินทิศละ 2 ภาพ)")]
    public Sprite[] spritesUp;
    public Sprite[] spritesDown;
    public Sprite[] spritesLeft;
    public Sprite[] spritesRight;
    public float animationSpeed = 0.2f;

    [Header("Death & Score Settings")]
    public Sprite deathSprite;                    // ภาพตอนตาย
    [SerializeField] private GameObject scorePopupPrefab; // พรีฟับป้ายคะแนน +100

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

        if (Random.value < 0.3f)
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

        // 1. ถ้าโดนระเบิด
        if (other.CompareTag("Explosion"))
        {
            StartCoroutine(DieRoutine());
        }
        // 2. ถ้าชน Player
        else if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerDied();
            }
            Debug.Log("Player ถูกศัตรูชน!");
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;
        moveSpeed = 0f;

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // เปลี่ยนเป็นภาพตอนตาย
        if (spriteRenderer != null && deathSprite != null)
        {
            spriteRenderer.sprite = deathSprite;
        }

        // จัดการเรื่องคะแนนและป้าย Popup
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
        }

        if (scorePopupPrefab != null)
        {
            GameObject popup = Instantiate(scorePopupPrefab, transform.position, Quaternion.identity);
            Destroy(popup, 2.0f);
        }

        // ดีเลย์รอ 1 วินาทีให้ภาพตายทำงาน ค่อยลบ GameObject ทิ้ง
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}