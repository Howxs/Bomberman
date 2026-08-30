using UnityEngine;

public class BombermanController : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // ล็อกไม่ให้เดินเฉียง (เดินได้ทีละทิศทางตามคลาสสิก NES)
        if (moveX != 0) moveY = 0;

        moveInput = new Vector2(moveX, moveY);
    }

    void FixedUpdate()
    {
        if (moveInput != Vector2.zero)
        {
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        }
    }
}