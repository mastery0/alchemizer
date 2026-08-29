using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float musicVolume=1f;

    [Range(0f,1f)]
    public float sfxVolume=1f;
}
