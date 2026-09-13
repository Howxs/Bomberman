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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
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
        // เคลื่อนที่ด้วย Rigidbody2D เพื่อให้ชนกับ Wall และ Block
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
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
}