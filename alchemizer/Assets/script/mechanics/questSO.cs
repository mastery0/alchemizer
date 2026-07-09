using UnityEngine;

public enum questType
{
    kill,
    collect,
    reach,
    talk,
    custom
}
public enum questStatus
{
    notStarted,
    inProgress,
    completed
}
[System.Serializable]
public class questObjective
{
    public questType type;
    public questStatus status;
    public string targetID;
    public int requiredAmount;
    public int currentAmount;
    public void ResetProgress()
    {
        currentAmount = 0;
        status = global::questStatus.notStarted;
    }
    public questStatus checkstatus()
    {
        if (currentAmount >= requiredAmount)
        {
            return global::questStatus.completed;
        }
        else
        {
            return global::questStatus.inProgress;
        }
    }
}
[CreateAssetMenu(menuName = "Alchemizer/Quest")]
public class quest : ScriptableObject
{
    public string questID;
    public string questName;
    public questStatus questStatus;
    [TextArea] public string questDescription;
    public questObjective[] objectives;
    public quest[] prerequisites;
    public essenceManager.essenceTypes[] Essencereward;
    public int essenceRewardAmount;
    public itemStack[] itemRewards;

    public bool canStartQuest()
    {
        foreach (var item in prerequisites) {
            if (item.questStatus != global::questStatus.completed) return false;
        }
        if(questStatus!=global::questStatus.notStarted)return false;
        return true;
    }
    public void startQuest()
    {
        questStatus = global::questStatus.inProgress;
        resetProgress();
        foreach (var item in objectives)
        {
            item.status = item.checkstatus();
        }
    }
    public void setID()
    {
        foreach (var item in objectives)
        {
            item.targetID = questID;
        }
    }
    public void resetProgress()
    {
        foreach (var item in objectives)
        {
            item.ResetProgress();
        }
    }
    public bool updateProgress(questType type, string targetID, int amount)
    {
        bool updated = false;
        foreach (var item in objectives)
        {
            if (item.status == global::questStatus.completed) continue;
            if (item.type != type) continue;
            if (item.targetID == targetID)
            {
                item.currentAmount += amount;
                item.status = item.checkstatus();
                updated = true;
            }
        }
        return updated;
    }

    public bool updateProgress(string targetID, int amount)
    {
        bool updated = false;
        foreach (var item in objectives)
        {
            if (item.status == global::questStatus.completed) continue;
            if (item.targetID == targetID)
            {
                item.currentAmount += amount;
                item.status = item.checkstatus();
                updated = true;
            }
        }
        return updated;
    }

    public bool canCompleteQuest()
    {
        if (questStatus != global::questStatus.inProgress) return false;
        foreach(var item in objectives)
        {
            // Keeps objective status aligned even when progress was restored from a save.
            item.status = item.checkstatus();
            if (item.status != global::questStatus.completed) return false;
        }
        return true;
    }

    public bool completeAndReward()
    {
        if (!canCompleteQuest()) return false;

        foreach(var item in itemRewards)
        {
            if (!inventory.instance.addItem(item.item, item.amount))
            {
                Debug.Log("Not enough space in inventory for quest reward");
                return false;
            }
        }

        int i = Essencereward.Length;
        foreach(var item in Essencereward)
        {
            essenceManager.instance.modifyAmount(item, essenceRewardAmount / i);
        }
        questStatus = global::questStatus.completed;
        return true;
    }
}
