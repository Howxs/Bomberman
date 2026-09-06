using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float destructionTime = 0.5f; // เวลาเล่น Animation บล็อกพังก่อนลบทิ้ง

    [Header("Item Drop Settings")]
    [Range(0f, 1f)]
    public float itemDropChance = 0.3f; // โอกาสดรอปไอเทม (30%)
    public GameObject[] itemPrefabs;     // รายการ Prefab ไอเทมที่จะสุ่มดรอป

    private bool isBeingDestroyed = false;

    public void DestroyBlock()
    {
        if (isBeingDestroyed) return;
        isBeingDestroyed = true;

        // ปิด Collider ทันทีเพื่อไม่ให้ไฟระเบิดหรือตัวละครชนซ้ำ
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // สุ่มดรอปไอเทมก่อนทำลายบล็อก
        TrySpawnItem();

        // ทำลาย GameObject บล็อกอิฐทิ้งตามเวลา
        Destroy(gameObject, destructionTime);
    }

    private void TrySpawnItem()
    {
        if (itemPrefabs.Length == 0) return;

        // สุ่มค่า 0.0 - 1.0 ถ้าได้ค่าน้อยกว่าโอกาสที่ตั้งไว้จะดรอปไอเทม
        if (Random.value <= itemDropChance)
        {
            int randomIndex = Random.Range(0, itemPrefabs.Length);
            Instantiate(itemPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
}