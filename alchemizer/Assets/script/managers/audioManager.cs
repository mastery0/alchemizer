using UnityEngine;
using UnityEngine.Audio;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;
    [Header("Audio Sources")]

    public AudioMixer audioMixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void playMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
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
        float vol;
        audioMixer.GetFloat("MasterVolume", out vol);
        Debug.Log(vol);
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
