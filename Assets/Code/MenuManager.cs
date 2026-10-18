using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // เรียกใช้ TextMeshPro
using System.Collections; // อย่าลืม using System.Collections สำหรับ Coroutine

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;    // แผงเมนูหลัก (ปุ่ม Start, Quit)
    public GameObject optionsPanel; // แผงตั้งค่า (ถ้าไม่มี ให้ปล่อยว่างไว้)
    public GameObject stage1Panel;  // แผงสำหรับแสดงผลเมื่อเริ่มด่าน 1

    [Header("UI Text Display")]
    public TMP_Text topScoreText;   // Drag & Drop ข้อความแสดง TOP Score มาใส่ตรงนี้

    [Header("Scene Settings")]
    public string defaultGameScene = "Stage1"; // ชื่อ Scene เกมหลัก

    private void Start()
    {
        // เมื่อเริ่มหน้าเมนู ให้ซ่อน stage1Panel ไว้ก่อน (ถ้ามี)
        if (stage1Panel != null)
        {
            stage1Panel.SetActive(false);
        }

        // เมื่อเริ่มหน้าเมนู ให้ดึงคะแนนสูงสุดจาก SaveManager มาแสดงผล
        UpdateTopScoreUI();
    }

    public void UpdateTopScoreUI()
    {
        if (topScoreText != null)
        {
            int topScore = SaveManager.GetHighScore();
            // แสดงผลตัวเลขคะแนน (เช่น 00 หรือ 000000)
            topScoreText.text = topScore.ToString("D2");
        }
    }

    // ฟังก์ชันกดเริ่มเกม (เรียกผ่าน Coroutine เพื่อให้เล่นเสียงและหน่วงเวลาก่อนโหลดฉาก)
    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    // Coroutine สำหรับเล่นเสียง stageStartSFX หน่วงเวลา แล้วค่อยเปลี่ยนฉาก
    private IEnumerator StartGameRoutine()
    {
        if (TitleSound.Instance != null)
        {
            TitleSound.Instance.StopTitleBGM();
        }

        // 1. รีเซ็ตค่าด่านกลับเป็น 1 เมื่อเริ่มเกมใหม่
        GameManager.currentStage = 1;

        // 2. สั่งเปิด stage1Panel ทันทีที่กดเริ่มเกม (ถ้ามีการตั้งค่าไว้)
        if (stage1Panel != null)
        {
            stage1Panel.SetActive(true);
            TitleSound.Instance.PlayStageStartSFX();
        }

        // 3. สั่งเล่นเสียง Start Game (stageStartSFX) และหยุดเพลงเมนูถ้ามี
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.stageStartSFX);
        }

        // 4. หน่วงเวลารอให้เสียงเล่น (ปรับเวลาได้ตามความยาวของเสียง เช่น 2.5 วินาที)
        yield return new WaitForSecondsRealtime(3f);

        // 5. โหลด Scene เกมหลัก
        if (!string.IsNullOrEmpty(defaultGameScene))
        {
            SceneManager.LoadScene(defaultGameScene);
        }
        else
        {
            Debug.LogError("กรุณาระบุชื่อ Scene เกมหลักในช่อง Default Game Scene ที่ Inspector!");
        }
    }

    // 🎯 ฟังก์ชันกด Continue (เล่นต่อจากด่านสูงสุดที่ปลดล็อกไว้)
    public void ContinueGame()
    {
        StartCoroutine(ContinueGameRoutine());
    }

    // Coroutine สำหรับปุ่ม Continue (ถ้าอยากให้เล่นเสียงด้วยเช่นกัน)
    private IEnumerator ContinueGameRoutine()
    {
        // 1. ดึงข้อมูลด่านสูงสุดที่เคยปลดล็อกไว้จาก SaveManager
        int unlockedStage = SaveManager.GetUnlockedStage();

        // 2. ส่งค่าด่านไปให้ GameManager
        GameManager.currentStage = unlockedStage;

        // 3. เล่นเสียง Start
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.stageStartSFX);
        }

        // 4. หน่วงเวลารอเสียงเล่น
        yield return new WaitForSecondsRealtime(2.5f);

        // 5. โหลดฉากเกมตามด่านที่ปลดล็อก
        string stageSceneName = "Stage" + unlockedStage;
        SceneManager.LoadScene(stageSceneName);
    }

    // เปิดหน้าจอตั้งค่า
    public void OpenOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    // กลับมาหน้าเมนูหลัก
    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    // ออกจากเกม
    public void QuitGame()
    {
        Debug.Log("Quit Game Executed.");
        Application.Quit();
    }
}