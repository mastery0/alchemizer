using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class dialogueManager : MonoBehaviour
{
    public static dialogueManager instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public GameObject canvas;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;

    public bool isTalking = false;
    //public Animator animator;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        lines = new Queue<DialogueLine>();
        canvas.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (isTalking) return;
        if (dialogue.oneTimeOnly && saveManager.instance.hasSeenDialogue(dialogue.dialogueID))
        {
            return;
        }
        Time.timeScale = 0;
        isTalking= true;
        saveManager.instance.markDialogueSeen(dialogue.dialogueID);
        canvas.SetActive(true);
        isDialogueActive = true;
        dialogue.shown = true;
        //animator.Play("show");
        if (dialogue.npc != null)
        {
            questGiver qg;

            if (dialogue.npc.TryGetComponent<questGiver>(out qg))
            {
                qg.tryGiveQuest(dialogue.dialogueID);
            }
        }
        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        canvas.SetActive(false);
        isTalking=false;
        Time.timeScale = 1;
        //animator.Play("hide");
    }
}
