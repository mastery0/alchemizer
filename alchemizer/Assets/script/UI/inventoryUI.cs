using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class inventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject itemPanel;
    public Sprite inventorySlotPrefab;

    private ScrollRect inventoryScrollRect;
    private ScrollRect itemScrollRect;
    private bool syncingScroll;

    private void Awake()
    {
        FindScrollRects();
    }

    private void OnEnable()
    {
        FindScrollRects();
        RegisterScrollSync();
        buildInvUI();
    }

    private void OnDisable()
    {
        UnregisterScrollSync();
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

    private void FindScrollRects()
    {
        if (inventoryPanel == null || itemPanel == null)
        {
            return;
        }

        ScrollRect[] scrollRects = GetComponentsInChildren<ScrollRect>(true);
        foreach (ScrollRect scrollRect in scrollRects)
        {
            if (scrollRect.content == inventoryPanel.transform)
            {
                inventoryScrollRect = scrollRect;
            }
            else if (scrollRect.content == itemPanel.transform)
            {
                itemScrollRect = scrollRect;
            }
        }
    }

    private void RegisterScrollSync()
    {
        UnregisterScrollSync();

        if (inventoryScrollRect != null)
        {
            inventoryScrollRect.onValueChanged.AddListener(SyncFromInventoryScroll);
        }

        if (itemScrollRect != null)
        {
            itemScrollRect.onValueChanged.AddListener(SyncFromItemScroll);
        }
    }

    private void UnregisterScrollSync()
    {
        if (inventoryScrollRect != null)
        {
            inventoryScrollRect.onValueChanged.RemoveListener(SyncFromInventoryScroll);
        }

        if (itemScrollRect != null)
        {
            itemScrollRect.onValueChanged.RemoveListener(SyncFromItemScroll);
        }
    }

    private void SyncFromInventoryScroll(Vector2 position)
    {
        SyncScroll(itemScrollRect, position);
    }

    private void SyncFromItemScroll(Vector2 position)
    {
        SyncScroll(inventoryScrollRect, position);
    }

    private void SyncScroll(ScrollRect target, Vector2 position)
    {
        if (syncingScroll || target == null)
        {
            return;
        }

        syncingScroll = true;
        target.normalizedPosition = position;
        syncingScroll = false;
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
