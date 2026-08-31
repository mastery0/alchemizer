using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class questSaveData
{
    public string questID;
    public int[] objectiveProgress;
}

public class questManager : MonoBehaviour
{
    public static questManager instance;
    public List<questSO> questDB=new List<questSO>();

    // QuestUI integration:
    // - use getActiveQuests()/getCompletedQuests() to build the journal
    // - subscribe to onQuestStarted/onQuestUpdated/onQuestCompleted and refresh visible rows
    // - read questName, questDescription, objectives, Essencereward and itemRewards from quest
    // - call completeQuest(quest) again if rewards failed because the inventory was full
    public System.Action<questSO> onQuestStarted;
    public System.Action<questSO> onQuestUpdated;
    public System.Action<questSO> onQuestCompleted;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Supports scenes or saves where a quest was already marked complete.
        Debug.Log(questDB!=null);
        foreach (questSO quest in questDB)
        {
            Debug.Log(quest!=null);
            if (quest.questStatus == questStatus.completed) unlockQuestPotion(quest);
        }
    }
    public void startQuest(questSO quest)
    {
        if (quest == null) return;
        if(!quest.canStartQuest())return;

        quest.startQuest();
        if(saveManager.instance!=null)saveManager.instance.addQuest(quest.questID);

        onQuestStarted?.Invoke(quest);
        onQuestUpdated?.Invoke(quest);
        if (quest.canCompleteQuest()) completeQuest(quest);
    }

    public void updateQuestProgress(questType type,string targetID,int amount=1)
    {
        foreach(var quest in questDB)
        {
            if (quest.questStatus != questStatus.inProgress) continue;
            if (!quest.updateProgress(type, targetID, amount)) continue;

            if (quest.canCompleteQuest()) completeQuest(quest);
            else onQuestUpdated?.Invoke(quest);
        }
    }

    public void updateQuestProgress(string targetID,int amount=1)
    {
        foreach (var quest in questDB)
        {
            if (quest.questStatus != questStatus.inProgress) continue;
            if (!quest.updateProgress(targetID, amount)) continue;

            if (quest.canCompleteQuest()) completeQuest(quest);
            else onQuestUpdated?.Invoke(quest);
        }
    }

    public bool completeQuest(questSO quest)
    {
        if (quest == null) return false;
        if (!quest.completeAndReward()) return false;

        unlockQuestPotion(quest);
        if(saveManager.instance!=null)saveManager.instance.completeQuest(quest.questID);
        onQuestCompleted?.Invoke(quest);
        onQuestUpdated?.Invoke(quest);
        return true;
    }

    public questSO getQuest(string questID)
    {
        foreach(var item in questDB)
        {
            if (item.questID == questID) return item;
        }
        return null;
    }

    public List<questSO> getActiveQuests()
    {
        List<questSO> activeQuests = new List<questSO>();
        foreach(var item in questDB)
        {
            if (item.questStatus == questStatus.inProgress) activeQuests.Add(item);
        }
        return activeQuests;
    }

    public List<questSO> getCompletedQuests()
    {
        List<questSO> completedQuests = new List<questSO>();
        foreach (var item in questDB)
        {
            if (item.questStatus == questStatus.completed) completedQuests.Add(item);
        }
        return completedQuests;
    }

    public questSaveData[] getQuestProgressData()
    {
        List<questSaveData> questProgress = new List<questSaveData>();
        foreach(var item in questDB)
        {
            if (item.questStatus != questStatus.inProgress) continue;

            questSaveData data = new questSaveData();
            data.questID = item.questID;
            data.objectiveProgress = new int[item.objectives.Length];
            for(int i=0;i<item.objectives.Length;i++)
            {
                data.objectiveProgress[i] = item.objectives[i].currentAmount;
            }
            questProgress.Add(data);
        }
        return questProgress.ToArray();
    }

    public void applySavedQuests(string[] activeQuestsID,string[] completedQuestsID,questSaveData[] questProgress)
    {
        resetQuestDB();

        foreach(string questID in completedQuestsID)
        {
            questSO quest = getQuest(questID);
            if (quest == null) continue;
            quest.questStatus = questStatus.completed;
            foreach(var objective in quest.objectives)
            {
                objective.currentAmount = objective.requiredAmount;
                objective.status = questStatus.completed;
            }
            unlockQuestPotion(quest);
        }

        foreach(string questID in activeQuestsID)
        {
            questSO quest = getQuest(questID);
            if (quest == null) continue;

            quest.questStatus = questStatus.inProgress;
            questSaveData data = getQuestSaveData(questID, questProgress);
            for(int i=0;i<quest.objectives.Length;i++)
            {
                quest.objectives[i].currentAmount = getSavedObjectiveAmount(data, i);
                quest.objectives[i].status = quest.objectives[i].checkstatus();
            }
            onQuestUpdated?.Invoke(quest);
        }
    }

    public void resetQuestDB()
    {
        foreach(var item in questDB)
        {
            item.questStatus = questStatus.notStarted;
            item.resetProgress();
        }
    }

    private questSaveData getQuestSaveData(string questID,questSaveData[] questProgress)
    {
        if (questProgress == null) return null;
        foreach(var item in questProgress)
        {
            if (item.questID == questID) return item;
        }
        return null;
    }

    private int getSavedObjectiveAmount(questSaveData data,int objectiveIndex)
    {
        if (data == null) return 0;
        if (data.objectiveProgress == null) return 0;
        if (objectiveIndex >= data.objectiveProgress.Length) return 0;
        return data.objectiveProgress[objectiveIndex];
    }

    private void unlockQuestPotion(questSO quest)
    {
        if (quest == null || healManager.instance == null) return;
        healManager.instance.unlockPotion(quest.potionRewardID);
    }
}
