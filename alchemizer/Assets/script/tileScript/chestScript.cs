using UnityEngine;

public class chestScript : MonoBehaviour
{
    public bool isOpen = false;
    public string chestID;
    public itemStack item;
    public bool isLocked = false;
    public string keyID;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        isOpen = saveManager.instance.hasOpenedChest(chestID);
        if (isOpen) return;
        if (collision.CompareTag("Player"))
        {
            OpenChest();
            //spawn img for player to click on and open the chest
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isOpen) return;
        if (collision.CompareTag("Player"))
        {
            //remove img for player to click on and open the chest
        }
    }

    public void OpenChest()
    {
        if (isOpen || saveManager.instance.hasOpenedChest(chestID))
        {
            Debug.Log("chest already opened");
            return;
        }
        if (isLocked)
        {
            if (!inventory.instance.hasItem(keyID))
            {
                Debug.Log("chest is locked");
                return;
            }
            Debug.Log("chest unlocked");
        }
        if (inventory.instance.addItem(item.item, item.amount))
        {
            Debug.Log("item added");
            if (questManager.instance != null) questManager.instance.updateQuestProgress(questType.collect, item.item.itemID, item.amount);
            isOpen = true;
            if (saveManager.instance != null) saveManager.instance.markChestOpened(chestID);
        }
        else Debug.Log("Inventory Full");
    }
}
