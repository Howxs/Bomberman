using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // ตัวแปรสำหรับเก็บค่าคีย์ (Key) ในการบันทึกข้อมูลลงในระบบ
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string UNLOCKED_STAGE_KEY = "UnlockedStage";

    // บันทึกคะแนนสูงสุด (ถ้าคะแนนใหม่มากกว่าเดิม ถึงจะบันทึกทับ)
    public static void SaveHighScore(int score)
    {
        int currentHighScore = GetHighScore();
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
            Debug.Log("Saved New High Score: " + score);
        }
    }

    // โหลดคะแนนสูงสุด
    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0); // ถ้ายังไม่เคยเซฟ จะคืนค่าเป็น 0
    }

    // บันทึกด่านสูงสุดที่ปลดล็อกแล้ว
    public static void SaveUnlockedStage(int stageLevel)
    {
        int currentUnlocked = GetUnlockedStage();
        if (stageLevel > currentUnlocked)
        {
            PlayerPrefs.SetInt(UNLOCKED_STAGE_KEY, stageLevel);
            PlayerPrefs.Save();
            Debug.Log("Saved Unlocked Stage: " + stageLevel);
        }
    }

    // โหลดด่านสูงสุดที่ปลดล็อก
    public static int GetUnlockedStage()
    {
        return PlayerPrefs.GetInt(UNLOCKED_STAGE_KEY, 1); // ค่าเริ่มต้นคือด่านที่ 1
    }

    // ฟังก์ชันสำหรับล้างข้อมูลทั้งหมด (ใช้เวลาต้องการรีเซ็ตเกมทดสอบ)
    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All Save Data Reset.");
    }

    // เพิ่มฟังก์ชันนี้ไว้ใน SaveManager.cs
    public static void UnlockStage(int stageNum)
    {
        // ดึงค่าด่านสูงสุดที่เคยปลดล็อกไว้เดิม (ถ้ายังไม่มีให้เริ่มที่ 1)
        int unlockedStage = PlayerPrefs.GetInt("UnlockedStage", 1);

        // ถ้าด่านที่กำลังจะปลดล็อก มากกว่าด่านสูงสุดเดิม ให้บันทึกทับใหม่
        if (stageNum > unlockedStage)
        {
            PlayerPrefs.SetInt("UnlockedStage", stageNum);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Stage: " + stageNum);
        }
    }
}