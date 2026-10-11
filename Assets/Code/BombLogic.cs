using UnityEngine;

public class BombLogic : MonoBehaviour
{
    public float fuseTime = 3f;
    public int explosionRadius = 1;
    public bool isRemote = false;
    public LayerMask blockingLayer;

    [Header("Explosion Prefabs")]
    public GameObject explosionCenterPrefab; // ไฟตรงกลาง
    public GameObject explosionUpPrefab;     // ปลายไฟด้านบน
    public GameObject explosionDownPrefab;   // ปลายไฟด้านล่าง
    public GameObject explosionLeftPrefab;   // ปลายไฟด้านซ้าย
    public GameObject explosionRightPrefab;  // ปลายไฟด้านขวา

    private float timer;
    private bool hasExploded = false;

    private void Start()
    {
        timer = fuseTime;
    }

    private void Update()
    {
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

        // 1. สร้างไฟระเบิดตรงกลาง
        if (explosionCenterPrefab != null)
        {
            Instantiate(explosionCenterPrefab, transform.position, Quaternion.identity);
        }

        SoundManager.Instance.PlaySFX(SoundManager.Instance.bombExplosionSFX);

        // 2. กระจายไฟปะทุ 4 ทิศทาง พร้อมระบุทิศทาง
        ExplodeInDirection(Vector2.up, explosionUpPrefab);
        ExplodeInDirection(Vector2.down, explosionDownPrefab);
        ExplodeInDirection(Vector2.left, explosionLeftPrefab);
        ExplodeInDirection(Vector2.right, explosionRightPrefab);

        Destroy(gameObject);
    }

    private void ExplodeInDirection(Vector2 direction, GameObject endPrefab)
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

                break; // ชนสิ่งกีดขวางแล้วหยุดกระจายไฟทิศนั้น
            }

            // ถ้าช่องนี้ว่าง ให้สร้าง Prefab ของไฟตามทิศทางนั้นๆ
            if (endPrefab != null)
            {
                Instantiate(endPrefab, targetPos, Quaternion.identity);
            }
        }
    }
}