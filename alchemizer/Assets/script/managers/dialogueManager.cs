using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class dialogueManager : MonoBehaviour
{
    public static dialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public GameObject canvas;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.2f;

    //public Animator animator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        lines = new Queue<DialogueLine>();
        canvas.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue.oneTimeOnly && saveManager.instance.hasSeenDialogue(dialogue.dialogueID))
        {
            return;
        }
        saveManager.instance.markDialogueSeen(dialogue.dialogueID);
        canvas.SetActive(true);
        isDialogueActive = true;
        dialogue.shown = true;
        //animator.Play("show");

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
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        canvas.SetActive(false);
        //animator.Play("hide");
    }
}
