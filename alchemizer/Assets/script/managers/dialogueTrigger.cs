using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public string dialogueID;
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
    [Header("Visibility")]
    public string requiredCompletedDialogueID;
    public string requiredActiveQuestID;
    public string requiredCompletedQuestID;

    [Header("Objective requirement")]
    [Tooltip("Quest che contiene l'obiettivo da completare prima di rendere disponibile questo dialogo. Lascia vuoto per non richiedere alcun obiettivo.")]
    public questSO requiredCompletedObjectiveQuest;

    [Min(0)]
    [Tooltip("Indice dell'obiettivo nella quest selezionata (0 = primo obiettivo).")]
    public int requiredCompletedObjectiveIndex;

    public bool oneTimeOnly = true;
    public bool shown = false;
    public bool isQuestDialogue = false;
    [HideInInspector] public GameObject npc;
    public bool canStart()
    {
        if (saveManager.instance == null) return false;
        if (oneTimeOnly && saveManager.instance.hasSeenDialogue(dialogueID)) return false;
        if (!string.IsNullOrEmpty(requiredCompletedDialogueID) && !saveManager.instance.hasSeenDialogue(requiredCompletedDialogueID)) return false;
        if (!string.IsNullOrEmpty(requiredActiveQuestID) && !saveManager.instance.isQuestActive(requiredActiveQuestID)) return false;
        if (!string.IsNullOrEmpty(requiredCompletedQuestID) && !saveManager.instance.isQuestCompleted(requiredCompletedQuestID)) return false;
        if (requiredCompletedObjectiveQuest != null)
        {
            if (!requiredCompletedObjectiveQuest.isObjectiveCompleted(requiredCompletedObjectiveIndex)) return false;
        }
        return true;
    }
}

public class dialogueTrigger : MonoBehaviour
{
    public List<Dialogue> dialogues=new();
    private void Awake()
    {
        foreach (Dialogue dialogue in dialogues)
        {
            if (dialogue != null) dialogue.npc = gameObject;
        }
    }
    public void TriggerDialogue()
    {
        foreach (Dialogue dialogue in dialogues)
        {
            Debug.Log(dialogue.dialogueID);
            if (!dialogue.canStart()) continue;
            dialogueManager.instance.StartDialogue(dialogue);
            if (questManager.instance != null) questManager.instance.updateQuestProgress(questType.talk, dialogue.dialogueID);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerDialogue();
        }
    }
}
