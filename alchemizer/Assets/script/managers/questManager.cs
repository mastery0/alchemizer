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
    public List<quest> questDB=new List<quest>();

    // QuestUI integration:
    // - use getActiveQuests()/getCompletedQuests() to build the journal
    // - subscribe to onQuestStarted/onQuestUpdated/onQuestCompleted and refresh visible rows
    // - read questName, questDescription, objectives, Essencereward and itemRewards from quest
    // - call completeQuest(quest) again if rewards failed because the inventory was full
    public System.Action<quest> onQuestStarted;
    public System.Action<quest> onQuestUpdated;
    public System.Action<quest> onQuestCompleted;

    private void Awake()
    {
        instance = this;
    }
    public void startQuest(quest quest)
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

    public bool completeQuest(quest quest)
    {
        if (quest == null) return false;
        if (!quest.completeAndReward()) return false;

        if(saveManager.instance!=null)saveManager.instance.completeQuest(quest.questID);
        onQuestCompleted?.Invoke(quest);
        onQuestUpdated?.Invoke(quest);
        return true;
    }

    public quest getQuest(string questID)
    {
        foreach(var item in questDB)
        {
            if (item.questID == questID) return item;
        }
        return null;
    }

    public List<quest> getActiveQuests()
    {
        List<quest> activeQuests = new List<quest>();
        foreach(var item in questDB)
        {
            if (item.questStatus == questStatus.inProgress) activeQuests.Add(item);
        }
        return activeQuests;
    }

    public List<quest> getCompletedQuests()
    {
        List<quest> completedQuests = new List<quest>();
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
            quest quest = getQuest(questID);
            if (quest == null) continue;
            quest.questStatus = questStatus.completed;
            foreach(var objective in quest.objectives)
            {
                objective.currentAmount = objective.requiredAmount;
                objective.status = questStatus.completed;
            }
        }

        foreach(string questID in activeQuestsID)
        {
            quest quest = getQuest(questID);
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
}
