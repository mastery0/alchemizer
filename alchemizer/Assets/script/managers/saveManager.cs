using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class saveManager : MonoBehaviour
{
    public skillSO[] allSkills;
    public static saveManager instance;
    private static SaveData pendingLoadData;
    private List<string> seenDialogue=new List<string>();
    private List<string> openedChest = new List<string>();
    private List<string> activeQuestsID = new List<string>();
    private List<string> completedQuestsID = new List<string>();
    private List<itemStack> inventory = new List<itemStack>();

    [System.Serializable]
    public class inventorySaveData
    {
        public string itemID;
        public string itemName;
        public int amount;
    }
    [System.Serializable]
    public class SaveData
    {
        public float maxHp;
        public Vector2 respawnAltar;
        public int respawnscene;

        public int airEss;
        public int waterEss;
        public int fireEss;
        public int lightEss;
        public int darkEss;

        public int[] unlockedSkillIDs;
        public string[] seenDialogueIDs;
        public string[] openedChestIDs;
        public itemStack[] inventory;
        public inventorySaveData[] inventoryItems;
        public string[] activeQuestsIDs;
        public string[] completedQuestsIDs;
        public questSaveData[] questProgress;
    }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (pendingLoadData != null)
        {
            StartCoroutine(ApplyPendingLoadWhenReady());
        }
    }
    public bool hasSeenDialogue(string dialogueID)
    {
        return seenDialogue.Contains(dialogueID);
    }
    public void markDialogueSeen(string dialogueID)
    {
        if (!seenDialogue.Contains(dialogueID))
        {
            seenDialogue.Add(dialogueID);
        }
    }
    public bool hasOpenedChest(string chestID)
    {
        return openedChest.Contains(chestID);
    }
    public void markChestOpened(string chestID)
    {
        if (!openedChest.Contains(chestID))
        {
            openedChest.Add(chestID);
        }
    }
    public void addQuest(string newQuest)
    {
        if (!activeQuestsID.Contains(newQuest) && !completedQuestsID.Contains(newQuest))
        {
            activeQuestsID.Add(newQuest);
        }
    }
    public void completeQuest(string completedQuestID)
    {
        if (activeQuestsID.Contains(completedQuestID))
        {
            activeQuestsID.Remove(completedQuestID);
            completedQuestsID.Add(completedQuestID);
        }
    }
    public bool isQuestActive(string questToCheck)
    {
        return activeQuestsID.Contains(questToCheck);
    }
    public bool isQuestCompleted(string questToCheck)
    {
        return completedQuestsID.Contains(questToCheck);
    }
    [ContextMenu("save")]
    public void save()
    {
        SaveData data = new SaveData();
        // Player stats are rebuilt by the saved skills when loading.
        // maxHp is retained in SaveData only to keep old save files compatible.
        data.respawnAltar=player.instance.respawnAltar;
        data.respawnscene=player.instance.respawnScene;

        data.airEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.air];
        data.waterEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.water];
        data.fireEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire];
        data.lightEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.light];
        data.darkEss = essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark];

        List<int> unlocked = new List<int>();
        foreach (skillSO skill in allSkills)
        {
            if (skill.isUnlocked)
            {
                unlocked.Add(skill.skillID);
            }
        }
        data.unlockedSkillIDs = unlocked.ToArray();
        data.seenDialogueIDs = seenDialogue.ToArray();
        data.openedChestIDs = openedChest.ToArray();
        inventory.Clear();
        if (global::inventory.instance != null)
        {
            inventory.AddRange(global::inventory.instance.items);
        }

        // inventoryItems is the persistent representation. The legacy field is
        // retained exclusively so files created by previous versions can load.
        data.inventory = new itemStack[0];
        data.inventoryItems = getInventorySaveData();
        data.activeQuestsIDs = activeQuestsID.ToArray();
        data.completedQuestsIDs = completedQuestsID.ToArray();
        if (questManager.instance != null)
        {
            data.questProgress = questManager.instance.getQuestProgressData();
        }
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }

    [ContextMenu("load")]
    public void load()
    {
        string path = Application.persistentDataPath + "/save.json";
        Debug.Log("Loading save data from: " + path);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data == null)
            {
                Debug.LogWarning("Save file could not be read.");
                return;
            }
            pendingLoadData = data;
            Time.timeScale = 1f;
            SceneManager.LoadScene(data.respawnscene);
        }
    }

    public void applyPendingLoad()
    {
        if (pendingLoadData == null) return;
        if (!CanApplyData()) return;

        ApplyData(pendingLoadData);
        pendingLoadData = null;
    }

    private IEnumerator ApplyPendingLoadWhenReady()
    {
        while (pendingLoadData != null && !CanApplyData())
        {
            yield return null;
        }

        applyPendingLoad();
    }

    private bool CanApplyData()
    {
        return player.instance != null && essenceManager.instance != null && coreInstability.instance != null && global::inventory.instance != null;
    }

    private void ApplyData(SaveData data)
    {
        player.instance.respawnScene = data.respawnscene;
        player.instance.respawnAltar = data.respawnAltar;
        player.instance.transform.position = data.respawnAltar;
        player.instance.isAlive = true;

        essenceManager.instance.essenceInv[essenceManager.essenceTypes.air]=data.airEss;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.water]=data.waterEss;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire]=data.fireEss;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.light]=data.lightEss;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark] = data.darkEss;

        foreach (skillSO skill in allSkills)
        {
            skill.isUnlocked= false;
            foreach (int id in data.unlockedSkillIDs ?? new int[0])
            {
                if (id != skill.skillID) continue;
                skill.applyEffects();
                break;
            }
        }

        seenDialogue.Clear();
        foreach (string dialogueID in data.seenDialogueIDs ?? new string[0])
        {
            seenDialogue.Add(dialogueID);
        }

        openedChest.Clear();
        foreach (string chestID in data.openedChestIDs ?? new string[0])
        {
            openedChest.Add(chestID);
        }

        applySavedInventory(data);

        activeQuestsID.Clear();
        string[] savedActiveQuests = data.activeQuestsIDs ?? new string[0];
        foreach (string questID in savedActiveQuests)
        {
            activeQuestsID.Add(questID);
        }

        completedQuestsID.Clear();
        string[] savedCompletedQuests = data.completedQuestsIDs ?? new string[0];
        foreach (string questID in savedCompletedQuests)
        {
            completedQuestsID.Add(questID);
        }

        if (questManager.instance != null)
        {
            questManager.instance.applySavedQuests(savedActiveQuests, savedCompletedQuests, data.questProgress);
        }
    }
    [ContextMenu("reset")]
    public void toDefault()
    {
        player.instance.maxHp = 100;
        player.instance.respawnAltar = new Vector2(0,0);
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.air] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.water] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.fire] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.light] = 0;
        essenceManager.instance.essenceInv[essenceManager.essenceTypes.dark] = 0;

        seenDialogue.Clear();
        openedChest.Clear();
        inventory.Clear();
        if (global::inventory.instance != null)
        {
            global::inventory.instance.items.Clear();
        }
        activeQuestsID.Clear();
        completedQuestsID.Clear();
        if (questManager.instance != null)
        {
            questManager.instance.resetQuestDB();
        }
        foreach (skillSO skill in allSkills)
        {
            skill.isUnlocked= false;
        }
    }

    private inventorySaveData[] getInventorySaveData()
    {
        List<inventorySaveData> savedItems = new List<inventorySaveData>();
        foreach (itemStack stack in inventory)
        {
            if (stack == null || stack.item == null || stack.amount <= 0) continue;

            savedItems.Add(new inventorySaveData
            {
                itemID = stack.item.itemID,
                itemName = stack.item.name,
                amount = stack.amount
            });
        }
        return savedItems.ToArray();
    }

    private void applySavedInventory(SaveData data)
    {
        List<itemStack> loadedItems = new List<itemStack>();

        if (data.inventoryItems != null)
        {
            foreach (inventorySaveData savedItem in data.inventoryItems)
            {
                if (savedItem == null || savedItem.amount <= 0) continue;

                itemData item = getItemFromSaveData(savedItem);
                if (item == null)
                {
                    Debug.LogWarning("Could not restore saved inventory item: " + savedItem.itemID);
                    continue;
                }

                loadedItems.Add(new itemStack { item = item, amount = savedItem.amount });
            }
        }
        // Supports save files created before inventoryItems was added.
        else if (data.inventory != null)
        {
            foreach (itemStack stack in data.inventory)
            {
                if (stack != null && stack.item != null && stack.amount > 0)
                {
                    loadedItems.Add(stack);
                }
            }
        }

        inventory.Clear();
        inventory.AddRange(loadedItems);
        global::inventory.instance.items.Clear();
        global::inventory.instance.items.AddRange(loadedItems);
    }

    private itemData getItemFromSaveData(inventorySaveData savedItem)
    {
        itemData[] allItems = Resources.FindObjectsOfTypeAll<itemData>();
        foreach (itemData item in allItems)
        {
            if (!string.IsNullOrEmpty(savedItem.itemID) && item.itemID == savedItem.itemID)
            {
                return item;
            }

            if (string.IsNullOrEmpty(savedItem.itemID) && item.name == savedItem.itemName)
            {
                return item;
            }
        }
        return null;
    }
}
