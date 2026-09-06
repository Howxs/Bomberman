using UnityEngine;
using System.Collections;

public class BombSpawner : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public int maxBombs = 1;
    public KeyCode dropKey = KeyCode.Space;
    public KeyCode detonateKey = KeyCode.E;

    [Header("Power-Up Stats")]
    public int explosionRadius = 1;
    public bool hasRemoteControl = false;
    public bool canPassBombs = false;

    [Header("Layers")]
    public LayerMask bombLayer;

    private int currentBombCount = 0;

    private void Update()
    {
        if (Input.GetKeyDown(dropKey) && currentBombCount < maxBombs)
        {
            DropBomb();
        }

        // กดปุ่ม E เพื่อจุดชนวนระเบิดแบบ Remote Control
        if (hasRemoteControl && Input.GetKeyDown(detonateKey))
        {
            DetonateAllBombs();
        }

        
    }



    private void DropBomb()
    {
        Vector3 footPosition = transform.position + new Vector3(0, -0.4f, 0);
        Vector2 spawnPos = new Vector2(Mathf.Round(footPosition.x), Mathf.Round(footPosition.y));

        Collider2D hit = Physics2D.OverlapBox(spawnPos, new Vector2(0.5f, 0.5f), 0f);
        if (hit != null && hit.GetComponent<BombLogic>() != null)
        {
            return;
        }

        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        // ส่งค่าการอัปเกรดไปยังลูกระเบิด
        BombLogic bombScript = bomb.GetComponent<BombLogic>();
        if (bombScript != null)
        {
            bombScript.explosionRadius = this.explosionRadius;
            bombScript.isRemote = this.hasRemoteControl;
        }

        currentBombCount++;
        StartCoroutine(TrackBombDestruction(bomb));
    }

    private void DetonateAllBombs()
    {
        BombLogic[] activeBombs = FindObjectsByType<BombLogic>(FindObjectsSortMode.None);
        foreach (BombLogic bomb in activeBombs)
        {
            bomb.Explode();
        }
    }

    private IEnumerator TrackBombDestruction(GameObject bomb)
    {
        while (bomb != null)
        {
            yield return null;
        }
        currentBombCount--;
    }

    // เรียกฟังก์ชันนี้จาก ItemPickUp เมื่อเก็บไอเทม Bomb Pass
    public void EnableBombPass()
    {
        canPassBombs = true;

        // ดึงค่า Layer Index จาก LayerMask แบบปลอดภัย
        int playerLayer = gameObject.layer;
        int bombLayerIndex = GetLayerFromMask(bombLayer);

        if (bombLayerIndex >= 0 && bombLayerIndex <= 31)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, bombLayerIndex, true);
        }
    }

    private int GetLayerFromMask(LayerMask mask)
    {
        int layer = 0;
        int layerValue = mask.value;
        if (layerValue <= 0) return -1;

        while (layerValue > 1)
        {
            layerValue >>= 1;
            layer += 1;
        }
        return layer;
    }




}