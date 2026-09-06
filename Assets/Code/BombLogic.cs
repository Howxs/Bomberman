using UnityEngine;

public class BombLogic : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float fuseTime = 3f;
    public int explosionRadius = 1;
    public LayerMask blockingLayer;

    [Header("Prefabs & Effects")]
    public GameObject explosionPrefab;

    private Collider2D bombCollider;
    private bool hasExploded = false;

    void Start()
    {
        bombCollider = GetComponent<Collider2D>();

        // ตั้งค่าให้เป็น Trigger ในตอนแรก เพื่อไม่ให้ดัน Player
        if (bombCollider != null)
        {
            bombCollider.isTrigger = true;
        }

        Invoke(nameof(Explode), fuseTime);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // เมื่อ Player เดินก้าวออกจากระเบิด ให้เปิด Physical Collision (กลายเป็นกำแพง)
        if (other.CompareTag("Player") && bombCollider != null)
        {
            bombCollider.isTrigger = false;
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        CancelInvoke(nameof(Explode));

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        ExplodeInDirection(Vector2.up);
        ExplodeInDirection(Vector2.down);
        ExplodeInDirection(Vector2.left);
        ExplodeInDirection(Vector2.right);

        Destroy(gameObject);
    }

    private void ExplodeInDirection(Vector2 direction)
    {
        for (int i = 1; i <= explosionRadius; i++)
        {
            Vector2 targetPos = (Vector2)transform.position + (direction * i);
            Collider2D hit = Physics2D.OverlapBox(targetPos, new Vector2(0.8f, 0.8f), 0f, blockingLayer);

            if (hit != null)
            {
                // 1. เช็กว่าเป็นระเบิดลูกอื่นหรือไม่ (Chain Reaction)
                BombLogic otherBomb = hit.GetComponent<BombLogic>();
                if (otherBomb != null)
                {
                    otherBomb.Explode();
                }

                // 2. เช็กว่าเป็น Soft Block ที่ทำลายได้หรือไม่
                Destructible destructible = hit.GetComponent<Destructible>();
                if (destructible != null)
                {
                    destructible.DestroyBlock(); // สั่งทำลายบล็อก
                }

                // ชนบล็อกแล้วให้หยุดการกระจายไฟในทิศทางนี้ทันที
                break;
            }

            // ถ้าเป็นพื้นที่ว่าง ให้สร้างไฟระเบิด
            Instantiate(explosionPrefab, targetPos, Quaternion.identity);
        }
    }


}