using UnityEngine;

public class fallDeath : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) player.instance.die();
        else Destroy(collision.gameObject);
    }
}
