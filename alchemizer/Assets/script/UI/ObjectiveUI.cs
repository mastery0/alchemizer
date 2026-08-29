using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public TMP_Text text;

    public void Setup(questObjective obj)
    {
        string objectiveText = string.IsNullOrWhiteSpace(obj.displayText) ? GetDefaultText(obj) : obj.displayText;
        text.text =objectiveText + " (" + obj.currentAmount + "/" + obj.requiredAmount +")";
    }

    private string GetDefaultText(questObjective obj)
    {
        string target = string.IsNullOrWhiteSpace(obj.targetID) ? "target" : obj.targetID;

        switch (obj.type)
        {
            case questType.kill:
                return "Defeat " + target;
            case questType.collect:
                return "Collect " + target;
            case questType.reach:
                return "Reach " + target;
            case questType.talk:
                return "Talk to " + target;
            default:
                return target;
        }
    }
}
