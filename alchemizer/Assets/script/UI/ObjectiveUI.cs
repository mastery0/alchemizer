using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public TMP_Text text;

    public void Setup(questObjective obj)
    {
        string check = obj.status == questStatus.completed ? "✓" : "□";

        text.text =
            check +
            " " +
            obj.type +
            " : " +
            obj.currentAmount +
            "/" +
            obj.requiredAmount;
    }
}