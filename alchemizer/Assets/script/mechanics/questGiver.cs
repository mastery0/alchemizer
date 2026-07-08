using UnityEngine;

public class questGiver : MonoBehaviour
{
    public quest quest;
    public void tryGiveQuest()
    {
        if (quest == null) return;
        if (!quest.canStartQuest()) return;
        quest.questStatus = questStatus.inProgress;
        quest.updateProgress(quest.questID,0);
    }
}
