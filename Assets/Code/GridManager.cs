using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnData
{
    public string enemyName = "Enemy";
    public GameObject enemyPrefab;
    public int count = 1;
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Setup (NES Full Stage)")]
    public int width = 31;
    public int height = 13;

    [Header("Prefabs")]
    public GameObject groundPrefab;
    public GameObject hardWallPrefab;
    public List<GameObject> softBlockPrefabs;
    public GameObject exitDoorPrefab;

    [Header("Soft Block Settings")]
    [Range(0f, 1f)] public float softBlockChance = 0.6f;

    [Header("Enemy Setup (ตั้งค่าศัตรูสำหรับ Scene นี้)")]
    // 🎯 ใส่รายชื่อและจำนวนศัตรูที่จะเกิดในด่านนี้ได้เลยทันทีใน Inspector
    public List<EnemySpawnData> enemyList = new List<EnemySpawnData>();

    private List<Vector2> softBlockPositions = new List<Vector2>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

                Instantiate(groundPrefab, spawnPos, Quaternion.identity, transform);

                bool isBorder = (x == 0 || x == width - 1 || y == 0 || y == height - 1);
                bool isInnerHardWall = (x % 2 == 0 && y % 2 == 0) && !isBorder;

                if (isBorder || isInnerHardWall)
                {
                    Instantiate(hardWallPrefab, spawnPos, Quaternion.identity, transform);
                }
                else
                {
                    bool isPlayerSafeZone = (x <= 3 && y <= 1) || (x <= 1 && y <= 3) || (x == 2 && y == 2);

                    if (!isPlayerSafeZone)
                    {
                        if (Random.value < softBlockChance && softBlockPrefabs != null && softBlockPrefabs.Count > 0)
                        {
                            int randomStyleIndex = Random.Range(0, softBlockPrefabs.Count);
                            GameObject selectedSoftBlock = softBlockPrefabs[randomStyleIndex];

                            Instantiate(selectedSoftBlock, spawnPos, Quaternion.identity, transform);
                            softBlockPositions.Add(spawnPos);
                        }
                    }
                }
            }
        }
    }

    void SpawnExitDoor()
    {
        if (softBlockPositions.Count > 0 && exitDoorPrefab != null)
        {
            int randomIndex = Random.Range(0, softBlockPositions.Count);
            Vector2 exitPos = softBlockPositions[randomIndex];

            Instantiate(exitDoorPrefab, exitPos, Quaternion.identity, transform);
        }
    }

    void SpawnEnemies()
    {
        List<Vector2> emptyPositions = new List<Vector2>();

        float offsetX = -(width - 1) / 2f;
        float offsetY = (-(height - 1) / 2f) - 1.5f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x <= 5 && y <= 5) continue;

                Vector2 pos = new Vector2(x + offsetX, y + offsetY);

                Collider2D hit = Physics2D.OverlapBox(pos, new Vector2(0.8f, 0.8f), 0f);
                if (hit == null)
                {
                    emptyPositions.Add(pos);
                }
            }
        }

        // วนลูปสปาวน์ศัตรูตามรายชื่อใน Inspector ของ Scene นี้
        foreach (EnemySpawnData enemyData in enemyList)
        {
            if (enemyData.enemyPrefab == null) continue;

            for (int i = 0; i < enemyData.count; i++)
            {
                if (emptyPositions.Count == 0) break;

                int randomIndex = Random.Range(0, emptyPositions.Count);
                Vector2 spawnPos = emptyPositions[randomIndex];

                Instantiate(enemyData.enemyPrefab, spawnPos, Quaternion.identity, transform);
                emptyPositions.RemoveAt(randomIndex);
            }
        }
    }
}