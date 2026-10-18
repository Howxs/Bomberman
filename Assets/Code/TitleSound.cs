using UnityEngine;

public class TitleSound : MonoBehaviour
{
    // 🎯 Singleton Instance สำหรับให้สคริปต์อื่น (เช่น MenuManager) เรียกใช้งาน
    public static TitleSound Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource sfxSource; // เพิ่ม AudioSource สำหรับเล่นเสียง SFX แยกต่างหาก (หรือใช้ร่วมกันก็ได้)

    [Header("Music Clips")]
    public AudioClip titleScreenBGM; // เพลงประกอบหน้า Title Screen

    [Header("SFX Clips")]
    public AudioClip stageStartSFX;  // 🎯 เสียงเริ่มเกม (Stage Start)

    private void Awake()
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

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        PlayTitleBGM();
    }

    // ฟังก์ชันเริ่มเล่นเพลง Title BGM
    public void PlayTitleBGM()
    {
        if (titleScreenBGM != null && audioSource != null)
        {
            audioSource.clip = titleScreenBGM;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // ฟังก์ชันสั่งหยุดเพลง Title BGM
    public void StopTitleBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // 🎯 ฟังก์ชันสำหรับเล่นเสียง Stage Start SFX
    public void PlayStageStartSFX()
    {
        if (stageStartSFX != null)
        {
            // ถ้ามี sfxSource แยก ให้ใช้ sfxSource.PlayOneShot ถ้าไม่มีให้ใช้ audioSource แทน
            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(stageStartSFX);
            }
            else if (audioSource != null)
            {
                audioSource.PlayOneShot(stageStartSFX);
            }
        }
    }
}