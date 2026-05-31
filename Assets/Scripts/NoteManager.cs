using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NoteManager : MonoBehaviour
{
    [SerializeField] GameObject noteUI;
    [SerializeField] TMP_Text noteText;
    [SerializeField] FirstPersonController firstPersonController;
    [SerializeField] int totalNotes = 5;
    [SerializeField] string nextScene = "NombreDeTuEscena";

    bool isReading = false;
    int notesRead = 0;

    void Update()
    {
        if (isReading && Input.GetKeyDown(KeyCode.F))
        {
            CloseNote();
        }
    }

    public void OpenNote(string text)
    {
        noteText.text = text;
        noteUI.SetActive(true);
        isReading = true;

        notesRead++;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (firstPersonController != null)
            firstPersonController.EnableControls(false);
    }

    public void CloseNote()
    {
        noteUI.SetActive(false);
        isReading = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (firstPersonController != null)
            firstPersonController.EnableControls(true);

        if (notesRead >= totalNotes)
        {
            Invoke("LoadNextScene", 1f);
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}