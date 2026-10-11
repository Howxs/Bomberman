using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip titleScreenBGM; // 01. Title Screen[cite: 7]
    public AudioClip stageBGM;       // 03. Main BGM[cite: 5]
    public AudioClip powerUpBGM;     // 04. Power-Up Get[cite: 4]
    public AudioClip endingBGM;      // 08. Ending[cite: 2]

    [Header("SFX Clips")]
    public AudioClip stageStartSFX;    // 02. Stage Start[cite: 6]
    public AudioClip powerUpSFX;       // powerup.wav[cite: 11]
    public AudioClip stageClearSFX;    // 05. Stage Clear[cite: 3]
    public AudioClip gameOverSFX;      // 10. Game Over[cite: 1]
    public AudioClip bombLaySFX;       // bomb_lay[cite: 9]
    public AudioClip bombExplosionSFX;// bomb_explosion[cite: 8]
    public AudioClip playerDeathSFX;  // bomberman_killed[cite: 10]

    [Header("Footstep SFX")]
    public AudioClip stepVerticalSFX;   // เสียงเดินแนวตั้ง
    public AudioClip stepHorizontalSFX; // เสียงเดินแนวนอน

    // ตัวแปรเช็กว่าในด่านนี้เก็บไอเทมไปแล้วหรือยัง
    private bool hasCollectedPowerUpInThisStage = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMainStageBGM();
    }

    // เริ่มเล่นเพลง Main BGM และรีเซ็ตสถานะของด่านใหม่
    public void PlayMainStageBGM()
    {
        hasCollectedPowerUpInThisStage = false; // รีเซ็ตให้สามารถเปลี่ยนเพลงเมื่อเก็บไอเทมชิ้นแรกของด่านได้
        PlayMusic(stageBGM);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // ฟังก์ชันเรียกเมื่อเก็บไอเทม
    public void OnCollectPowerUp()
    {
        // 1. เล่นเสียง SFX สั้นๆ ทุกครั้งที่เก็บไอเทม[cite: 11]
        PlaySFX(powerUpSFX);

        // 2. เปลี่ยนเพลง BGM เป็น Power-Up BGM แค่ "ครั้งแรก" ของด่านนั้นๆ[cite: 4]
        if (!hasCollectedPowerUpInThisStage && powerUpBGM != null)
        {
            hasCollectedPowerUpInThisStage = true; // ล็อกไว้ว่าเก็บไปแล้ว จะไม่เปลี่ยนเพลงซ้ำในด่านนี้
            PlayMusic(powerUpBGM);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    // สั่งหยุดเสียงทั้งหมด (ทั้ง BGM และ SFX) ทันที
    public void StopAllAudio()
    {
        if (musicSource != null) musicSource.Stop();
        if (sfxSource != null) sfxSource.Stop();
    }
}