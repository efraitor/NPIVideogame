using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogBox;
    public TextMeshProUGUI dialogContent;
    private Coroutine running;

    public void Show(string[] lines, float delay)
    {
        if (running != null) StopCoroutine(running);
        dialogBox.SetActive(true);
        running = StartCoroutine(RunDialog(lines, delay));
    }

    private IEnumerator RunDialog(string[] lines, float delay)
    {
        foreach (var line in lines)
        {
            dialogContent.text = line;
            yield return new WaitForSeconds(delay);
        }
        dialogBox.SetActive(false);
        running = null;
    }
}