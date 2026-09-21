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
        if (playerObj != null)
        {
            playerObj.SetActive(false); // ซ่อนตัวละครชั่วคราวขณะแสดงผลการตาย
        }

        yield return new WaitForSeconds(1.5f); // รอ 1.5 วินาที

        // โหลด Scene ปัจจุบันขึ้นมาใหม่ทั้งหมดเพื่อรีเซ็ตบล็อกและศัตรู
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StageClear()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f; // หยุดเกม

        if (stageClearText != null)
        {
            stageClearText.text = "STAGE " + (currentStage + 1);
        }

        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(true);
        }
        Debug.Log("STAGE CLEAR! Next: Stage " + (currentStage + 1));
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // หยุดเกม

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Debug.Log("GAME OVER");
    }

    public void RestartGame()
    {
        // 1. คืนค่าเวลาให้เกมกลับมารันตามปกติ
        Time.timeScale = 1f;

        // 2. รีเซ็ตค่า static เมื่อเริ่มเล่นใหม่
        currentLives = maxLives;
        currentScore = 0;
        currentStage = 1;

        // 3. โหลด Scene ใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI()
    {
        if (timeText != null) timeText.text = "TIME " + Mathf.CeilToInt(timeRemaining).ToString("D3");
        if (livesText != null) livesText.text = "LEFT " + currentLives;
        if (scoreText != null) scoreText.text = "SCORE " + currentScore.ToString("D6");
    }
}