using UnityEngine;

public class parallaxBuonds : MonoBehaviour
{
    public GameObject backGround;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        backGround.SetActive(true);
    }
  /*  private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        backGround.SetActive(true);
    }*/
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        backGround.SetActive(false);
    }
}
