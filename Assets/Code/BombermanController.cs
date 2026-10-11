using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BombSpawner))] // มั่นใจว่ามี BombSpawner อยู่ที่ตัวเดียวกัน
public class BombermanController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public LayerMask obstacleLayer; // เลือก Layer ของสิ่งกีดขวางทั้งหมด (Hard Wall, Soft Block, Bomb)

    [Header("Corner Correction Settings")]
    public float rayOffset = 0.25f;     // ระยะห่างของ Raycast ซ้าย-ขวา/บน-ล่าง
    public float rayLength = 0.4f;      // ความยาวในการตรวจจับขอบมุม
    public float cornerSlideSpeed = 2.5f; // ความเร็วสไลด์เข้ามุม

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Power-Up Settings")]
    public LayerMask softBlockLayer; // เลือก Layer ของ Soft Block ใน Inspector

    [Header("Footstep Settings")]
    public float footstepCooldown = 0.3f; // ระยะเวลาหน่วงขั้นต่ำระหว่างเสียงก้าวเท้า (วินาที)
    private float lastFootstepTime;

    private Rigidbody2D rb;
    private BombSpawner bombSpawner;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down; // จำทิศทางล่าสุดสำหรับ Idle
    private bool isDead = false; // ป้องกันการเรียกฟังก์ชันตายซ้ำซ้อน

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bombSpawner = GetComponent<BombSpawner>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return; // ถ้าตายแล้ว ไม่รับปุ่มบังคับอีก

        // 1. รับค่าการกดปุ่มบังคับ (WASD / Arrow Keys)
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // 2. ล็อกไม่ให้เดินเฉียง (เดินได้ทีละแกนสไตล์ NES)
        if (Mathf.Abs(inputX) > 0.1f)
        {
            movement = new Vector2(inputX, 0);
        }
        else if (Mathf.Abs(inputY) > 0.1f)
        {
            movement = new Vector2(0, inputY);
        }
        else
        {
            movement = Vector2.zero;
        }

        // 3. คำนวณสถานะการเคลื่อนที่
        bool isMoving = movement != Vector2.zero;

        if (isMoving)
        {
            lastDirection = movement; // อัปเดตทิศทางล่าสุดเมื่อมีการเดิน
        }

        // 4. ส่งค่า Parameter ไปยัง Animator
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);

            // ส่งค่าทิศทางล่าสุดเพื่อให้ Blend Tree หรือ Idle ทำงานตรงทิศ
            animator.SetFloat("MoveX", lastDirection.x);
            animator.SetFloat("MoveY", lastDirection.y);
        }

        // 5. กลับด้าน Sprite เมื่อเดินไปทางซ้าย (ถ้าใช้ Sprite หันข้างรูปเดียว)
        if (lastDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (lastDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        // หมายเหตุ: ระบบวางระเบิด (Drop Key) และจุดชนวน (Detonate Key) 
        // ถูกจัดการไว้ในสคริปต์ BombSpawner ที่อยู่บนตัวผู้เล่นเรียบร้อยแล้วใน Update ของมันเองครับ
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // เคลื่อนที่ด้วย Rigidbody2D + ระบบ Corner Correction ช่วยเลื่อนเข้ามุม
        if (movement != Vector2.zero)
        {
            Vector2 adjustedMove = ApplyCornerCorrection(movement);
            Vector2 targetPosition = rb.position + adjustedMove * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- ระบบ Corner Correction (ช่วยเลื่อนเข้ามุมเมื่อเดินติดเหลี่ยม) ---
    private Vector2 ApplyCornerCorrection(Vector2 inputDir)
    {
        Vector2 pos = rb.position;

        // เช็กการเดินแนวนอน (ซ้าย/ขวา)
        if (inputDir.x != 0)
        {
            Vector2 topRayOrigin = pos + new Vector2(0, rayOffset);
            Vector2 bottomRayOrigin = pos + new Vector2(0, -rayOffset);

            bool topHit = Physics2D.Raycast(topRayOrigin, new Vector2(inputDir.x, 0), rayLength, obstacleLayer);
            bool bottomHit = Physics2D.Raycast(bottomRayOrigin, new Vector2(inputDir.x, 0), rayLength, obstacleLayer);

            if (topHit && !bottomHit)
            {
                return new Vector2(inputDir.x, -1f * cornerSlideSpeed).normalized;
            }
            else if (!topHit && bottomHit)
            {
                return new Vector2(inputDir.x, 1f * cornerSlideSpeed).normalized;
            }
        }
        // เช็กการเดินแนวตั้ง (ขึ้น/ลง)
        else if (inputDir.y != 0)
        {
            Vector2 leftRayOrigin = pos + new Vector2(-rayOffset, 0);
            Vector2 rightRayOrigin = pos + new Vector2(rayOffset, 0);

            bool leftHit = Physics2D.Raycast(leftRayOrigin, new Vector2(0, inputDir.y), rayLength, obstacleLayer);
            bool rightHit = Physics2D.Raycast(rightRayOrigin, new Vector2(0, inputDir.y), rayLength, obstacleLayer);

            if (leftHit && !rightHit)
            {
                return new Vector2(1f * cornerSlideSpeed, inputDir.y).normalized;
            }
            else if (!leftHit && rightHit)
            {
                return new Vector2(-1f * cornerSlideSpeed, inputDir.y).normalized;
            }
        }

        return inputDir;
    }

    // ฟังก์ชันนี้ถูกเรียกโดย Animation Event ในจังหวะก้าวเท้า
    public void PlayFootstepSound()
    {
        if (SoundManager.Instance == null) return;

        if (Time.time - lastFootstepTime < footstepCooldown) return;
        lastFootstepTime = Time.time;

        if (Mathf.Abs(lastDirection.y) > Mathf.Abs(lastDirection.x))
        {
            if (SoundManager.Instance.stepVerticalSFX != null)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.stepVerticalSFX);
            }
        }
        else
        {
            if (SoundManager.Instance.stepHorizontalSFX != null)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.stepHorizontalSFX);
            }
        }
    }

    // เรียกฟังก์ชันนี้เมื่อเก็บไอเทม Wall Pass
    public void EnableWallPass()
    {
        int playerLayer = gameObject.layer;

        int softBlockLayerIndex = 0;
        int layerValue = softBlockLayer.value;

        while (layerValue > 1)
        {
            layerValue >>= 1;
            softBlockLayerIndex++;
        }

        Physics2D.IgnoreLayerCollision(playerLayer, softBlockLayerIndex, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Explosion") || other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    // ฟังก์ชันจัดการความตายแบบปลอดภัย ไม่ใช้ Coroutine ในตัว Player
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        movement = Vector2.zero;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.playerDeathSFX);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }
}