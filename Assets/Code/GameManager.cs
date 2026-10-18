using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int maxLives = 3;
    public float stageTime = 200f; // เวลาถอยหลัง (วินาที)

    // ใช้ static เพื่อให้จำค่าข้ามการ LoadScene ได้
    public static int currentStage = 1;
    private static int currentLives = 3;
    private static int currentScore = 0;

    [Header("UI Elements (TextMeshPro)")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI stageClearText;
    public GameObject gameOverPanel;
    public GameObject stageClearPanel;

    private float timeRemaining;
    private bool isGameOver = false;

    private Vector2 playerStartPos;
    private GameObject playerObj;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // กำหนดเวลาถอยหลังประจำด่าน
        timeRemaining = stageTime;

        playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerStartPos = playerObj.transform.position;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (stageClearPanel != null) stageClearPanel.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // นับเวลาถอยหลัง
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateUI();
        }
        else
        {
            timeRemaining = 0;
            UpdateUI();
            PlayerDied(); // เวลาหมด = ผู้เล่นเสียชีวิต
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    public void PlayerDied()
    {
        if (isGameOver) return;

        currentLives--;
        UpdateUI();

        if (currentLives > 0)
        {
            StartCoroutine(RespawnPlayerRoutine());
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator RespawnPlayerRoutine()
    {
        // 1. รอให้แอนิเมชันตาย (Die) และเสียงเล่นจบก่อน (เช่น รอ 1.2 วินาที)
        yield return new WaitForSeconds(1.2f);

        // 2. หลังจากแอนิเมชันเล่นจบแล้วค่อยซ่อนตัวละคร
        if (playerObj != null)
        {
            playerObj.SetActive(false);
        }

        // 3. หน่วงต่ออีกนิดก่อนโหลดฉากใหม่
        yield return new WaitForSeconds(0.3f);

        // โหลด Scene ปัจจุบันขึ้นมาใหม่เพื่อรีเซ็ตบล็อก ศัตรู และตำแหน่งผู้เล่น
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ฟังก์ชันเรียกเคลียร์ด่าน
    public void CallStageClear()
    {
        if (isGameOver) return;
        StartCoroutine(StageClearRoutine());
    }

    // Coroutine ควบคุมการแสดงผล เสียง และหน่วงเวลา 2 รอบ
    private IEnumerator StageClearRoutine()
    {
        if (isGameOver) yield break;
        isGameOver = true;
        Time.timeScale = 0f; // หยุดเกม

        if (stageClearText != null)
        {
            stageClearText.text = "STAGE " + (currentStage + 1);
        }

        

        if (SoundManager.Instance != null)
        {
            // หยุดเพลงเดิม และเล่นเสียง Stage Clear รอบเดียวจบ
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.stageClearSFX);
        }

        // 🔄 รอบที่ 1: หน่วงเวลา 2 วินาที (ให้เสียง Stage Clear เล่น)
        yield return new WaitForSecondsRealtime(3f);

        // 🔄 รอบที่ 2: หน่วงเวลาต่ออีก 1 วินาทีก่อนโหลดด่านถัดไป
        SoundManager.Instance.PlaySFX(SoundManager.Instance.stageStartSFX);
        
        // เปิด Panel ทันที
        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(true);
        }
        Debug.Log("STAGE CLEAR! Next: Stage " + (currentStage + 1));
        yield return new WaitForSecondsRealtime(3f);

        // 1. โหลดด่านถัดไป
        LoadNextStage();

        // 2. สั่งให้ SoundManager เล่นเพลงหลักของด่านใหม่
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMainStageBGM();
        }
    }

    // รองรับสคริปต์อื่นที่เรียกใช้ฟังก์ชันนี้ตรงๆ
    public void StageClear()
    {
        CallStageClear();
    }

    public void LoadNextStage()
    {
        // 1. คืนค่า Time.timeScale ให้เกมเดินต่อปกติ
        Time.timeScale = 1f;

        // 2. เพิ่มค่าด่านปัจจุบันขึ้น 1
        currentStage++;

        // 3. บันทึกด่านสูงสุดที่ปลดล็อกลง SaveManager
        SaveManager.UnlockStage(currentStage);

        // 4. โหลด Scene ถัดไป (เช่น Stage2, Stage3 ตามหมายเลข)
        string nextSceneName = "Stage" + currentStage;

        // เช็คว่ามี Scene นี้อยู่จริงไหมก่อนโหลด
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("ไม่พบ Scene ชื่อ " + nextSceneName + " อาจจะเป็นด่านสุดท้ายแล้ว!");
            SceneManager.LoadScene("MainMenu"); // ถ้าหมดด่านแล้วให้กลับหน้าเมนูหลัก
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // หยุดเกม

        // สั่งบันทึกคะแนนสูงสุดทันทีที่ Game Over
        SaveManager.SaveHighScore(currentScore);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Debug.Log("GAME OVER - Final Score saved: " + currentScore);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.gameOverSFX);
        }
    }

    public void RestartGame()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopAllAudio();
        }
        Time.timeScale = 1f;

        currentLives = maxLives;
        currentScore = 0;
        currentStage = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMainStageBGM();
        }
    }

    private void UpdateUI()
    {
        if (timeText != null) timeText.text = "TIME " + Mathf.CeilToInt(timeRemaining).ToString("D3");
        if (livesText != null) livesText.text = "LEFT " + currentLives;
        if (scoreText != null) scoreText.text = currentScore.ToString("D6");
    }
}