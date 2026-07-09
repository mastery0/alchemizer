using UnityEngine;

public class questProgressTrigger : MonoBehaviour
{
    public questType type = questType.reach;
    public string targetID;
    public int amount = 1;
    public bool oneTimeOnly = true;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (oneTimeOnly && triggered) return;
        if (!collision.CompareTag("Player")) return;

        // Used by reach/custom objectives: targetID must match questObjective.targetID.
        triggered = true;
        questManager.instance.updateQuestProgress(type, targetID, amount);
    }
}
