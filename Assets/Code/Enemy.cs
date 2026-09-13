using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. ถ้าชนกับไฟระเบิด -> ศัตรูตาย
        if (other.CompareTag("Explosion"))
        {
            Die();
        }
        // 2. ถ้าชนกับ Player -> Player ตาย
        else if (other.CompareTag("Player"))
        {
            // เรียกฟังก์ชันจัดการการตายของ Player (หรือส่ง Log ก่อนในตอนนี้)
            Debug.Log("Player ถูกศัตรูชน! Game Over / Lose Life");
            // Destroy(other.gameObject); // ปลดล็อกบรรทัดนี้เมื่อมีระบบ Respawn/Game Over
        }
    }

    public void Die()
    {
        // ในอนาคตสามารถใส่ Animation หรือเสียงตอนศัตรูตายตรงนี้ได้
        Debug.Log("Enemy Destroyed!");
        Destroy(gameObject);
    }
}