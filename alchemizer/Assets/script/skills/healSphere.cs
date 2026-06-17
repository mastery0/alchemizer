using UnityEngine;
using static essenceManager;

public class healSphere : MonoBehaviour
{
    public int heal;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.instance.heal(heal);
            Destroy(gameObject);
        }
    }
}
