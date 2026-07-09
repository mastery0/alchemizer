using UnityEngine;

public class questGiver : MonoBehaviour
{
    public quest quest;
    public void tryGiveQuest()
    {
        if (quest == null) return;
        questManager.instance.startQuest(quest);
    }
}
