using UnityEngine;

public class projScript : MonoBehaviour
{
    float atk;
    private void Start()
    {
        Destroy(gameObject,3f);
    }
    public void setDamage(float damage)
    {
        atk=damage;
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer==LayerMask.NameToLayer("Ground"))Destroy(gameObject);
        if (collision.CompareTag("Player"))
        {
            player.instance.takeDamage(atk);
            Debug.Log(player.instance.hp);
            Destroy(gameObject);
        }
    }
}

