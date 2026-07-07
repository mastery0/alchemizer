using Unity.VisualScripting;
using UnityEngine;
    public enum itemType
    {
        keyItem,
        consumable,
        material,
        equipment
    }
[CreateAssetMenu(menuName = "Alchemizer/Item")]
public class itemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemID;
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite itemIcon;
    public itemType type;
    public int maxStack = 99;
    public GameObject worldPrefab;
}
[System.Serializable]
public class itemStack
{
    public itemData item;
    public int amount;
}
