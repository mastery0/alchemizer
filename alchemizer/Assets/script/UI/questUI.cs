using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("Quest List")]
    public Transform questListParent;
    public GameObject questEntryPrefab;

    [Header("Details")]
    public TMP_Text questName;
    public TMP_Text questDescription;
    public TMP_Text rewardText;

    public Transform objectiveParent;
    public GameObject objectivePrefab;

    private quest selectedQuest;

    void Start()
    {
        questManager.instance.onQuestStarted += Refresh;
        questManager.instance.onQuestUpdated += Refresh;
        questManager.instance.onQuestCompleted += Refresh;

        BuildQuestList();
    }

    private void OnDestroy()
    {
        if (questManager.instance == null) return;

        questManager.instance.onQuestStarted -= Refresh;
        questManager.instance.onQuestUpdated -= Refresh;
        questManager.instance.onQuestCompleted -= Refresh;
    }

    void Refresh(quest q)
    {
        BuildQuestList();

        if (selectedQuest == q)
            ShowQuest(q);
    }

    public void BuildQuestList()
    {
        foreach (Transform child in questListParent)
            Destroy(child.gameObject);

        List<quest> quests = new List<quest>();

        quests.AddRange(questManager.instance.getActiveQuests());
        quests.AddRange(questManager.instance.getCompletedQuests());

        foreach (quest q in quests)
        {
            GameObject obj = Instantiate(questEntryPrefab, questListParent);

            QuestUIEntry entry = obj.GetComponent<QuestUIEntry>();

            entry.Setup(q, this);
        }
    }

    public void ShowQuest(quest q)
    {
        selectedQuest = q;

        questName.text = q.questName;
        questDescription.text = q.questDescription;

        foreach (Transform child in objectiveParent)
            Destroy(child.gameObject);

        foreach (var objective in q.objectives)
        {
            GameObject obj = Instantiate(objectivePrefab, objectiveParent);

            ObjectiveUI objectiveUI = obj.GetComponent<ObjectiveUI>();

            objectiveUI.Setup(objective);
        }

        rewardText.text = "";

        foreach (var item in q.itemRewards)
        {
            rewardText.text += item.item.itemName + " x" + item.amount + "\n";
        }

        if (q.Essencereward.Length > 0)
        {
            rewardText.text += "\nEssence: " + q.essenceRewardAmount;
        }
    }
}