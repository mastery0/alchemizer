using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
[DefaultExecutionOrder(-5)]
public class audioManager : MonoBehaviour
{
    public static audioManager instance;
    [Header("Audio Sources")]

    public AudioMixer audioMixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public static bool hasAwoken=false;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        hasAwoken = true;
    }
    public static IEnumerator playMusicCR(AudioClip clip)
    {
        yield return new WaitUntil(()=>hasAwoken == true);
        instance.musicSource.clip = clip;
        instance.musicSource.loop = true;
        instance.musicSource.time = 0;
        instance.musicSource.Play();
    }
    public void stopMusic()
    {
        musicSource.Stop();
    }
    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void setMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume",Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f))*20);
    }

    public void setMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f))*20);
    }

    public void setSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f))*20);
    }
    
}
