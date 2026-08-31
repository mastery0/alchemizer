using UnityEngine;

public class backGroundMusic : MonoBehaviour
{
    public AudioClip music;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(audioManager.playMusicCR(music));
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        audioManager.instance.stopMusic();
    }
}
