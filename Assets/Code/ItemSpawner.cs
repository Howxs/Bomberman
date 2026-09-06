using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    [Header("Exit Door Settings")]
    public GameObject exitDoorPrefab;

    private List<Vector2> softBlockPositions = new List<Vector2>();
    private bool exitSpawned = false;

    private void Awake()
    {
        Instance = this;
    }

    // ฟังก์ชันลงทะเบียนตำแหน่ง Soft Block ทั้งหมด
    public void RegisterSoftBlock(Vector2 position)
    {
        softBlockPositions.Add(position);
    }

    // เรียกสุ่มสปอว์นประตู 1 จุด หลังจากสปอว์น Soft Block ทั้งหมดเสร็จ
    public void SpawnExitDoor()
    {
        if (exitSpawned || softBlockPositions.Count == 0) return;

        int randomIndex = Random.Range(0, softBlockPositions.Count);
        Vector2 exitPosition = softBlockPositions[randomIndex];

        Instantiate(exitDoorPrefab, exitPosition, Quaternion.identity);
        exitSpawned = true;
    }
}