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
    public bool oneTimeOnly = true;
    public bool shown = false;
    public bool isQuestDialogue = false;
    [HideInInspector] public GameObject npc;
}

public class dialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private void Awake()
    {
        if(dialogue.isQuestDialogue)
        {
            dialogue.npc = gameObject;
        }
    }
    public void TriggerDialogue()
    {
        dialogueManager.Instance.StartDialogue(dialogue);
        if (questManager.instance != null) questManager.instance.updateQuestProgress(questType.talk, dialogue.dialogueID);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerDialogue();
        }
    }
}
