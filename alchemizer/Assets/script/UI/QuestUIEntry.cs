using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIEntry : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text status;

    private quest quest;
    private QuestUI ui;

    public Button button;

    public void Setup(quest q, QuestUI questUI)
    {
        quest = q;
        ui = questUI;

        title.text = q.questName;
        status.text = q.questStatus.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        ui.ShowQuest(quest);
    }
}