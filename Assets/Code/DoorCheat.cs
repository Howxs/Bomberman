using UnityEngine;

public class DoorCheat : MonoBehaviour
{
    [Header("Door Reference")]
    // ลาก GameObject ของประตู หรือสคริปต์เปิดประตูของคุณมาใส่ตรงนี้
    public GameObject doorObject;

    // หรือถ้าใช้สคริปต์เปิดประตูเฉพาะ สามารถเรียกใช้ฟังก์ชันเปิดประตูตรงๆ ได้ เช่น:
    // public DoorController doorController;

    void Update()
    {
        // เช็คว่ากดปุ่ม G บนคีย์บอร์ดหรือไม่
        if (Input.GetKeyDown(KeyCode.G))
        {
            ForceOpenDoor();
        }
    }

    void ForceOpenDoor()
    {
        Debug.Log("Cheat Activated: Force Opening Door!");

        // วิธีที่ 1: ถ้าประตูเป็น GameObject ธรรมดา ให้สั่งปิดการแสดงผล (Disable) หรือเปิดอนิเมชัน
        if (doorObject != null)
        {
            doorObject.SetActive(false); // ซ่อน/เปิดทางเดินประตูทันที
        }

        // วิธีที่ 2: ถ้าคุณมีฟังก์ชันเปิดประตูในสคริปต์ของประตูอยู่แล้ว สามารถเรียกใช้แบบนี้ได้ครับ:
        /*
        if (doorController != null)
        {
            doorController.OpenDoor();
        }
        */
    }
}