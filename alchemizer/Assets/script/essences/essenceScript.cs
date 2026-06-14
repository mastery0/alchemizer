using UnityEngine;

public class essenceScript : MonoBehaviour
{
    public essenceManager.essenceTypes essenceType;
    public int amount;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            essenceManager.instance.modifyAmount(essenceType, amount);
            Destroy(gameObject);
        }
    }
}
