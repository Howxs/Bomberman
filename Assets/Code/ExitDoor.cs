using UnityEngine;
using System.Collections;

public class ExitDoor : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite closedSprite;                 // ภาพตอนประตูยังปิดอยู่
    public Sprite[] openAnimationSprites;      // ภาพอนิเมชันตอนประตูเปิดแล้ว (สลับวนลูปไปเรื่อยๆ)
    public float animationSpeed = 0.2f;         // ความเร็วในการเปลี่ยนภาพ

    [Header("Penalty Settings")]
    [SerializeField] private GameObject enemyPrefab; // Prefab ศัตรูที่จะเสกเมื่อประตูโดนไฟ
    [SerializeField] private int enemiesToSpawn = 1;   // จำนวนศัตรูที่จะเสก
    [SerializeField] private float spawnDelay = 1.0f;  // หน่วงเวลา 1 วินาทีก่อนศัตรูเกิด

    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;
    private bool isOpen = false;
    private bool hasTriggeredPenalty = false;

    private int currentAnimationFrame = 0;
    private float animationTimer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();

        // เริ่มต้น: ปิดประตูไว้ก่อน
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = false; // ยังเดินเข้าไม่ได้ตอนยังไม่เปิด
        }
    }

    void Update()
    {
        // 🎯 [ระบบโกง] กดปุ่ม G เพื่อบังคับเปิดประตูทันทีสำหรับเทสระบบ
        if (Input.GetKeyDown(KeyCode.G) && !isOpen)
        {
            Debug.Log("Cheat Activated: Force Opening Exit Door!");
            OpenDoor();
        }

        // เช็คว่าศัตรูหมดฉากหรือยังทุกๆ เฟรม (ถ้ายังไม่เปิด)
        if (!isOpen)
        {
            int totalEnemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None).Length +
                               FindObjectsByType<OvapeMovement>(FindObjectsSortMode.None).Length +
                               FindObjectsByType<DahlMovement>(FindObjectsSortMode.None).Length;

            if (totalEnemies == 0)
            {
                OpenDoor();
            }
        }
        else
        {
            // ถ้าประตูเปิดแล้ว ให้เล่นอนิเมชันสลับภาพวนลูปไปเรื่อยๆ
            PlayOpenAnimation();
        }
    }

    public void OpenDoor()
    {
        isOpen = true;

        if (doorCollider != null)
        {
            doorCollider.enabled = true; // เปิดให้ผู้เล่นเดินชน/เข้าประตูได้
        }

        Debug.Log("ประตูเปิดออกแล้ว! สามารถเดินเข้าเพื่อเคลียร์ด่านได้");
    }

    private void PlayOpenAnimation()
    {
        if (spriteRenderer == null || openAnimationSprites == null || openAnimationSprites.Length == 0) return;

        animationTimer += Time.deltaTime;
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;
            currentAnimationFrame = (currentAnimationFrame + 1) % openAnimationSprites.Length;
            spriteRenderer.sprite = openAnimationSprites[currentAnimationFrame];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. ถ้าโดนไฟระเบิด
        if (other.CompareTag("Explosion") && !hasTriggeredPenalty)
        {
            hasTriggeredPenalty = true;
            TriggerExplosionPenalty();
            return;
        }

        // 2. ผู้เล่นเดินเข้าประตู (ต้องเปิดแล้วเท่านั้นถึงจะผ่านด่านได้)
        if (other.CompareTag("Player") && isOpen)
        {
            // 🎯 สั่งจัดการเสียงเมื่อผ่านด่าน
            if (SoundManager.Instance != null)
            {
                // หยุดเพลงประกอบฉากทั้งหมดทันที
                SoundManager.Instance.StopMusic();
                // เล่นเสียง Stage Clear เพียงเสียงเดียว
                SoundManager.Instance.PlaySFX(SoundManager.Instance.stageClearSFX);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StageClear();
            }
            Debug.Log("Stage Clear! Player entered the exit door.");
        }
    }

    private void TriggerExplosionPenalty()
    {
        Debug.Log("Exit Door was hit by explosion! Starting spawn penalty delay...");
        StartCoroutine(SpawnPenaltyRoutine());
    }

    private IEnumerator SpawnPenaltyRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (enemyPrefab != null)
        {
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            }
            Debug.Log("Penalty enemies spawned successfully after delay!");
        }
    }
}