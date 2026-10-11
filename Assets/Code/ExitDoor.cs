using UnityEngine;
using System.Collections;

public class ExitDoor : MonoBehaviour
{
    [Header("Penalty Settings")]
    [SerializeField] private GameObject enemyPrefab; // Prefab ศัตรูที่จะเสกเมื่อประตูโดนไฟ
    [SerializeField] private int enemiesToSpawn = 1;   // จำนวนศัตรูที่จะเสกโทษฐานโดนไฟ
    [SerializeField] private float spawnDelay = 1.0f;  // หน่วงเวลา 1 วินาทีก่อนศัตรูเกิด

    private bool hasTriggeredPenalty = false; // ตัวแปรป้องกันไม่ให้ทำงานซ้ำซ้อนจากไฟหลายชิ้นส่วน

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. เช็กกรณี "ไฟระเบิดโดนประตู" (และเช็กว่ายังไม่เคยถูกทริกเกอร์ในรอบนี้)
        if (other.CompareTag("Explosion") && !hasTriggeredPenalty)
        {
            hasTriggeredPenalty = true;
            TriggerExplosionPenalty();
            return;
        }

        // 2. เช็กกรณี "ผู้เล่นเดินเข้าประตู"
        if (other.CompareTag("Player"))
        {
            Enemy[] remainingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

            if (remainingEnemies.Length == 0)
            {
                GameManager.Instance.StageClear();
                Debug.Log("Stage Clear! All enemies defeated.");
            }
            else
            {
                Debug.Log($"Door Locked! Remaining enemies: {remainingEnemies.Length}");
            }
        }
    }

    private void TriggerExplosionPenalty()
    {
        Debug.Log("Exit Door was hit by explosion! Starting spawn penalty delay...");
        StartCoroutine(SpawnPenaltyRoutine());
    }

    private IEnumerator SpawnPenaltyRoutine()
    {
        // รอให้ไฟระเบิดรอบๆ จางหายไปก่อน (1 วินาที)
        yield return new WaitForSeconds(spawnDelay);

        if (enemyPrefab != null)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            }
            Debug.Log("Penalty enemies spawned successfully after delay!");
        }
        else
        {
            Debug.LogWarning("Enemy Prefab is not assigned in ExitDoor!");
        }

        // (ทางเลือก) รีเซ็ตสถานะเผื่ออนาคตระเบิดโดนซ้ำรอบสอง แต่เกมทั่วไปมักจะจบแค่นี้
        // hasTriggeredPenalty = false; 
    }
}