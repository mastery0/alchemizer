using UnityEngine;
using UnityEngine.Video;
public class introVideo : MonoBehaviour
{
    public VideoPlayer player;
    public GameObject mainMenu;
    public GameObject videoCanvas;
    private void Start()
    {
        player.loopPointReached += videoFinished;
        player.Play();
    }

    public void skip()
    {
        player.Stop();
        mainMenu.SetActive(true);
        videoCanvas.SetActive(false);
    }

    private void videoFinished(VideoPlayer player)
    {
        mainMenu.SetActive(true);
        videoCanvas.SetActive(false);
    }
}
