using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NanoManager : MonoBehaviour
{
    public static NanoManager Instance;
    public GameObject nanoPanel;
    public TMP_InputField nanoInput;
    public Button saveButton;
    private string currentFilePath;

    void Awake()
    {
        Instance = this;
        nanoPanel.SetActive(false);
        saveButton.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(currentFilePath))
                File.WriteAllText(currentFilePath, nanoInput.text);
            nanoPanel.SetActive(false);
        });
    }

    public void Open(string filePath)
    {
        currentFilePath = filePath;
        nanoInput.text = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
        nanoPanel.SetActive(true);
        nanoInput.ActivateInputField();
    }
}