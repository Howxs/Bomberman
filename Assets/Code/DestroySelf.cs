using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float lifetime = 0.5f; // เวลาที่จะให้ไฟหายไป (0.5 วินาที)

    void Start()
    {
        // สั่งทำลาย GameObject ตัวนี้ทิ้งอัตโนมัติเมื่อครบเวลา
        Destroy(gameObject, lifetime);
    }
}