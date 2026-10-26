using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingManager : MonoBehaviour
{
    [Header("UI Panels & Displays")]
    public GameObject endingPanel;      // แผงหน้าจอฉากจบ
    public TMP_Text finalScoreText;     // ข้อความแสดงคะแนนรวมตอนจบ
    public TMP_Text endingMessageText;  // ข้อความแสดงความยินดี / เนื้อเรื่องตอนจบ

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // ชื่อ Scene หน้าเมนูหลัก

    [Header("Audio Settings")]
    public AudioClip endingBGM;         // เพลงประกอบฉากจบ

    void Start()
    {
        // เริ่มต้นเรียกฟังก์ชันแสดงฉากจบ
        TriggerEnding();
    }

    public void TriggerEnding()
    {
        // 1. เปิด Panel ฉากจบ
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
        }

        // 2. ดึงคะแนนสูงสุดหรือคะแนนสุดท้ายมาแสดงผล
        if (finalScoreText != null)
        {
            int finalScore = SaveManager.GetHighScore();
            finalScoreText.text = "FINAL SCORE : " + finalScore.ToString("D6");
        }

        // 3. จัดการเพลงฉากจบ
        if (SoundManager.Instance != null && endingBGM != null)
        {
            SoundManager.Instance.StopMusic();
            // หากใน SoundManager มีฟังก์ชันเล่นเพลงเฉพาะ สามารถปรับเปลี่ยนตรงนี้ได้ครับ
            SoundManager.Instance.PlaySFX(endingBGM);
        }

        // ปลดล็อกเวลาให้ปกติ (เผื่อมีการหยุดเกมไว้)
        Time.timeScale = 1f;
    }

    // ฟังก์ชันผูกกับปุ่มกดเพื่อกลับสู่หน้าเมนูหลัก
    public void ReturnToMainMenu()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopAllAudio();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}