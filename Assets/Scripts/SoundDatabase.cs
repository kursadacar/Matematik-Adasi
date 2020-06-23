using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SoundDatabase : Singleton<SoundDatabase>
{
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioSource musicSource;

    public AudioClip mainMenuMusic;
    public List<AudioClip> backgroundMusic = new List<AudioClip>();
    public List<AudioClip> footsteps = new List<AudioClip>();
    public AudioClip fishSplashSound;

    public AudioClip buttonSound;
    public AudioClip winSound;
    public AudioClip notificationSound;

    private void Start()
    {
        musicSource.PlayOneShot(mainMenuMusic);
    }
    private void Update()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.PlayOneShot(backgroundMusic[Random.Range(0, backgroundMusic.Count)]);
        }
    }

    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void PlayAudio(AudioClip clip)
    {
        Instance.audioSrc.PlayOneShot(clip);
    }

    public static void Play(AudioClip clip)
    {
        Instance.PlayAudio(clip);
    }

    public static void PlayNotificationSound()
    {
        Play(Instance.notificationSound);
    }

    public static void PlayWinSound()
    {
        Play(Instance.winSound);
    }

    public static void PlayButtonSound()
    {
        Play(Instance.buttonSound);
    }

    public static AudioClip GetRandomFootstep()
    {
        return Instance.footsteps[Random.Range(0, Instance.footsteps.Count)];
    }
}
