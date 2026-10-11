using UnityEngine;
using System.Collections;

public class BombSpawner : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public int maxBombs = 1;
    public KeyCode dropKey = KeyCode.Space;
    public KeyCode detonateKey = KeyCode.E;

    [Header("Spawn Reference")]
    public Transform bombSpawnPoint; // ลาก Empty Object ที่อยู่ตรงเท้ามาใส่ตรงนี้

    [Header("Power-Up Stats")]
    public int explosionRadius = 1;
    public bool hasRemoteControl = false;
    public bool canPassBombs = false;

    [Header("Layers")]
    public LayerMask bombLayer;

    private int currentBombCount = 0;
    private Collider2D playerCollider;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();

        // ถ้าไม่ได้ลาก Bomb Spawn Point มา ให้ใช้ตำแหน่งตัวละครแทน
        if (bombSpawnPoint == null)
        {
            bombSpawnPoint = transform;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(dropKey) && currentBombCount < maxBombs)
        {
            DropBomb();
            if (SoundManager.Instance != null && SoundManager.Instance.bombLaySFX != null)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.bombLaySFX);
            }
        }

        // กดปุ่ม E เพื่อจุดชนวนระเบิดแบบ Remote Control
        if (hasRemoteControl && Input.GetKeyDown(detonateKey))
        {
            DetonateAllBombs();
        }
    }

    private void DropBomb()
    {
        // 1. อิงตำแหน่งจาก Empty Object ที่เท้า แล้วปัดเศษลง Grid ของฉาก
        Vector2 spawnPos = new Vector2(
            Mathf.Round(bombSpawnPoint.position.x),
            Mathf.Round(bombSpawnPoint.position.y)
        );

        // 2. เช็กว่าช่องนั้นมีระเบิดวางอยู่แล้วหรือยัง
        Collider2D hit = Physics2D.OverlapBox(spawnPos, new Vector2(0.5f, 0.5f), 0f);
        if (hit != null && hit.GetComponent<BombLogic>() != null)
        {
            return;
        }

        // 3. สร้างระเบิด
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        // ส่งค่าการอัปเกรดไปยังลูกระเบิด
        BombLogic bombScript = bomb.GetComponent<BombLogic>();
        if (bombScript != null)
        {
            bombScript.explosionRadius = this.explosionRadius;
            bombScript.isRemote = this.hasRemoteControl;
        }

        // 4. ดึง Collider ของระเบิดชิ้นที่เพิ่งวาง เพื่อให้ตัวละครเดินทะลุได้ชั่วคราว
        Collider2D bombCollider = bomb.GetComponent<Collider2D>();
        if (playerCollider != null && bombCollider != null)
        {
            StartCoroutine(TemporaryPassBomb(playerCollider, bombCollider));
        }

        currentBombCount++;
        StartCoroutine(TrackBombDestruction(bomb));
    }

    // โครูทีนจัดการให้ผู้เล่นเดินทะลุระเบิดที่เพิ่งวางเป็นเวลา 1 วินาที
    private IEnumerator TemporaryPassBomb(Collider2D playerCol, Collider2D bombCol)
    {
        Physics2D.IgnoreCollision(playerCol, bombCol, true);

        // รอจนกว่าตัวละครจะเดินพ้นตัวระเบิดออกไป (หรือครบ 1 วินาที)
        yield return new WaitForSeconds(1.0f);

        // คืนค่าการชนกลับมาเหมือนเดิม (ถ้าตัวละครยังทับอยู่ Unity จะดันออกให้อัตโนมัติเมื่อเดินออก)
        if (bombCol != null && playerCol != null)
        {
            Physics2D.IgnoreCollision(playerCol, bombCol, false);
        }
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