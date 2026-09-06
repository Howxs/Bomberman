using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemType
    {
        FireUp,
        BombUp,
        SpeedUp,
        RemoteControl,
        BombPass,
        WallPass
    }

    [Header("Item Configuration")]
    public ItemType type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void ApplyEffect(GameObject player)
    {
        BombSpawner spawner = player.GetComponent<BombSpawner>();
        BombermanController controller = player.GetComponent<BombermanController>();

        switch (type)
        {
            case ItemType.FireUp:
                if (spawner != null) spawner.explosionRadius++;
                break;

            case ItemType.BombUp:
                if (spawner != null) spawner.maxBombs++;
                break;

            case ItemType.SpeedUp:
                if (controller != null) controller.moveSpeed += 0.5f;
                break;

            case ItemType.RemoteControl:
                if (spawner != null) spawner.hasRemoteControl = true;
                break;

            case ItemType.BombPass:
                if (spawner != null) spawner.canPassBombs = true;
                break;

            case ItemType.WallPass:
                if (controller != null) spawner.canPassWalls = true;
                break;
        }
    }
}