using UnityEngine;

public class BombermanController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 4f;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Power-Up Settings")]
    public LayerMask softBlockLayer; // เลือก Layer ของ Soft Block ใน Inspector

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down; // จำทิศทางล่าสุดสำหรับ Idle
    private bool isDead = false; // ป้องกันการเรียกฟังก์ชันตายซ้ำซ้อน

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (inputX != 0)
        {
            movement = new Vector2(inputX, 0);
        }
        else if (inputY != 0)
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
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // เคลื่อนที่ปกติด้วย Rigidbody2D
        Vector2 targetPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    [Header("Footstep Settings")]
    public float footstepCooldown = 0.3f; // ระยะเวลาหน่วงขั้นต่ำระหว่างเสียงก้าวเท้า (วินาที)
    private float lastFootstepTime;

    // ฟังก์ชันนี้ถูกเรียกโดย Animation Event ในจังหวะก้าวเท้า
    public void PlayFootstepSound()
    {
        if (SoundManager.Instance == null) return;

        // เช็กเวลาว่าผ่านไปถึงกำหนดหรือยัง (ป้องกันเสียงรัวเกินไป)
        if (Time.time - lastFootstepTime < footstepCooldown) return;
        lastFootstepTime = Time.time;

        // เช็กว่ากำลังเดินแนวตั้ง (ขึ้น/ลง) หรือแนวนอน (ซ้าย/ขวา) มากกว่ากัน
        if (Mathf.Abs(lastDirection.y) > Mathf.Abs(lastDirection.x))
        {
            // เดินแนวตั้ง (ใช้เสียง step_vertical)
            if (SoundManager.Instance.stepVerticalSFX != null)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.stepVerticalSFX);
            }
        }
        else
        {
            // เดินแนวนอน (ใช้เสียง step_horizontal)
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

        // คำนวณหา Layer Index จาก LayerMask
        int softBlockLayerIndex = 0;
        int layerValue = softBlockLayer.value;

        while (layerValue > 1)
        {
            layerValue >>= 1;
            softBlockLayerIndex++;
        }

        // สั่งให้ระบบ Physics ปิดการชนกันระหว่าง Player กับ Soft Block
        Physics2D.IgnoreLayerCollision(playerLayer, softBlockLayerIndex, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // เมื่อผู้เล่นสัมผัสกับไฟระเบิด หรือ ศัตรู
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

        // 1. หยุดการเคลื่อนที่
        movement = Vector2.zero;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // ปิดระบบฟิสิกส์ไม่ให้ชนซ้ำ
        }

        // 2. เล่นแอนิเมชันตาย
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 3. เล่นเสียงตาย[cite: 10]
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.playerDeathSFX);
        }

        // 4. แจ้งให้ GameManager จัดการรีเซ็ตฉากหรือลดชีวิตต่อทันที
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }
}