using UnityEngine;
using TMPro;
using System.Collections;

// Script mínimo para la Misión 1: navegar con 'cd' y listar con 'ls'
public class TerminalMission1 : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI historyText;
    public TMP_InputField editorInput;
    public GameObject dialogBox;
    public TextMeshProUGUI dialogContent;
    public float dialogDelay = 2f;

    private const int MISSION_INITIAL_1_CD_1 = 1;
    private const int MISSION_INITIAL_2_LS_2 = 2;

    private int missionActual = MISSION_INITIAL_1_CD_1;
    private string currentPath = "/";

    // Mensajes de la misión inicial 1
    private readonly string[] missionInitialDialogs1 = new string[]
    {
        "¡Buen trabajo! Has llegado a /home/usuario1/Documentos.",
        "Ahora prueba listar los archivos con 'ls'."
    };

    void Start()
    {
        // Aseguramos que es un campo de línea única para capturar Enter
        editorInput.lineType = TMP_InputField.LineType.SingleLine;
        // Listener para capturar Enter en On End Edit
        editorInput.onEndEdit.AddListener(OnCommandSubmitted);
        dialogBox.SetActive(false);
        // Activamos el foco inicial
        editorInput.ActivateInputField();
    }

    private void OnCommandSubmitted(string command)
    {
        // Solo ejecutar si fue por Enter
        if (!Input.GetKeyDown(KeyCode.Return)) return;

        command = command.Trim();
        if (string.IsNullOrEmpty(command))
        {
            editorInput.ActivateInputField();
            return;
        }

        ExecuteCommand(command);
        editorInput.text = string.Empty;
        editorInput.ActivateInputField();
    }

    private void ExecuteCommand(string command)
    {
        // Mostrar el comando en el historial
        historyText.text += "\n> " + command;

        if (command.StartsWith("cd "))
        {
            string target = command.Substring(3).Trim();
            string newPath = target.StartsWith("/")
                ? target
                : (currentPath.TrimEnd('/') + "/" + target);

            currentPath = newPath;
            historyText.text += "\nDirectorio actual: " + currentPath;

            if (currentPath == "/home/usuario1/Documentos" && missionActual == MISSION_INITIAL_1_CD_1)
            {
                missionActual = MISSION_INITIAL_2_LS_2;
                StartCoroutine(StartDialog(missionInitialDialogs1));
            }
        }
        else if ((command == "ls" || command.StartsWith("ls ")) && missionActual == MISSION_INITIAL_2_LS_2)
        {
            string[] files = new string[] { "informe.txt", "imagen.png", "notas.docx" };
            historyText.text += "\n" + string.Join("   ", files);
        }
        else
        {
            historyText.text += "\nComando no reconocido.";
        }
    }

    private IEnumerator StartDialog(string[] lines)
    {
        dialogBox.SetActive(true);
        foreach (var line in lines)
        {
            dialogContent.text = line;
            yield return new WaitForSeconds(dialogDelay);
        }
        dialogBox.SetActive(false);
    }
}
