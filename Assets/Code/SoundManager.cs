using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip titleScreenBGM; // 01. Title Screen
    public AudioClip stageBGM;       // 03. Main BGM
    public AudioClip powerUpBGM;     // 04. Power-Up Get
    public AudioClip endingBGM;      // 08. Ending[cite: 2]

    [Header("SFX Clips")]
    public AudioClip stageStartSFX;    // 02. Stage Start
    public AudioClip powerUpSFX;       // powerup.wav
    public AudioClip stageClearSFX;    // 05. Stage Clear
    public AudioClip gameOverSFX;      // 10. Game Over[cite: 1]
    public AudioClip bombLaySFX;       // bomb_lay
    public AudioClip bombExplosionSFX;// bomb_explosion
    public AudioClip playerDeathSFX;  // bomberman_killed

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
        // 1. เล่นเสียง SFX สั้นๆ ทุกครั้งที่เก็บไอเทม
        PlaySFX(powerUpSFX);

        // 2. เปลี่ยนเพลง BGM เป็น Power-Up BGM แค่ "ครั้งแรก" ของด่านนั้นๆ
        if (!hasCollectedPowerUpInThisStage && powerUpBGM != null)
        {
            hasCollectedPowerUpInThisStage = true; // ล็อกไว้ว่าเก็บไปแล้ว จะไม่เปลี่ยนเพลงซ้ำในด่านนี้
            PlayMusic(powerUpBGM);
        }
    }

    // 🎯 เพิ่มฟังก์ชันสำหรับเล่นเสียงเดิน (แยกตามแนวตั้ง/แนวนอน)
    public void PlayFootstep(bool isVertical)
    {
        AudioClip clipToPlay = isVertical ? stepVerticalSFX : stepHorizontalSFX;
        if (clipToPlay != null)
        {
            // ใช้ PlayOneShot เพื่อให้เสียงเดินแทรกกับเสียงอื่นๆ ได้โดยไม่สะดุด
            sfxSource.PlayOneShot(clipToPlay);
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

    // 🎯 สั่งหยุดเฉพาะเพลงที่กำหนด (ถ้าเพลงนั้นกำลังเล่นอยู่ถึงจะหยุด)
    public void StopSpecificMusic(AudioClip clip)
    {
        if (musicSource != null && musicSource.clip == clip && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}