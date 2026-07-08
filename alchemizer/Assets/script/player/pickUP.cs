using UnityEngine;

public class pickUP : MonoBehaviour
{
    public itemData item;
    public int amount = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { 
            if(inventory.instance.addItem(item, amount))Destroy(gameObject);
            else Debug.Log("Inventory Full");
        }
    }
}
