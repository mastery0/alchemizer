using JetBrains.Annotations;
using Unity.VisualScripting;
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
    }
    public questStatus checkstatus()
    {
        if (currentAmount >= requiredAmount)
        {
            return questStatus.completed;
        }
        else
        {
            return questStatus.inProgress;
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
        foreach (var item in objectives) {
            if(!(item.checkstatus()==questStatus.completed))return false; 
        }
        if(questStatus!=questStatus.notStarted)return false;
        return true;
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
    public void updateProgress(string targetID, int amount)
    {
        foreach (var item in objectives)
        {
            if (item.targetID == targetID)
            {
                item.currentAmount += amount;
                item.status = item.checkstatus();
            }
        }
    }

    public bool completeAndReward()
    {
        foreach(var item in objectives)
        {
            if (item.status == questStatus.completed) return false;
        }
        int i = Essencereward.Length;
        foreach(var item in Essencereward)
        {
            essenceManager.instance.modifyAmount(item, essenceRewardAmount / i);
        }
        foreach(var item in itemRewards)
        {
            if (!inventory.instance.addItem(item.item, item.amount))
            {
                Debug.Log("Not enough space in inventory for quest reward");
                return false;
            }
        }
        return true;
    }
}
