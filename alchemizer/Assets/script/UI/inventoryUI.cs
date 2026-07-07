using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class inventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject itemPanel;
    public Sprite inventorySlotPrefab;

    private void OnEnable()
    {
        buildInvUI();
    }

    public void buildInvUI()
    {
        if (inventory.instance == null || inventoryPanel == null || itemPanel == null)
        {
            Debug.LogWarning("Inventory UI is missing a reference.");
            return;
        }

        ClearChildren(inventoryPanel.transform, itemPanel.transform);
        ClearChildren(itemPanel.transform);

        for (int i = 0; i < inventory.instance.maxSlots; i++)
        {
            CreateImage("itemSlot", inventoryPanel.transform, inventorySlotPrefab);
        }

        for (int i = 0; i < inventory.instance.items.Count; i++)
        {
            itemStack stack = inventory.instance.items[i];
            GameObject slot = CreateImage("item", itemPanel.transform, stack.item.itemIcon);

            TextMeshProUGUI amountText = new GameObject("amountText", typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
            amountText.transform.SetParent(slot.transform, false);
            amountText.text = stack.amount.ToString();
            amountText.alignment = TextAlignmentOptions.BottomRight;
            amountText.fontSize = 18;
            amountText.color = Color.white;

            RectTransform amountRect = amountText.GetComponent<RectTransform>();
            amountRect.anchorMin = Vector2.zero;
            amountRect.anchorMax = Vector2.one;
            amountRect.offsetMin = Vector2.zero;
            amountRect.offsetMax = Vector2.zero;
        }
    }

    private void ClearChildren(Transform parent, Transform childToKeep = null)
    {
        foreach (Transform child in parent)
        {
            if (child == childToKeep) continue;
            Destroy(child.gameObject);
        }
    }

    private GameObject CreateImage(string objectName, Transform parent, Sprite sprite)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(64, 64);

        return imageObject;
    }
}
