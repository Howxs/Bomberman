using UnityEngine;

public class BombLogic : MonoBehaviour
{
    public float fuseTime = 3f;
    public int explosionRadius = 1;
    public bool isRemote = false; // ถ้าเป็น true จะไม่ระเบิดเองตามเวลา
    public LayerMask blockingLayer;
    public GameObject explosionPrefab;

    private float timer;
    private bool hasExploded = false;

    private void Start()
    {
        timer = fuseTime;
    }

    private void Update()
    {
        // ถ้าเป็นระเบิดแบบ Remote จะไม่นับถอยหลัง รอสั่งระเบิดจากปุ่มกดอย่างเดียว
        if (!isRemote && !hasExploded)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Explode();
            }
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // สร้างไฟระเบิดตรงกลาง
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // กระจายไฟปะทุ 4 ทิศทางตาม explosionRadius
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
                BombLogic otherBomb = hit.GetComponent<BombLogic>();
                if (otherBomb != null)
                {
                    otherBomb.Explode();
                }

                Destructible destructible = hit.GetComponent<Destructible>();
                if (destructible != null)
                {
                    destructible.DestroyBlock();
                }

                break; // ชนสิ่งกีดขวางแล้วหยุดกระจายไฟ
            }

            Instantiate(explosionPrefab, targetPos, Quaternion.identity);
        }
    }
}