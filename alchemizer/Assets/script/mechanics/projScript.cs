using UnityEngine;

public class projScript : MonoBehaviour
{
    float atk;
    private void Start()
    {
        float atk=GetComponentInParent<enemy>().damage;
        Destroy(gameObject,3f);
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.instance.takeDamage(atk);
            Debug.Log(player.instance.hp);
            Destroy(gameObject);
        }
    }
}

