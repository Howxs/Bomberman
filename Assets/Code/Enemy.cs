using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Score Popup Settings")]
    [SerializeField] private GameObject scorePopupPrefab; // พรีแอฟาบป้าย +100 ที่จะให้เด้งขึ้นมา

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. ถ้าชนกับไฟระเบิด -> ศัตรูตาย
        if (other.CompareTag("Explosion"))
        {
            GameManager.Instance.AddScore(100); // ได้ 100 คะแนนเมื่อฆ่าศัตรู
            ShowScorePopup();
            Die();
        }
        // 2. ถ้าชนกับ Player -> Player ตาย
        else if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerDied();
            Debug.Log("Player ถูกศัตรูชน! Game Over / Lose Life");
            // Destroy(other.gameObject); // ปลดล็อกบรรทัดนี้เมื่อมีระบบ Respawn/Game Over
        }
    }

    private void ShowScorePopup()
    {
        if (scorePopupPrefab != null)
        {
            // สร้างป้ายคะแนนขึ้นมาที่ตำแหน่งของศัตรูที่ตาย
            GameObject popup = Instantiate(scorePopupPrefab, transform.position, Quaternion.identity);

            // สั่งทำลายป้ายนี้ทิ้งหลังจากผ่านไป 2 วินาที
            Destroy(popup, 2.0f);
        }
    }

    public void Die()
    {
        Debug.Log("Enemy Destroyed!");
        Destroy(gameObject);
    }
}