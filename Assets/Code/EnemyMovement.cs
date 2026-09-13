using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public LayerMask obstacleLayer; // เลือก Wall, SoftBlock, Bomb

    private Vector2 currentDir;
    private bool isMoving = false;

    private readonly Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Start()
    {
        ChooseRandomDirection();
    }

    void Update()
    {
        if (!isMoving)
        {
            if (CanMoveInDirection(currentDir))
            {
                Vector2 targetPos = (Vector2)transform.position + currentDir;
                StartCoroutine(MoveToTile(targetPos));
            }
            else
            {
                ChooseRandomDirection();
            }
        }
    }

    private IEnumerator MoveToTile(Vector2 target)
    {
        isMoving = true;

        while (Vector2.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        isMoving = false;

        // เมื่อถึงจุดตัดช่อง Grid มีโอกาส 30% ที่จะสุ่มเปลี่ยนทิศทางใหม่
        if (Random.value < 0.3f)
        {
            ChooseRandomDirection();
        }
    }

    private bool CanMoveInDirection(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.8f, obstacleLayer);
        return hit.collider == null;
    }

    private void ChooseRandomDirection()
    {
        List<Vector2> validDirs = new List<Vector2>();

        foreach (Vector2 dir in possibleDirections)
        {
            if (CanMoveInDirection(dir))
            {
                validDirs.Add(dir);
            }
        }

        if (validDirs.Count > 0)
        {
            currentDir = validDirs[Random.Range(0, validDirs.Count)];
        }
        else
        {
            currentDir = Vector2.zero;
        }
    }
}