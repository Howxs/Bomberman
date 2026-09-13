using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Setup (NES Full Stage)")]
    public int width = 31;  // ความกว้างสนาม 31 ช่อง
    public int height = 13; // ความสูงสนาม 13 ช่อง

    [Header("Prefabs")]
    public GameObject groundPrefab;
    public GameObject hardWallPrefab;
    public GameObject softBlockPrefab; // [เพิ่มใหม่]
    public GameObject exitDoorPrefab;  // [เพิ่มใหม่]

    [Header("Soft Block Settings")]
    [Range(0f, 1f)] public float softBlockChance = 0.6f; // โอกาสเกิด Soft Block (60%)

    private List<Vector2> softBlockPositions = new List<Vector2>();

    void Start()
    {
        GenerateGrid();
        SpawnExitDoor();
        SpawnEnemies();
    }

    void GenerateGrid()
    {
        float offsetX = -(width - 1) / 2f;
        float offsetY = (-(height - 1) / 2f) - 1.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 spawnPos = new Vector2(x + offsetX, y + offsetY);

                // 1. วางพื้นสนามทุกช่อง
                Instantiate(groundPrefab, spawnPos, Quaternion.identity, transform);

                // 2. กำแพงขอบนอกสุด 4 ด้าน
                bool isBorder = (x == 0 || x == width - 1 || y == 0 || y == height - 1);

                // 3. เสาด้านใน (เว้นทางเดินขอบ 1 ช่องรอบทิศ)
                bool isInnerHardWall = (x % 2 == 0 && y % 2 == 0) && !isBorder;

                if (isBorder || isInnerHardWall)
                {
                    Instantiate(hardWallPrefab, spawnPos, Quaternion.identity, transform);
                }
                else
                {
                    // 4. เว้นพื้นที่ปลอดภัยให้ Player จุดเกิด (มุมซ้ายล่าง: x=1,2 และ y=1,2)
                    bool isPlayerSafeZone = (x <= 2 && y <= 2);

                    if (!isPlayerSafeZone)
                    {
                        // 5. สุ่มวาง Soft Block
                        if (Random.value < softBlockChance)
                        {
                            Instantiate(softBlockPrefab, spawnPos, Quaternion.identity, transform);
                            softBlockPositions.Add(spawnPos); // บันทึกตำแหน่งไว้ซ่อนประตู Exit
                        }
                    }
                }
            }
        }
    }

    void SpawnExitDoor()
    {
        // สุ่มเลือกตำแหน่ง Soft Block 1 จุดเพื่อวางประตู Exit ซ่อนไว้ด้านใต้
        if (softBlockPositions.Count > 0 && exitDoorPrefab != null)
        {
            int randomIndex = Random.Range(0, softBlockPositions.Count);
            Vector2 exitPos = softBlockPositions[randomIndex];

            Instantiate(exitDoorPrefab, exitPos, Quaternion.identity, transform);
        }
    }

    [Header("Enemy Setup")]
    public GameObject enemyPrefab;
    public int enemyCount = 3;

    // เรียกใน Start() ต่อจาก SpawnExitDoor()
    void SpawnEnemies()
    {
        List<Vector2> emptyPositions = new List<Vector2>();

        float offsetX = -(width - 1) / 2f;
        float offsetY = (-(height - 1) / 2f) - 1.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // เว้นระยะ Safe Zone ให้ Player (มุมซ้ายล่าง x<=3, y<=3)
                if (x <= 3 && y <= 3) continue;

                Vector2 pos = new Vector2(x + offsetX, y + offsetY);

                // เช็กว่าตรงช่องว่างนี้ไม่มี Wall หรือ Soft Block บังอยู่
                Collider2D hit = Physics2D.OverlapBox(pos, new Vector2(0.8f, 0.8f), 0f);
                if (hit == null)
                {
                    emptyPositions.Add(pos);
                }
            }
        }

        for (int i = 0; i < enemyCount; i++)
        {
            if (emptyPositions.Count == 0) break;

            int randomIndex = Random.Range(0, emptyPositions.Count);
            Vector2 spawnPos = emptyPositions[randomIndex];

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
            emptyPositions.RemoveAt(randomIndex);
        }
    }
}