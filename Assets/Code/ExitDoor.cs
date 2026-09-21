using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
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
}