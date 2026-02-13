using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] GameObject dialogueMark;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] string[] dialogueLines;
    [SerializeField] float typingTime;
    bool isPlayerInRange = false;
    public bool isDialogueStarted = false;
    int lineIndex;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
          if (!isDialogueStarted)
          {
             StartDialogue();
          }
          else if (dialogueText.text == dialogueLines[lineIndex])
          {
             NextLine();
          }
          else 
          { 
            StopAllCoroutines();
            dialogueText.text = dialogueLines[lineIndex];
          }
        }
    }
    void StartDialogue()
    {
        isDialogueStarted = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        StartCoroutine(ShowLine());
    }
    void NextLine()
    {
       lineIndex++;
        if (lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            isDialogueStarted = false;
            dialoguePanel.SetActive(false);
            dialogueMark.SetActive(true);
        }
    }
    IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        foreach(char letter in dialogueLines[lineIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingTime);
        }
    }
    private void OnTriggerEnter3D(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            dialogueMark.SetActive(true);
            Debug.Log("Player entered dialogue range.");
        }
    }

    private void OnTriggerExit3D(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueMark.SetActive(false);
            Debug.Log("Player exit dialogue range.");
        }
    }
}
