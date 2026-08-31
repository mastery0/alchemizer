using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueQuestAssignment
{
    [Tooltip("ID del dialogo che deve avviare questa quest.")]
    public string dialogueID;

    [Tooltip("Quest assegnata quando viene avviato il dialogo indicato.")]
    public questSO quest;
}

public class questGiver : MonoBehaviour
{
    [Header("Quest per dialogo")]
    [Tooltip("Lo stesso NPC puo' assegnare piu' quest. Aggiungi una riga per ogni coppia dialogo/quest.")]
    public List<DialogueQuestAssignment> dialogueQuestAssignments = new();


    public void tryGiveQuest(string dialogueID)
    {
        if (questManager.instance == null) return;
        foreach (DialogueQuestAssignment assignment in dialogueQuestAssignments)
        {
            if (assignment == null || assignment.quest == null) continue;
            if (assignment.dialogueID != dialogueID) continue;
            questManager.instance.startQuest(assignment.quest);
        }
    }
}
