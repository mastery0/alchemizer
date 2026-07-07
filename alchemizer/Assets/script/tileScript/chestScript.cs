using UnityEngine;

public class chestScript : MonoBehaviour
{
    public bool isOpen = false;
    public string chestID;
    public itemStack item;

    private void Start()
    {
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        isOpen = saveManager.instance.hasOpenedChest(chestID);
        if (isOpen) return;
        if (collision.CompareTag("Player"))
        {
            //spawn img for player to click on and open the chest
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if(isOpen) return;
        if (collision.CompareTag("Player"))
        {
            //remove img for player to click on and open the chest
        }
    }

    public void OpenChest()
    {
        isOpen = saveManager.instance.hasOpenedChest(chestID);
        if (isOpen)
        {
            Debug.Log("chest already opened");
            return;
        }
        isOpen =inventory.instance.addItem(item.item, item.amount);
        if(isOpen)saveManager.instance.markChestOpened(chestID);
    }
}
