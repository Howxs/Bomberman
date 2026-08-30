using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // ลาก Player มาใส่
    public float smoothSpeed = 5f;

    [Header("Camera Clamp Bounds")]
    public float minX = -7.5f; // ค่าเดิมตามที่เคยตั้งไว้
    public float maxX = 7.5f;

    // ----------------------------------------------------
    // ปรับ minY ลดลงอีก 1 (เช่น ถ้าของเดิมเป็น -1 ให้เปลี่ยนเป็น -2 
    // หรือถ้าเดิมเป็น -0.5 ให้เปลี่ยนเป็น -1.5)
    // ----------------------------------------------------
    public float minY = -2f; // ขยับขอบล่างสุดลงมาตามสนาม
    public float maxY = 0f;  // ขอบบนสุด (รวมแถบ HUD แล้ว)

    void LateUpdate()
    {
        if (target == null) return;

        // คำนวณตำแหน่งที่กล้องต้องการไป
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // จำกัดขอบเขตไม่ให้กล้องเห็นนอกขอบสนาม
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, transform.position.z), smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}