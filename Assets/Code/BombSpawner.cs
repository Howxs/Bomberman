using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public int maxBombs = 1;
    public KeyCode dropKey = KeyCode.Space;

    [Header("Power-Up Stats")]
    public int explosionRadius = 1;
    public bool hasRemoteControl = false;
    public bool canPassBombs = false;
    public bool canPassWalls = false;

    private int currentBombCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(dropKey) && currentBombCount < maxBombs)
        {
            DropBomb();
        }

        // กดปุ่ม E เพื่อสั่งระเบิดแบบ Remote Control
        if (hasRemoteControl && Input.GetKeyDown(KeyCode.E))
        {
            DetonateAllBombs();
        }
    }

    void DropBomb()
    {
        Vector3 footPosition = transform.position + new Vector3(0, -0.4f, 0);

        Vector2 spawnPos = new Vector2(
            Mathf.Round(footPosition.x),
            Mathf.Round(footPosition.y)
        );

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

    private System.Collections.IEnumerator TrackBombDestruction(GameObject bomb)
    {
        while (bomb != null)
        {
            yield return null;
        }
        currentBombCount--;
    }
}