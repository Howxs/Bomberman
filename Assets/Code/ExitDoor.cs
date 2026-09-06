using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // เช็กว่าตัวละครเดินเข้ามาชนประตูหรือไม่
        if (other.CompareTag("Player"))
        {
            // TODO: สามารถเพิ่มเงื่อนไขเช็กว่ากำจัดศัตรูหมดหรือยังได้ใน Week 5/6
            Debug.Log("Stage Clear! เดินเข้าประตู Exit เรียบร้อย");
        }
    }
}