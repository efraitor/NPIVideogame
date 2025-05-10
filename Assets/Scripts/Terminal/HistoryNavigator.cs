using System.Collections.Generic;
using UnityEngine;
using TMPro;

// HistoryNavigator.cs
// Gestiona la navegación por flechas a través del historial de comandos
public class HistoryNavigator : MonoBehaviour
{
    [Header("Dependencies")]
    public TerminalManager terminalManager;
    public TMP_InputField inputField;

    private List<string> history = new List<string>();
    private int histIndex = 0;

    void Start()
    {
        if (terminalManager != null)
            terminalManager.OnCommandProcessed += OnCommandProcessed;
    }

    private void OnCommandProcessed(string cmd)
    {
        history.Add(cmd);
        histIndex = history.Count;
    }

    void Update()
    {
        if (history.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            histIndex = Mathf.Max(0, histIndex - 1);
            SetInputField(history[histIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            histIndex = Mathf.Min(history.Count, histIndex + 1);
            if (histIndex == history.Count)
                SetInputField(string.Empty);
            else
                SetInputField(history[histIndex]);
        }
    }

    private void SetInputField(string text)
    {
        inputField.text = text;
        inputField.caretPosition = text.Length;
    }
}